namespace VerificationPortal.Models.ViewModels
{
    public class HomeViewModel
    {
        public int TotalColleges { get; set; }
        public int VerifiedColleges { get; set; }
        public int PendingColleges { get; set; }
        public int TotalFaculties { get; set; }
        public int TotalDistricts { get; set; }
        public List<RecentActivity> RecentVerifications { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }

    public class RecentActivity
    {
        public string CollegeName { get; set; }
        public string Faculty { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string VerifiedBy { get; set; }
    }

    public class Notification
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }

    public class Faculty
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int CollegeCount { get; set; }
        public string Color { get; set; }
    }
}
