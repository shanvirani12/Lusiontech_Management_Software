namespace Lusiontech_Management_Software.ViewModels
{
    public class HomeIndexViewModel
    {
        public int TotalProjects { get; set; }
        public List<object> BidCountPerUser { get; set; } // Adjust type as per your User and Bid relationship
        public List<object> BidCountPerAccount { get; set; } // Adjust type as per your Account and Bid relationship
    }
}
