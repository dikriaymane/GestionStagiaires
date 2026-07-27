using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GestionStagiaires.Models
{
    public class Stagiaire
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string? Nom { get; set; }
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string? Prenom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide")]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de début")]
        public DateTime? DateDebut { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        public DateTime? DateFin { get; set; }

        [Required(ErrorMessage = "Le service est obligatoire.")]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Le nom du tuteur est obligatoire")]
        [Display(Name = "Tuteur")]
        public string? Tuteur { get; set; }

        public string? EmailTuteur { get; set; }

        public string? TelephoneTuteur { get; set; }

        public string? BureauTuteur { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateDebut.HasValue && DateFin.HasValue && DateFin.Value < DateDebut.Value)
            {
                yield return new ValidationResult("La date de fin doit être postérieure à la date de début.",new[] { nameof(DateFin) });
            }
        }
    }
}