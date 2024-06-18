using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Lusiontech_Management_Software.Models;
using Lusiontech_Management_Software.Data;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public IActionResult Index()
        {
            DateTime today = DateTime.Today;
            string userId = _userManager.GetUserId(User); // Get current logged-in user Id

            // Check if there is an existing table (BidTable) for today
            Bid todayTable = _context.Bids.FirstOrDefault(t => t.TableDate == today);


            // Retrieve bids for the current user and today's table
            List<Bid> bids = _context.Bids
                .Where(b => b.TableDate == today && b.UserId == userId)
                .ToList();

            return View(bids);
        }

        [HttpPost]
        public IActionResult AddBid(string link)
        {
            if (!string.IsNullOrWhiteSpace(link))
            {
                DateTime today = DateTime.Today;
                string userId = _userManager.GetUserId(User); // Get current logged-in user Id

                // Check if there is an existing table (BidTable) for today
                Bid todayTable = _context.Bids.FirstOrDefault(t => t.TableDate == today);

                // If no table exists for today, create a new one
                

                if (string.IsNullOrEmpty(link))
                {
                    // Handle validation error
                    ModelState.AddModelError("Link", "Link is required");
                    return BadRequest(ModelState);
                }

                // Create a new Bid object
                Bid newBid = new Bid
                {
                    SerialNumber = _context.Bids.Count(b => b.TableDate == today && b.UserId == userId) + 1,
                    Date = DateTime.Now,
                    Link = link,
                    TableDate = today,
                    UserId = userId
                };

                // Add the new bid to the database
                _context.Bids.Add(newBid);
                _context.SaveChanges();

                // Retrieve all bids for the current user and today after adding the new one
                List<Bid> bids = _context.Bids
                    .Where(b => b.TableDate == today && b.UserId == userId)
                    .ToList();

                // Return the updated table view as a partial view
                return PartialView("_BidsTable", bids);
            }

            // If link is empty or null, return empty partial view
            return PartialView("_BidsTable", new List<Bid>());
        }

        [HttpPost]
        public IActionResult EditBid(int id, string link)
        {
            if (!string.IsNullOrWhiteSpace(link))
            {
                Bid bid = _context.Bids.FirstOrDefault(b => b.Id == id);
                if (bid != null)
                {
                    bid.Link = link;
                    _context.SaveChanges();

                    DateTime today = DateTime.Today;
                    string userId = _userManager.GetUserId(User);

                    List<Bid> bids = _context.Bids
                        .Where(b => b.TableDate == today && b.UserId == userId)
                        .ToList();

                    return PartialView("_BidsTable", bids);
                }
            }

            return BadRequest("Invalid data.");
        }
    }
}
