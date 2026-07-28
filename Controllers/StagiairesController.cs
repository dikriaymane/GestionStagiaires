using GestionStagiaires.Models;
using GestionStagiaires.Services;
using GestionStagiaires.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionStagiaires.Controllers;

[Authorize(Roles = "Responsable")]
public class StagiairesController : Controller
{
    private readonly IStagiaireService _stagiaireService;

    public StagiairesController(IStagiaireService stagiaireService)
    {
        _stagiaireService = stagiaireService;
    }

    public async Task<IActionResult> Index(
        string? recherche,
        string? tri,
        int page = 1)
    {
        const int taillePage = 5;

        if (page < 1)
        {
            page = 1;
        }

        var resultat = await _stagiaireService.GetPaginatedAsync(
            recherche,
            tri,
            page,
            taillePage);

        ViewBag.Recherche = recherche;
        ViewBag.Tri = tri;
        ViewBag.PageActuelle = page;
        ViewBag.NombrePages = resultat.NombrePages;

        return View(resultat.Stagiaires);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStagiaireViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _stagiaireService.EmailExistsAsync(model.Email))
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Un stagiaire utilisant cette adresse email existe déjà.");

            return View(model);
        }

        var creationReussie = await _stagiaireService.CreateAsync(model);

        if (!creationReussie)
        {
            ModelState.AddModelError(
                "",
                "Impossible de créer le compte du stagiaire.");

            return View(model);
        }

        TempData["Succes"] =
            "Le stagiaire a été créé avec succès.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var stagiaire = await _stagiaireService.GetByIdAsync(id);

        if (stagiaire == null)
        {
            return NotFound();
        }

        return View(stagiaire);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Stagiaire stagiaire)
    {
        if (!ModelState.IsValid)
        {
            return View(stagiaire);
        }

        var modificationReussie =
            await _stagiaireService.UpdateAsync(stagiaire);

        if (!modificationReussie)
        {
            return NotFound();
        }

        TempData["Succes"] =
            "Le stagiaire a été modifié avec succès.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var stagiaire = await _stagiaireService.GetByIdAsync(id);

        if (stagiaire == null)
        {
            return NotFound();
        }

        return View(stagiaire);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var suppressionReussie =
            await _stagiaireService.DeleteAsync(id);

        if (!suppressionReussie)
        {
            return NotFound();
        }

        TempData["Succes"] =
            "Le stagiaire a été supprimé avec succès.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var stagiaire = await _stagiaireService.GetByIdAsync(id);

        if (stagiaire == null)
        {
            return NotFound();
        }

        return View(stagiaire);
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var model =
            await _stagiaireService.GetDashboardAsync();

        return View(model);
    }
}