using GestionStagiaires.Models;
using Microsoft.AspNetCore.Mvc;
using GestionStagiaires.Data;
using Microsoft.AspNetCore.Authorization;
namespace GestionStagiaires.Controllers;
using GestionStagiaires.ViewModels;
using Microsoft.AspNetCore.Identity;


    [Authorize(Roles = "Responsable")]
    public class StagiairesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public StagiairesController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                s.Email!.Contains(recherche)||
                s.Tuteur!.Contains(recherche));
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
            return View(new CreateStagiaireViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateStagiaireViewModel model)
        {
            if (model.DateDebut.HasValue &&
                model.DateFin.HasValue &&
                model.DateFin.Value < model.DateDebut.Value)
            {
                ModelState.AddModelError(
                    nameof(model.DateFin),
                    "La date de fin doit être postérieure à la date de début."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var utilisateurExistant =
                await _userManager.FindByEmailAsync(model.Email!);

            if (utilisateurExistant != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Un compte existe déjà avec cette adresse email."
                );

                return View(model);
            }

            var utilisateur = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var resultatCreation =
                await _userManager.CreateAsync(
                    utilisateur,
                    model.MotDePasse!
                );

            if (!resultatCreation.Succeeded)
            {
                foreach (var erreur in resultatCreation.Errors)
                {
                    ModelState.AddModelError(
                        nameof(model.MotDePasse),
                        erreur.Description
                    );
                }

                return View(model);
            }

            var resultatRole =
                await _userManager.AddToRoleAsync(
                    utilisateur,
                    "Stagiaire"
                );

            if (!resultatRole.Succeeded)
            {
                await _userManager.DeleteAsync(utilisateur);

                foreach (var erreur in resultatRole.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        erreur.Description
                    );
                }

                return View(model);
            }

            var stagiaire = new Stagiaire
            {
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                Tuteur = model.Tuteur,
                DateDebut = model.DateDebut,
                DateFin = model.DateFin,
                UserId = utilisateur.Id
            };

            try
            {
                _context.Stagiaires.Add(stagiaire);
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(utilisateur);

                ModelState.AddModelError(
                    string.Empty,
                    "Une erreur est survenue pendant l’enregistrement du stagiaire."
                );

                return View(model);
            }

            TempData["Succes"] =
                "Le stagiaire et son compte ont été créés avec succès.";

            return RedirectToAction(nameof(Index));
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
            if (stagiaire.DateDebut.HasValue &&
            stagiaire.DateFin.HasValue &&
            stagiaire.DateFin < stagiaire.DateDebut)
            {
                ModelState.AddModelError(
                    "DateFin",
                    "La date de fin doit être postérieure à la date de début."
                );
            }
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

