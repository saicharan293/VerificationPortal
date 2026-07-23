using System.ComponentModel.DataAnnotations;
using VerificationPortal.Models;

namespace VerificationPortal.Models
{
    public class CollegeMappingListViewModel
    {
        public List<CollegeMappingWithUser> Mappings { get; set; } = new();
        public int TotalMappings { get; set; }
        public int ActiveMappings { get; set; }
        public int TotalUsers { get; set; }
    }

    public class CollegeMappingWithUser
    {
        public TblCollegeMapping Mapping { get; set; } = null!;
        public string? UserDesignation { get; set; }
        public string? FacultyName { get; set; }
    }

    public class CollegeMappingCreateViewModel
    {
        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "User")]
        public int SelectedUserId { get; set; }

        [Required]
        [Display(Name = "Faculty")]
        public int SelectedFacultyId { get; set; }

        [Required(ErrorMessage = "Please select at least one college")]
        [Display(Name = "Colleges")]
        public List<string> SelectedCollegeCodes { get; set; } = new();

        // Filter properties for alphabetical range filtering
        // Alphabetical Range
        [RegularExpression("^[A-Z]$", ErrorMessage = "Must be a single uppercase letter (A-Z)")]
        [Display(Name = "From Letter")]
        public string FromLetter { get; set; } = "A";

        [RegularExpression("^[A-Z]$", ErrorMessage = "Must be a single uppercase letter (A-Z)")]
        [Display(Name = "To Letter")]
        public string ToLetter { get; set; } = "Z";

        // Helper properties
        public List<TblRguhsFacultyUser> AvailableUsers { get; set; } = new();
        public List<AffiliationCollegeMaster> AvailableColleges { get; set; } = new();
        public List<Faculty> AvailableFaculties { get; set; } = new();
    }

    public class CollegeMappingRangeViewModel
    {
        [Required]
        [Display(Name = "User")]
        public int SelectedUserId { get; set; }

        [Required]
        [Display(Name = "Faculty")]
        public int SelectedFacultyId { get; set; }

        [Required]
        [RegularExpression("^[A-Z]$", ErrorMessage = "Must be a single uppercase letter")]
        [Display(Name = "College From")]
        public string CollegeFrom { get; set; } = "A";

        [Required]
        [RegularExpression("^[A-Z]$", ErrorMessage = "Must be a single uppercase letter")]
        [Display(Name = "College To")]
        public string CollegeTo { get; set; } = "Z";

        public List<TblRguhsFacultyUser> AvailableUsers { get; set; } = new();
        public List<Faculty> AvailableFaculties { get; set; } = new();
    }
}
