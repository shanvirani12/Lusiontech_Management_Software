using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Lusiontech_Management_Software.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public int SerialNumber { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Link is required")]
        public string Link { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TableDate { get; set; } // Date for the table creation

        // Foreign key for relationship with Employee
        public string UserId { get; set; }
        public Employee User { get; set; }

        [Required]
        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
    }
}
