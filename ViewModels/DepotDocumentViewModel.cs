using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionStagiaires.ViewModels
{
    public class DepotDocumentViewModel
    {
        [Required]
        public int StagiaireId { get; set; }

        [Required(ErrorMessage = "Le nom du document est obligatoire.")]
        [Display(Name = "Nom du document")]
        public string NomDocument { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez sélectionner un fichier.")]
        [Display(Name = "Fichier PDF")]
        public IFormFile? Fichier { get; set; }
    }
}