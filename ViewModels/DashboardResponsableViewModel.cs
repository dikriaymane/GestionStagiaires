namespace GestionStagiaires.ViewModels
{
    public class DashboardResponsableViewModel
    {
        public int TotalStagiaires { get; set; }

        public int StagesEnCours { get; set; }

        public int StagesTermines { get; set; }

        public int StagesAVenir { get; set; }

        public int DemandesEnAttente { get; set; }
    }
}