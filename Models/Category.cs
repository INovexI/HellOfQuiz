using System.ComponentModel.DataAnnotations;

namespace HellOfQuiz.Models
{
    /// <summary>
    /// Quiz kategorilerini temsil eden model sınıfı.
    /// Örnek: Tarih, Coğrafya, Yazılım, Spor
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Kategoriyi temsil eden emoji veya ikon kodu
        [StringLength(10)]
        public string Icon { get; set; } = "🎯";

        // Kategori arka plan rengi (CSS renk kodu)
        [StringLength(20)]
        public string Color { get; set; } = "#6C63FF";

        // İlişki: Bir kategoride birden fazla soru olabilir
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
