using GestionStagiaires.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStagiaires.ViewComponents
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationBadgeViewComponent(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? userId =
                _userManager.GetUserId(UserClaimsPrincipal);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return View(0);
            }

            int nombreNotifications = await _context.Notifications
                .CountAsync(n =>
                    n.UserId == userId &&
                    !n.EstLue);

            return View(nombreNotifications);
        }
    }
}