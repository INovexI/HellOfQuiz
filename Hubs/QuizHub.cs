using Microsoft.AspNetCore.SignalR;
using HellOfQuiz.Services;
using HellOfQuiz.Models;

namespace HellOfQuiz.Hubs
{
    public class QuizHub : Hub
    {
        private readonly RoomService _roomService;

        public QuizHub(RoomService roomService)
        {
            _roomService = roomService;
        }

        // Host Odayı Sahiplenir
        public async Task RegisterHost(string pin)
        {
            var room = _roomService.GetRoom(pin);
            if (room != null)
            {
                room.HostConnectionId = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, pin);
            }
        }

        public async Task JoinRoom(string pin, string playerName)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null)
            {
                await Clients.Caller.SendAsync("Error", "Oda bulunamadı!");
                return;
            }

            var player = room.Players.FirstOrDefault(p => p.Name == playerName);

            if (player != null)
            {
                // Oyuncu tekrar bağlandı
                player.ConnectionId = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, pin);
                await Clients.Caller.SendAsync("JoinedRoom", pin, playerName);
                
                if (room.State == RoomState.ShowingQuestion)
                {
                    await Clients.Caller.SendAsync("ShowQuestion", room.TimeLimitSeconds);
                }
                else if (room.State == RoomState.ShowingAnswer)
                {
                    int rank = room.Players.OrderByDescending(p => p.Score).ToList().IndexOf(player) + 1;
                    await Clients.Caller.SendAsync("ShowPlayerResult", player.HasAnsweredCurrentQuestion, player.LastAnswerIsCorrect, player.Score, rank);
                }
                return;
            }

            if (room.State != RoomState.WaitingForPlayers)
            {
                await Clients.Caller.SendAsync("Error", "Oyun çoktan başladı!");
                return;
            }

            player = new Player
            {
                ConnectionId = Context.ConnectionId,
                Name = playerName
            };

            room.Players.Add(player);

            await Groups.AddToGroupAsync(Context.ConnectionId, pin);
            await Clients.Caller.SendAsync("JoinedRoom", pin, playerName);
            
            // Host'a yeni oyuncuyu bildir
            await Clients.Client(room.HostConnectionId).SendAsync("PlayerJoined", player.Name);
        }

        // Host Soruyu Başlatır
        public async Task NextQuestion(string pin)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null || room.HostConnectionId != Context.ConnectionId) return;

            room.CurrentQuestionIndex++;
            
            if (room.CurrentQuestionIndex >= room.Questions.Count)
            {
                room.State = RoomState.Finished;
                var topPlayers = room.Players.OrderByDescending(p => p.Score).Take(3).ToList();
                await Clients.Group(pin).SendAsync("GameFinished");
                await Clients.Client(room.HostConnectionId).SendAsync("ShowPodium", topPlayers);
                return;
            }

            room.State = RoomState.ShowingQuestion;
            room.CurrentQuestionStartTime = DateTime.UtcNow;
            var currentQuestion = room.Questions[room.CurrentQuestionIndex];

            // Oyuncuların cevap durumunu sıfırla
            foreach (var player in room.Players)
            {
                player.HasAnsweredCurrentQuestion = false;
                player.LastAnswerIsCorrect = false;
            }

            // Oyunculara sadece seçenekleri yolla
            await Clients.Group(pin).SendAsync("ShowQuestion", room.TimeLimitSeconds);
            // Host'a sorunun tam halini yolla
            await Clients.Client(room.HostConnectionId).SendAsync("ShowHostQuestion", currentQuestion, room.TimeLimitSeconds);
        }

        // Oyuncu Cevap Verir
        public async Task SubmitAnswer(string pin, string answer)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null || room.State != RoomState.ShowingQuestion) return;

            var player = room.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player == null || player.HasAnsweredCurrentQuestion) return;

            player.HasAnsweredCurrentQuestion = true;
            var currentQuestion = room.Questions[room.CurrentQuestionIndex];
            
            bool isCorrect = currentQuestion.CorrectAnswer.ToUpper() == answer.ToUpper();
            player.LastAnswerIsCorrect = isCorrect;
            
            if (isCorrect)
            {
                // Puan hesaplama: Kalan süreye göre bonus
                var timeTaken = (DateTime.UtcNow - room.CurrentQuestionStartTime).TotalSeconds;
                int basePoints = 1000;
                int timeBonus = (int)Math.Max(0, (room.TimeLimitSeconds - timeTaken) / room.TimeLimitSeconds * 500);
                
                player.Score += basePoints + timeBonus;
                player.ConsecutiveCorrectAnswers++;
            }
            else
            {
                player.ConsecutiveCorrectAnswers = 0;
            }

            await Clients.Caller.SendAsync("AnswerReceived", isCorrect);
            await Clients.Client(room.HostConnectionId).SendAsync("PlayerAnswered");
        }

        // Host süreyi bitirir ve cevabı gösterir
        public async Task ShowAnswer(string pin)
        {
            var room = _roomService.GetRoom(pin);
            if (room == null || room.HostConnectionId != Context.ConnectionId) return;

            room.State = RoomState.ShowingAnswer;
            var currentQuestion = room.Questions[room.CurrentQuestionIndex];

            var orderedPlayers = room.Players.OrderByDescending(p => p.Score).ToList();
            var top5 = orderedPlayers.Take(5).ToList();
            
            await Clients.Client(room.HostConnectionId).SendAsync("ShowHostAnswerResult", currentQuestion.CorrectAnswer, top5);

            for (int i = 0; i < orderedPlayers.Count; i++)
            {
                var player = orderedPlayers[i];
                int rank = i + 1;
                await Clients.Client(player.ConnectionId).SendAsync("ShowPlayerResult", player.HasAnsweredCurrentQuestion, player.LastAnswerIsCorrect, player.Score, rank);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var room = _roomService.GetRoomByHostId(Context.ConnectionId);
            if (room != null)
            {
                // Host çıktıysa odayı kapat
                Clients.Group(room.Pin).SendAsync("RoomClosed");
                _roomService.RemoveRoom(room.Pin);
            }
            else
            {
                // Çıkan oyuncuysa ve oyun henüz başlamadıysa listeden sil
                var playerRoom = _roomService.GetRoomByPlayerId(Context.ConnectionId);
                if (playerRoom != null)
                {
                    var player = playerRoom.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
                    if (player != null && playerRoom.State == RoomState.WaitingForPlayers)
                    {
                        playerRoom.Players.Remove(player);
                        Clients.Client(playerRoom.HostConnectionId).SendAsync("PlayerLeft", player.Name);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
