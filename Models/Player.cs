namespace HellOfQuiz.Models
{
    public class Player
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public bool HasAnsweredCurrentQuestion { get; set; } = false;
        public int ConsecutiveCorrectAnswers { get; set; } = 0;
    }
}
