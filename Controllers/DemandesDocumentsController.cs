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
            var responsables = await _userManager
                .GetUsersInRoleAsync("Responsable");

            foreach (var responsable in responsables)
            {
                var notification = new Notification
                {
                    UserId = responsable.Id,
                    Titre = "Nouvelle demande de document",
                    Message =
                        $"{stagiaire.Prenom} {stagiaire.Nom} a demandé : " +
                        $"{demande.TypeDocument}.",
                    DateCreation = DateTime.Now,
                    EstLue = false,
                    Lien = Url.Action(
                        "Gestion",
                        "DemandesDocuments"
                    )
                };

                _context.Notifications.Add(notification);
            }

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
                .Include(d => d.Stagiaire)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null)
            {
                return NotFound();
            }

            demande.Statut = "Acceptée";

            if (!string.IsNullOrWhiteSpace(demande.Stagiaire?.UserId))
            {
                var notification = new Notification
                {
                    UserId = demande.Stagiaire.UserId,
                    Titre = "Demande acceptée",
                    Message =
                        $"Votre demande « {demande.TypeDocument} » a été acceptée.",
                    DateCreation = DateTime.Now,
                    EstLue = false,
                    Lien = Url.Action(
                        "Index",
                        "DemandesDocuments"
                    )
                };

                _context.Notifications.Add(notification);
            }

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
                .Include(d => d.Stagiaire)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null)
            {
                return NotFound();
            }

            demande.Statut = "Refusée";

            if (!string.IsNullOrWhiteSpace(demande.Stagiaire?.UserId))
            {
                var notification = new Notification
                {
                    UserId = demande.Stagiaire.UserId,
                    Titre = "Demande refusée",
                    Message =
                        $"Votre demande « {demande.TypeDocument} » a été refusée.",
                    DateCreation = DateTime.Now,
                    EstLue = false,
                    Lien = Url.Action(
                        "Index",
                        "DemandesDocuments"
                    )
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "La demande a été refusée.";

            return RedirectToAction(nameof(Gestion));
        }
    }
}