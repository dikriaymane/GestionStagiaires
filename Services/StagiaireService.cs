using GestionStagiaires.Data;
using GestionStagiaires.Models;
using GestionStagiaires.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GestionStagiaires.Services;

public class StagiaireService : IStagiaireService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public StagiaireService(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Stagiaire?> GetByIdAsync(int id)
    {
        return await _context.Stagiaires
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Stagiaire?> GetByUserIdAsync(string userId)
    {
        return await _context.Stagiaires
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Stagiaires
            .AnyAsync(s => s.Email == email);
    }

    public async Task<bool> CreateAsync(
    CreateStagiaireViewModel model)
    {
        var utilisateur = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var resultatCreation = await _userManager.CreateAsync(
            utilisateur,
            model.MotDePasse);

        if (!resultatCreation.Succeeded)
        {
            return false;
        }

        var resultatRole = await _userManager.AddToRoleAsync(
            utilisateur,
            "Stagiaire");

        if (!resultatRole.Succeeded)
        {
            await _userManager.DeleteAsync(utilisateur);
            return false;
        }

        var stagiaire = new Stagiaire
        {
            Nom = model.Nom,
            Prenom = model.Prenom,
            Email = model.Email,
            Tuteur = model.Tuteur,
            EmailTuteur = model.EmailTuteur,
            TelephoneTuteur = model.TelephoneTuteur,
            BureauTuteur = model.BureauTuteur,
            Service = model.Service,
            DateDebut = model.DateDebut,
            DateFin = model.DateFin,
            UserId = utilisateur.Id
        };

        _context.Stagiaires.Add(stagiaire);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> UpdateAsync(Stagiaire stagiaire)
    {
        var stagiaireExistant =
            await _context.Stagiaires
                .FirstOrDefaultAsync(s => s.Id == stagiaire.Id);

        if (stagiaireExistant == null)
        {
            return false;
        }

        stagiaireExistant.Nom = stagiaire.Nom;
        stagiaireExistant.Prenom = stagiaire.Prenom;
        stagiaireExistant.Email = stagiaire.Email;
        stagiaireExistant.Tuteur = stagiaire.Tuteur;
        stagiaireExistant.EmailTuteur = stagiaire.EmailTuteur;
        stagiaireExistant.TelephoneTuteur = stagiaire.TelephoneTuteur;
        stagiaireExistant.BureauTuteur = stagiaire.BureauTuteur;
        stagiaireExistant.Service = stagiaire.Service;
        stagiaireExistant.DateDebut = stagiaire.DateDebut;
        stagiaireExistant.DateFin = stagiaire.DateFin;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var stagiaire =
            await _context.Stagiaires
                .FirstOrDefaultAsync(s => s.Id == id);

        if (stagiaire == null)
        {
            return false;
        }

        _context.Stagiaires.Remove(stagiaire);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<StagiairePaginationResult>
        GetPaginatedAsync(
            string? recherche,
            string? tri,
            int page,
            int taillePage)
    {
        IQueryable<Stagiaire> requete =
            _context.Stagiaires.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            recherche = recherche.Trim();

            requete = requete.Where(s =>
                (s.Nom != null &&
                 s.Nom.Contains(recherche)) ||

                (s.Prenom != null &&
                 s.Prenom.Contains(recherche)) ||

                (s.Email != null &&
                 s.Email.Contains(recherche)) ||

                (s.Tuteur != null &&
                 s.Tuteur.Contains(recherche)) ||

                (s.Service != null &&
                 s.Service.Contains(recherche)));
        }

        requete = tri switch
        {
            "nom" =>
                requete.OrderBy(s => s.Nom),

            "prenom" =>
                requete.OrderBy(s => s.Prenom),

            "email" =>
                requete.OrderBy(s => s.Email),

            "id_desc" =>
                requete.OrderByDescending(s => s.Id),

            _ =>
                requete.OrderBy(s => s.Id)
        };

        int nombreTotal =
            await requete.CountAsync();

        int nombrePages =
            (int)Math.Ceiling(
                nombreTotal / (double)taillePage);

        var stagiaires = await requete
            .Skip((page - 1) * taillePage)
            .Take(taillePage)
            .ToListAsync();

        return new StagiairePaginationResult
        {
            Stagiaires = stagiaires,
            NombreTotal = nombreTotal,
            NombrePages = nombrePages
        };
    }

    public async Task<DashboardResponsableViewModel>
        GetDashboardAsync()
    {
        DateTime aujourdHui = DateTime.Today;

        return new DashboardResponsableViewModel
        {
            TotalStagiaires =
                await _context.Stagiaires.CountAsync(),

            StagesEnCours =
                await _context.Stagiaires.CountAsync(s =>
                    s.DateDebut.HasValue &&
                    s.DateFin.HasValue &&
                    s.DateDebut.Value <= aujourdHui &&
                    s.DateFin.Value >= aujourdHui),

            StagesTermines =
                await _context.Stagiaires.CountAsync(s =>
                    s.DateFin.HasValue &&
                    s.DateFin.Value < aujourdHui),

            StagesAVenir =
                await _context.Stagiaires.CountAsync(s =>
                    s.DateDebut.HasValue &&
                    s.DateDebut.Value > aujourdHui),

            DemandesEnAttente =
                await _context.DemandesDocuments.CountAsync(d =>
                    d.Statut == "En attente")
        };
    }
}