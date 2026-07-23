using GestionStagiaires.Models;
using Microsoft.AspNetCore.Mvc;
using GestionStagiaires.Data;
using Microsoft.AspNetCore.Authorization;
namespace GestionStagiaires.Controllers
{
    [Authorize]
    public class StagiairesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StagiairesController(ApplicationDbContext context)
        {
            _context = context;
        }

      public IActionResult Index(
    string? recherche,
    string? tri,
    int page = 1)
    {
        int taillePage = 5;

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

        int nombreTotal = stagiaires.Count();

        List<Stagiaire> liste = stagiaires
            .Skip((page - 1) * taillePage)
            .Take(taillePage)
            .ToList();

        ViewBag.Recherche = recherche;
        ViewBag.Tri = tri;
        ViewBag.PageActuelle = page;
        ViewBag.NombrePages =
            (int)Math.Ceiling(nombreTotal / (double)taillePage);

        return View(liste);
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

            TempData["Succes"] = "Le stagiaire a été ajouté avec succès.";

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

            TempData["Succes"] = "Le stagiaire a été modifié avec succès.";

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

                TempData["Succes"] = "Le stagiaire a été supprimé avec succès.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Stagiaire? stagiaire = _context.Stagiaires.Find(id);

            if (stagiaire == null)
            {
                return NotFound();
            }

            return View(stagiaire);
        }
    }
}
