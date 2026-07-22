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

       public IActionResult Index(string? recherche, string? tri)
        {
            var stagiaires = _context.Stagiaires.AsQueryable();

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                stagiaires = stagiaires.Where(s =>
                    s.Nom!.Contains(recherche) ||
                    s.Prenom!.Contains(recherche) ||
                    s.Email!.Contains(recherche));
            }

            stagiaires = tri switch
            {
                "nom" => stagiaires.OrderBy(s => s.Nom),
                "prenom" => stagiaires.OrderBy(s => s.Prenom),
                "email" => stagiaires.OrderBy(s => s.Email),
                "id_desc" => stagiaires.OrderByDescending(s => s.Id),
                _ => stagiaires.OrderBy(s => s.Id)
            };

            ViewBag.Recherche = recherche;
            ViewBag.Tri = tri;

            return View(stagiaires.ToList());
        }
                [HttpGet]
                public IActionResult Create()
        {
            return View();
        }

       [HttpPost]
        public IActionResult Create(Stagiaire stagiaire)
        {
            if (!ModelState.IsValid)
            {
                return View(stagiaire);
            }

            _context.Stagiaires.Add(stagiaire);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Stagiaire? stagiaire = _context.Stagiaires.Find(id);

            if (stagiaire == null)
            {
                return NotFound();
            }

            return View(stagiaire);
        }
        [HttpPost]
        public IActionResult Edit(Stagiaire stagiaire)
        {
            if (!ModelState.IsValid)
            {
                return View(stagiaire);
            }

            _context.Stagiaires.Update(stagiaire);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Stagiaire? stagiaire = _context.Stagiaires.Find(id);

            if (stagiaire == null)
            {
                return NotFound();
            }

            return View(stagiaire);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Stagiaire? stagiaire = _context.Stagiaires.Find(id);

            if (stagiaire != null)
            {
                _context.Stagiaires.Remove(stagiaire);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
