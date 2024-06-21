using System.ComponentModel.DataAnnotations.Schema;

namespace Lusiontech_Management_Software.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BidId { get; set; }
        [ForeignKey("BidId")]
        public virtual Bid Bid { get; set; }
    }
}
