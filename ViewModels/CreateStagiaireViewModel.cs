using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.ViewModels
{
    public class CreateStagiaireViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string? Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string? Prenom { get; set; }

        [Required(ErrorMessage = "L’adresse email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L’adresse email n’est pas valide.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Le service est obligatoire.")]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Le tuteur est obligatoire.")]
        public string? Tuteur { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de début")]
        public DateTime? DateDebut { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        public DateTime? DateFin { get; set; }

        [Required(ErrorMessage = "Le mot de passe temporaire est obligatoire.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe temporaire")]
        public string? MotDePasse { get; set; }

        [Required(ErrorMessage = "Veuillez confirmer le mot de passe.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(MotDePasse),
            ErrorMessage = "Les deux mots de passe ne correspondent pas."
        )]
        [Display(Name = "Confirmation du mot de passe")]
        public string? ConfirmationMotDePasse { get; set; }
    }
}