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

    public class CollegeMappingEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Faculty")]
        public int FacultyCode { get; set; }

        [Required]
        [RegularExpression("^[A-Z]$", ErrorMessage = "Must be a single uppercase letter")]
        [Display(Name = "From Letter")]
        public string FromLetter { get; set; } = "A";

        [Required]
        [RegularExpression("^[A-Z]$", ErrorMessage = "Must be a single uppercase letter")]
        [Display(Name = "To Letter")]
        public string ToLetter { get; set; } = "Z";

        [Required]
        [Display(Name = "From College")]
        public string CollegeFrom { get; set; } = "";

        [Required]
        [Display(Name = "To College")]
        public string CollegeTo { get; set; } = "";

        public bool IsActive { get; set; } = true;

        // Display properties
        public string UserName { get; set; } = "";
        public string UserId { get; set; } = "";
        public string UserDesignation { get; set; } = "";
        public string FacultyName { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = "";

        // Dropdown data
        public List<Faculty> AvailableFaculties { get; set; } = new();
        public List<SelectCollegeOption> AvailableColleges { get; set; } = new();

        // For client-side filtering
        public string SelectedCollegeFromCode { get; set; } = "";
        public string SelectedCollegeToCode { get; set; } = "";
    }

    public class SelectCollegeOption
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string DisplayText => $"{Code} - {Name}";
    }

    // View model for user's own college mappings view
    public class CollegeMappingWithCollegesViewModel
    {
        public TblCollegeMapping Mapping { get; set; } = null!;
        public string FacultyName { get; set; } = "";
        public string UserDesignation { get; set; } = "";
        public List<AffiliationCollegeMaster> Colleges { get; set; } = new();
        public int CollegeCount { get; set; }
        public string FromLetter { get; set; } = "";
        public string ToLetter { get; set; } = "";
        public string CollegeFromCode { get; set; } = "";
        public string CollegeToCode { get; set; } = "";

        public Dictionary<string, CollegeFeedbackStatusViewModel> FeedbackStatuses
        {
            get;
            set;
        } = new();
    }

    public class CollegeFeedbackStatusViewModel
    {
        public string Status { get; set; } = "Not Started";

        public int TotalSections { get; set; }

        public int CompletedSections { get; set; }

        public int PendingSections { get; set; }

        public int RejectedSections { get; set; }

        public DateTime? LastVerifiedOn { get; set; }

        public string? LastVerifiedBy { get; set; }

        // Optional: designation whose status is being displayed
        public string? Designation { get; set; }
    }
}
