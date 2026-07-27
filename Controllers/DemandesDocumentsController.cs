using GestionStagiaires.Data;
using GestionStagiaires.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionStagiaires.ViewModels;

namespace GestionStagiaires.Controllers
{
    [Authorize]
    public class DemandesDocumentsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DemandesDocumentsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
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
        [Authorize(Roles = "Responsable")]
        [HttpGet]
        public async Task<IActionResult> Transmettre(int id)
        {
            var demande = await _context.DemandesDocuments
                .Include(d => d.Stagiaire)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null)
            {
                return NotFound();
            }

            if (demande.Statut != "En attente")
            {
                TempData["Erreur"] = "Cette demande a déjà été traitée.";

                return RedirectToAction(nameof(Gestion));
            }

            ViewBag.Demande = demande;

            return View(new TraiterDemandeViewModel
            {
                DemandeId = demande.Id
            });
        }
        [Authorize(Roles = "Responsable")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transmettre(
            TraiterDemandeViewModel model)
        {
            var demande = await _context.DemandesDocuments
                .Include(d => d.Stagiaire)
                .FirstOrDefaultAsync(d => d.Id == model.DemandeId);

            if (demande == null)
            {
                return NotFound();
            }

            if (demande.Statut != "En attente")
            {
                TempData["Erreur"] = "Cette demande a déjà été traitée.";

                return RedirectToAction(nameof(Gestion));
            }

            ViewBag.Demande = demande;

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
                Path.GetExtension(model.Fichier.FileName)
                    .ToLowerInvariant();

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
                StagiaireId = demande.StagiaireId,
                NomDocument = demande.TypeDocument,
                NomFichier = Path.GetFileName(
                    model.Fichier.FileName
                ),
                CheminFichier =
                    $"/uploads/documents/{nomFichierStocke}",
                DateDepot = DateTime.Now,
                ResponsableId = responsableId
            };

            _context.DocumentsStagiaires.Add(document);

            await _context.SaveChangesAsync();

            demande.Statut = "Acceptée";
            demande.DateTraitement = DateTime.Now;
            demande.DocumentStagiaireId = document.Id;

            if (!string.IsNullOrWhiteSpace(
                demande.Stagiaire?.UserId))
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = demande.Stagiaire.UserId,
                    Titre = "Votre document est disponible",
                    Message =
                        $"Votre demande de « {demande.TypeDocument} » " +
                        "a été traitée. Le document est maintenant disponible.",
                    DateCreation = DateTime.Now,
                    EstLue = false,
                    Lien = Url.Action(
                        "MesDocuments",
                        "DocumentsStagiaires"
                    )
                });
            }

            await _context.SaveChangesAsync();

            TempData["Succes"] =
                "Le document a été transmis au stagiaire.";

            return RedirectToAction(nameof(Gestion));
        }

        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Details(int id)
        {
            var demande = await _context.DemandesDocuments
                .Include(d => d.Stagiaire)
                .Include(d => d.DocumentStagiaire)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (demande == null)
            {
                return NotFound();
            }

            return View(demande);
        }

        [Authorize(Roles = "Stagiaire")]
        public async Task<IActionResult> DetailsStagiaire(int id)
        {
            string? userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var demande = await _context.DemandesDocuments
                .Include(d => d.Stagiaire)
                .Include(d => d.DocumentStagiaire)
                .FirstOrDefaultAsync(d =>
                    d.Id == id &&
                    d.Stagiaire != null &&
                    d.Stagiaire.UserId == userId);

            if (demande == null)
            {
                return NotFound();
            }

            return View(demande);
        }
    }
}