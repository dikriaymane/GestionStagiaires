using GestionStagiaires.Models;
using GestionStagiaires.ViewModels;

namespace GestionStagiaires.Services;

public interface IStagiaireService
{
    Task<Stagiaire?> GetByIdAsync(int id);

    Task<Stagiaire?> GetByUserIdAsync(string userId);

    Task<bool> EmailExistsAsync(string email);

    Task<bool> CreateAsync(CreateStagiaireViewModel model);

    Task<bool> UpdateAsync(Stagiaire stagiaire);

    Task<bool> DeleteAsync(int id);

    Task<StagiairePaginationResult> GetPaginatedAsync(
        string? recherche,
        string? tri,
        int page,
        int taillePage);

    Task<DashboardResponsableViewModel> GetDashboardAsync();
}