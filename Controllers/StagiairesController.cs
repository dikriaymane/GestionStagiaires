using GestionStagiaires.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionStagiaires.Controllers
{
    public class StagiairesController : Controller
    {
        public IActionResult Index()
        {
            List<Stagiaire> stagiaires = new List<Stagiaire>
            {
                new Stagiaire
                {
                    Id = 1,
                    Nom = "Dikri",
                    Prenom = "Aymane",
                    Email = "aymane@email.com"
                },

                new Stagiaire
                {
                    Id = 2,
                    Nom = "Alami",
                    Prenom = "Sara",
                    Email = "sara@email.com"
                }
            };

            return View(stagiaires);
        }
    }
}