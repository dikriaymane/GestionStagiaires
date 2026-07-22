using GestionStagiaires.Models;
using Microsoft.AspNetCore.Mvc;
using GestionStagiaires.Data;

namespace GestionStagiaires.Controllers
{
    public class StagiairesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StagiairesController(ApplicationDbContext context)
        {
            _context = context;
        }

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
                [HttpGet]
                public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Stagiaire stagiaire)
        {
            _context.Stagiaires.Add(stagiaire);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
