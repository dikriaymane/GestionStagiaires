using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.Models
{
    public class DocumentStagiaire
    {
        public int Id { get; set; }

        [Required]
        public int StagiaireId { get; set; }

        [Required(ErrorMessage = "Le nom du document est obligatoire.")]
        [Display(Name = "Nom du document")]
        public string NomDocument { get; set; } = string.Empty;

        [Required]
        public string NomFichier { get; set; } = string.Empty;

        [Required]
        public string CheminFichier { get; set; } = string.Empty;

        public DateTime DateDepot { get; set; } = DateTime.Now;

        public string? ResponsableId { get; set; }

        public Stagiaire? Stagiaire { get; set; }
    }
}