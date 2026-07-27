using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Titre { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool EstLue { get; set; } = false;

        public string? Lien { get; set; }
    }
}