using GestionStagiaires.Data;
using GestionStagiaires.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Controllers
{
    [Authorize(Roles = "Stagiaire")]
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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new DemandeDocument());
        }

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
    }
}