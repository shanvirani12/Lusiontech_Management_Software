using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Lusiontech_Management_Software.Models;
using Lusiontech_Management_Software.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Lusiontech_Management_Software.Controllers
{
    [Authorize] // Restrict access to authenticated users only
    public class BidsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public BidsController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Display the index page with bids for today
        public IActionResult Index()
        {
            DateTime today = DateTime.Today;
            string userId = _userManager.GetUserId(User); // Get current logged-in user Id

            // Retrieve bids for the current user, selected account, and today's date
            int? accountId = HttpContext.Session.GetInt32("SelectedAccountId");
            if (!accountId.HasValue)
            {
                // Handle case where accountId is not set (redirect or display error)
                return RedirectToAction("SelectAccount");
            }

            List<Bid> bids = _context.Bids
                .Where(b => b.TableDate == today && b.UserId == userId && b.AccountId == accountId.Value)
                .Include(b => b.Account) // Include account details
                .ToList();

            return View(bids);
        }

        // GET: Display the page to select an account for bidding
        public IActionResult SelectAccount()
        {
            // Retrieve accounts for the current user
            List<Account> accounts = _context.Accounts.ToList();

            return View(accounts);
        }

        // POST: Process account selection and redirect to bid entry page
        [HttpPost]
        public IActionResult StartBidding(int accountId)
        {
            // Store the selected account ID in session
            HttpContext.Session.SetInt32("SelectedAccountId", accountId);

            // Redirect to the bid entry page
            return RedirectToAction("Index");
        }

        // POST: Add a new bid for the selected account and current user
        [HttpPost]
        public IActionResult AddBid(string link)
        {
            if (!string.IsNullOrWhiteSpace(link))
            {
                DateTime today = DateTime.Today;
                string userId = _userManager.GetUserId(User); // Get current logged-in user Id

                // Retrieve the selected account ID from session
                int? accountId = HttpContext.Session.GetInt32("SelectedAccountId");
                if (!accountId.HasValue)
                {
                    return BadRequest("Selected account not found."); // Handle case where accountId is not set
                }

                // Create a new Bid object
                Bid newBid = new Bid
                {
                    SerialNumber = _context.Bids.Count(b => b.TableDate == today && b.UserId == userId && b.AccountId == accountId.Value) + 1,
                    Date = DateTime.Now,
                    Link = link,
                    TableDate = today,
                    UserId = userId,
                    AccountId = accountId.Value
                };

                // Add the new bid to the database
                _context.Bids.Add(newBid);
                _context.SaveChanges();

                // Retrieve all bids for the current user, selected account, and today after adding the new one
                List<Bid> bids = _context.Bids
                    .Where(b => b.TableDate == today && b.UserId == userId && b.AccountId == accountId.Value)
                    .ToList();

                // Return the updated table view as a partial view
                return PartialView("_BidsTable", bids);
            }

            // If link is empty or null, return empty partial view
            return PartialView("_BidsTable", new List<Bid>());
        }

        // POST: Update the link for an existing bid
        [HttpPost]
        public IActionResult EditBid(int id, string link)
        {
            if (!string.IsNullOrWhiteSpace(link))
            {
                var bid = _context.Bids.Find(id);
                if (bid != null)
                {
                    bid.Link = link;
                    _context.SaveChanges();

                    // Retrieve updated bids for the current user, selected account, and today's table
                    DateTime today = DateTime.Today;
                    string userId = _userManager.GetUserId(User);
                    int? accountId = HttpContext.Session.GetInt32("SelectedAccountId");
                    if (!accountId.HasValue)
                    {
                        return BadRequest("Selected account not found."); // Handle case where accountId is not set
                    }

                    List<Bid> bids = _context.Bids
                        .Where(b => b.TableDate == today && b.UserId == userId && b.AccountId == accountId.Value)
                        .Include(b => b.Account) // Include account details
                        .ToList();

                    return PartialView("_BidsTable", bids);
                }
            }

            // If link is empty or null, return BadRequest
            return BadRequest("Invalid data.");
        }
    }
}
