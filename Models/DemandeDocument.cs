using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.Models
{
    public class DemandeDocument
    {
        public int Id { get; set; }

        [Required]
        public int StagiaireId { get; set; }

        [Required(ErrorMessage = "Le type de document est obligatoire.")]
        [Display(Name = "Type de document")]
        public string TypeDocument { get; set; } = string.Empty;

        [Display(Name = "Message")]
        public string? Message { get; set; }

        [Display(Name = "Date de la demande")]
        public DateTime DateDemande { get; set; } = DateTime.Now;

        public string Statut { get; set; } = "En attente";

        public Stagiaire? Stagiaire { get; set; }
    }
}