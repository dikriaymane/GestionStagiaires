using GestionStagiaires.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Controllers
{
    [Authorize(Roles = "Stagiaire")]
    public class EspaceStagiaireController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EspaceStagiaireController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

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
                return View("AucuneFiche");
            }

            return View(stagiaire);
        }
    }
}