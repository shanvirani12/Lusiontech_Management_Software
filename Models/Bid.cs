using System.ComponentModel.DataAnnotations;

namespace Lusiontech_Management_Software.Models
{
    public class Bid
    {
        [Key]
        public int Id { get; set; }
        public int SerialNumber { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
    }

}
