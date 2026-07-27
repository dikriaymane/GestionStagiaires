using GestionStagiaires.Models;

namespace GestionStagiaires.ViewModels;

public class StagiairePaginationResult
{
    public List<Stagiaire> Stagiaires { get; set; } = [];

    public int NombreTotal { get; set; }

    public int NombrePages { get; set; }
}