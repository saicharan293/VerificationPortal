using VerificationPortal.Models;

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
    }
}
