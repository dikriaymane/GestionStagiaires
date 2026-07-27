using GestionStagiaires.Data;
using GestionStagiaires.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Controllers
{
    [Authorize]
    public class DemandesDocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DemandesDocumentsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Stagiaire")]
        public async Task<IActionResult> Index()
        {
            var utilisateur = await _userManager.GetUserAsync(User);

            if (utilisateur == null)
            {
                return Challenge();
            }

            var stagiaire = await _context.Stagiaires
                .FirstOrDefaultAsync(s => s.UserId == utilisateur.Id);

            if (stagiaire == null)
            {
                return NotFound();
            }

            var demandes = await _context.DemandesDocuments
                .Where(d => d.StagiaireId == stagiaire.Id)
                .OrderByDescending(d => d.DateDemande)
                .ToListAsync();

            return View(demandes);
        }

        [Authorize(Roles = "Stagiaire")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new DemandeDocument());
        }

        [Authorize(Roles = "Stagiaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DemandeDocument demande)
        {
            var utilisateur = await _userManager.GetUserAsync(User);

            if (utilisateur == null)
            {
                return Challenge();
            }

            var stagiaire = await _context.Stagiaires
                .FirstOrDefaultAsync(s => s.UserId == utilisateur.Id);

            if (stagiaire == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(DemandeDocument.StagiaireId));
            ModelState.Remove(nameof(DemandeDocument.Stagiaire));

            if (!ModelState.IsValid)
            {
                return View(demande);
            }

            demande.StagiaireId = stagiaire.Id;
            demande.DateDemande = DateTime.Now;
            demande.Statut = "En attente";

            _context.DemandesDocuments.Add(demande);
            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "Votre demande a été envoyée avec succès.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Gestion()
        {
            var demandes = await _context.DemandesDocuments
                .Include(d => d.Stagiaire)
                .OrderByDescending(d => d.DateDemande)
                .ToListAsync();

            return View(demandes);
        }

        [Authorize(Roles = "Responsable")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accepter(int id)
        {
            var demande = await _context.DemandesDocuments
                .FindAsync(id);

            if (demande == null)
            {
                return NotFound();
            }

            demande.Statut = "Acceptée";

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "La demande a été acceptée.";

            return RedirectToAction(nameof(Gestion));
        }

        [Authorize(Roles = "Responsable")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refuser(int id)
        {
            var demande = await _context.DemandesDocuments
                .FindAsync(id);

            if (demande == null)
            {
                return NotFound();
            }

            demande.Statut = "Refusée";

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "La demande a été refusée.";

            return RedirectToAction(nameof(Gestion));
        }
    }
}