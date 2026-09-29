
namespace VerificationPortal.Models
{
    public class DashboardViewModel
    {
        public TblRguhsFacultyUser User { get; set; }
        public Faculty Faculty { get; set; }
        public TblCollegeMapping CollegeMapping { get; set; }
        public int TotalAssignedColleges { get; set; }
        public string Role { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSection { get; set; }
        public bool ShowAffiliationSelector { get; set; }
        public int TotalVerified { get; set; }

        public int TotalPending { get; set; }

        public DateTime CurrentDate { get; set; }

        public string Greeting => CurrentDate.Hour < 12 ? "Good Morning" : CurrentDate.Hour < 17 ? "Good Afternoon" : "Good Evening";

        public List<DashboardResourceViewModel> Resources { get; set; } = new();

    }

    public class DashboardResourceViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Url { get; set; } = "#";
    }

}
