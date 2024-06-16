using Lusiontech_Management_Software.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lusiontech_Management_Software.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee>
    {
        public DbSet<Bid> Bids { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {
        }
       
    }
}
