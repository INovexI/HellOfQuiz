using System.Collections.Concurrent;
using HellOfQuiz.Models;

namespace HellOfQuiz.Services
{
    public class RoomService
    {
        private readonly ConcurrentDictionary<string, Room> _rooms = new();

        public string CreateRoom(List<Question> questions)
        {
            var pin = GeneratePin();
            var room = new Room
            {
                Pin = pin,
                Questions = questions
            };
            
            _rooms.TryAdd(pin, room);
            return pin;
        }

        public Room? GetRoom(string pin)
        {
            _rooms.TryGetValue(pin, out var room);
            return room;
        }

        public Room? GetRoomByHostId(string hostConnectionId)
        {
            return _rooms.Values.FirstOrDefault(r => r.HostConnectionId == hostConnectionId);
        }

        public Room? GetRoomByPlayerId(string playerConnectionId)
        {
            return _rooms.Values.FirstOrDefault(r => r.Players.Any(p => p.ConnectionId == playerConnectionId));
        }

        public void RemoveRoom(string pin)
        {
            _rooms.TryRemove(pin, out _);
        }

        private string GeneratePin()
        {
            var random = new Random();
            string pin;
            do
            {
                pin = random.Next(100000, 999999).ToString();
            } while (_rooms.ContainsKey(pin));
            return pin;
        }
    }
}
