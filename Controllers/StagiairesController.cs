using Microsoft.AspNetCore.Mvc;

namespace GestionStagiaires.Controllers
{
    public class StagiairesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}