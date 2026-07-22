using System.ComponentModel.DataAnnotations;

namespace GestionStagiaires.Models
{
    public class Stagiaire
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string? Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string? Prenom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide")]
        public string? Email { get; set; }
    }
}