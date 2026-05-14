using System.ComponentModel.DataAnnotations;

namespace HellOfQuiz.Models
{
    /// <summary>
    /// Uygulamadaki kullanıcıları temsil eden model sınıfı.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Kullanıcının admin olup olmadığını belirtir
        public bool IsAdmin { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
