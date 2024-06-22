using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lusiontech_Management_Software.Data;
using Lusiontech_Management_Software.Models;
using Microsoft.AspNetCore.Authorization;

namespace Lusiontech_Management_Software.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Projects
        public async Task<IActionResult> Index(int? accountId, string userEmail)
        {
            var projectsQuery = _context.Projects
                .Include(p => p.Bid)
                    .ThenInclude(b => b.Account)
                .Include(p => p.Bid)
                    .ThenInclude(b => b.User)
                .AsQueryable();

            if (accountId.HasValue)
            {
                projectsQuery = projectsQuery.Where(p => p.Bid.AccountId == accountId.Value);
            }

            if (!string.IsNullOrEmpty(userEmail))
            {
                projectsQuery = projectsQuery.Where(p => p.Bid.User.Email == userEmail);
            }

            var projects = await projectsQuery.ToListAsync();

            var accounts = await _context.Accounts.ToListAsync();
            var users = await _context.Users.ToListAsync();

            ViewData["Accounts"] = new SelectList(accounts, "Id", "Name");
            ViewData["Users"] = new SelectList(users, "Email", "Email");

            return View(projects);
        }

        // GET: Projects/SearchBids
        public async Task<IActionResult> SearchBids(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { });
            }

            var bids = await _context.Bids
                .Include(b => b.User)
                .Include(b => b.Account)
                .Where(b => b.Link.Contains(query))
                .Select(b => new
                {
                    id = b.Id,
                    link = b.Link,
                    user = b.User.UserName,
                    account = b.Account.Name
                })
                .ToListAsync();

            return Json(bids);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Bid)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["BidId"] = new SelectList(_context.Bids, "Id", "Link");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,BidId")] Project project)
        {    
            _context.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
                
        }


        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Populate the SelectList for BidId
            ViewData["BidId"] = new SelectList(_context.Bids, "Id", "Link", project.BidId);

            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BidId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            try
            {
                // Check if BidId is valid (exists in Bids table)
                var bid = await _context.Bids.FindAsync(project.BidId);
                if (bid == null)
                {
                    ModelState.AddModelError("BidId", "Invalid BidId selected.");
                    ViewData["BidId"] = new SelectList(_context.Bids, "Id", "Link", project.BidId);
                    return View(project);
                }

                // Update the project
                _context.Update(project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw; // Optionally rethrow the exception for further investigation
            }
        }


        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Bid)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
