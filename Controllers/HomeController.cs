using Lusiontech_Management_Software.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Lusiontech_Management_Software.Data;
using Lusiontech_Management_Software.ViewModels;


namespace Lusiontech_Management_Software.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Total number of projects
            var totalProjects = await _context.Projects.CountAsync();

            // Bid count per user
            var bidCountPerUser = await _context.Bids
                .GroupBy(b => b.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    UserEmail = g.FirstOrDefault().User.Email,
                    BidCount = g.Count()
                })
                .ToListAsync();

            // Bid count per account
            var bidCountPerAccount = await _context.Bids
                .GroupBy(b => b.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    AccountName = g.FirstOrDefault().Account.Name,
                    BidCount = g.Count()
                })
                .ToListAsync();

            ViewBag.TotalProjects = totalProjects;
            ViewBag.BidCountPerUser = bidCountPerUser;
            ViewBag.BidCountPerAccount = bidCountPerAccount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
