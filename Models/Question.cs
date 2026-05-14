using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HellOfQuiz.Models
{
    /// <summary>
    /// Quiz sorularını temsil eden model sınıfı.
    /// Her sorunun 4 şıkkı vardır, biri doğru cevaptır.
    /// </summary>
    public class Question
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Soru metni zorunludur.")]
        [StringLength(500)]
        public string Text { get; set; } = string.Empty;

        // 4 şık
        [Required(ErrorMessage = "A şıkkı zorunludur.")]
        [StringLength(200)]
        public string OptionA { get; set; } = string.Empty;

        [Required(ErrorMessage = "B şıkkı zorunludur.")]
        [StringLength(200)]
        public string OptionB { get; set; } = string.Empty;

        [Required(ErrorMessage = "C şıkkı zorunludur.")]
        [StringLength(200)]
        public string OptionC { get; set; } = string.Empty;

        [Required(ErrorMessage = "D şıkkı zorunludur.")]
        [StringLength(200)]
        public string OptionD { get; set; } = string.Empty;

        // Doğru cevap: "A", "B", "C" veya "D"
        [Required]
        [StringLength(1)]
        public string CorrectAnswer { get; set; } = "A";

        // Sorunun zorluğu: 1=Kolay, 2=Orta, 3=Zor
        public int Difficulty { get; set; } = 1;

        // İlişki: Foreign Key - hangi kategoriye ait olduğu
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
