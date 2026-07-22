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
            List<Stagiaire> stagiaires = _context.Stagiaires.ToList();
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
