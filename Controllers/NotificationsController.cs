using GestionStagiaires.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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
            var notifications = await _context.Notifications
                .Where(n => n.UserId == utilisateur.Id)
                .OrderByDescending(n => n.DateCreation)
                .ToListAsync();
            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarquerCommeLue(int id)
        {
            var utilisateur = await _userManager.GetUserAsync(User);
            if (utilisateur == null)
            {
                return Challenge();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == id &&
                    n.UserId == utilisateur.Id);

            if (notification == null)
            {
                return NotFound();
            }

            notification.EstLue = true;

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(notification.Lien))
            {
                return LocalRedirect(notification.Lien);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToutMarquerCommeLu()
        {
            var utilisateur = await _userManager.GetUserAsync(User);

            if (utilisateur == null)
            {
                return Challenge();
            }

            var notifications = await _context.Notifications
                .Where(n =>
                    n.UserId == utilisateur.Id &&
                    !n.EstLue)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.EstLue = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}