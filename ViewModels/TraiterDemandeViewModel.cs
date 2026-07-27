using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionStagiaires.ViewModels
{
    public class TraiterDemandeViewModel
    {
        [Required]
        public int DemandeId { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un fichier.")]
        [Display(Name = "Document à transmettre")]
        public IFormFile? Fichier { get; set; }
    }
}