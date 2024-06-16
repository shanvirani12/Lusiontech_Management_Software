using Lusiontech_Management_Software.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lusiontech_Management_Software.Controllers
{
    [Authorize]
    public class BidsController : Controller
    {
        private static List<Bid> bids = new List<Bid>();

        public ActionResult Index()
        {
            return View(bids);
        }

        [HttpPost]
        public ActionResult AddBid(string link)
        {
            var newBid = new Bid
            {
                SerialNumber = bids.Count + 1,
                Date = DateTime.Now,
                Link = link
            };

            bids.Add(newBid);

            return PartialView("_BidsTable", bids);
        }
    }

}
