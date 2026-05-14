namespace HellOfQuiz.Models
{
    public class Room
    {
        public string Pin { get; set; } = string.Empty;
        public string HostConnectionId { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
        public int CurrentQuestionIndex { get; set; } = -1;
        public RoomState State { get; set; } = RoomState.WaitingForPlayers;
        public DateTime CurrentQuestionStartTime { get; set; }
        public int TimeLimitSeconds { get; set; } = 20;
    }

    public enum RoomState
    {
        WaitingForPlayers,
        ShowingQuestion,
        ShowingAnswer,
        Finished
    }
}
