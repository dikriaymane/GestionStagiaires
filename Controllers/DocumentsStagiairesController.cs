using GestionStagiaires.Data;
using GestionStagiaires.Models;
using GestionStagiaires.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Controllers
{
    [Authorize]
    public class DocumentsStagiairesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public DocumentsStagiairesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [Authorize(Roles = "Responsable")]
        [HttpGet]
        public async Task<IActionResult> Depot(int stagiaireId)
        {
            var stagiaire = await _context.Stagiaires
                .FindAsync(stagiaireId);

            if (stagiaire == null)
            {
                return NotFound();
            }

            ViewBag.Stagiaire = stagiaire;

            return View(new DepotDocumentViewModel
            {
                StagiaireId = stagiaire.Id
            });
        }

        [Authorize(Roles = "Responsable")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Depot(
            DepotDocumentViewModel model)
        {
            var stagiaire = await _context.Stagiaires
                .FindAsync(model.StagiaireId);

            if (stagiaire == null)
            {
                return NotFound();
            }

            ViewBag.Stagiaire = stagiaire;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Fichier == null || model.Fichier.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(model.Fichier),
                    "Veuillez sélectionner un fichier."
                );

                return View(model);
            }

            // Limite : 5 Mo
            const long tailleMaximale = 5 * 1024 * 1024;

            if (model.Fichier.Length > tailleMaximale)
            {
                ModelState.AddModelError(
                    nameof(model.Fichier),
                    "Le fichier ne doit pas dépasser 5 Mo."
                );

                return View(model);
            }

            string extension =
                Path.GetExtension(model.Fichier.FileName).ToLowerInvariant();

            if (extension != ".pdf")
            {
                ModelState.AddModelError(
                    nameof(model.Fichier),
                    "Seuls les fichiers PDF sont autorisés."
                );

                return View(model);
            }

            string dossierUploads = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "documents"
            );

            Directory.CreateDirectory(dossierUploads);

            // Ne jamais utiliser directement le nom fourni par l'utilisateur.
            string nomFichierStocke =
                $"{Guid.NewGuid()}{extension}";

            string cheminPhysique = Path.Combine(
                dossierUploads,
                nomFichierStocke
            );

            await using (var stream = new FileStream(
                cheminPhysique,
                FileMode.Create))
            {
                await model.Fichier.CopyToAsync(stream);
            }

            string? responsableId =
                _userManager.GetUserId(User);

            var document = new DocumentStagiaire
            {
                StagiaireId = stagiaire.Id,
                NomDocument = model.NomDocument,
                NomFichier = Path.GetFileName(model.Fichier.FileName),
                CheminFichier =
                    $"/uploads/documents/{nomFichierStocke}",
                DateDepot = DateTime.Now,
                ResponsableId = responsableId
            };

            _context.DocumentsStagiaires.Add(document);

            if (!string.IsNullOrWhiteSpace(stagiaire.UserId))
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = stagiaire.UserId,
                    Titre = "Nouveau document disponible",
                    Message =
                        $"Le document « {model.NomDocument} » " +
                        "a été déposé dans votre espace.",
                    DateCreation = DateTime.Now,
                    EstLue = false,
                    Lien = Url.Action(
                        nameof(MesDocuments),
                        "DocumentsStagiaires"
                    )
                });
            }

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "Le document a été déposé avec succès.";

            return RedirectToAction(
                nameof(Liste),
                new { stagiaireId = stagiaire.Id }
            );
        }

        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Liste(int stagiaireId)
        {
            var stagiaire = await _context.Stagiaires
                .FindAsync(stagiaireId);

            if (stagiaire == null)
            {
                return NotFound();
            }

            var documents = await _context.DocumentsStagiaires
                .Where(d => d.StagiaireId == stagiaireId)
                .OrderByDescending(d => d.DateDepot)
                .ToListAsync();

            ViewBag.Stagiaire = stagiaire;

            return View(documents);
        }

        [Authorize(Roles = "Stagiaire")]
        public async Task<IActionResult> MesDocuments()
        {
            string? userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var stagiaire = await _context.Stagiaires
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (stagiaire == null)
            {
                return NotFound();
            }

            var documents = await _context.DocumentsStagiaires
                .Where(d => d.StagiaireId == stagiaire.Id)
                .OrderByDescending(d => d.DateDepot)
                .ToListAsync();

            return View(documents);
        }
    }
}