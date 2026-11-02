using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VerificationPortal.Models.TabManagementViewModel
{
    public class TabViewModel
    {
        public int TabId { get; set; }

        [Required(ErrorMessage = "Please select a faculty.")]
        [Display(Name = "Faculty")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Please enter tab name.")]
        [StringLength(150, ErrorMessage = "Tab name cannot exceed 150 characters.")]
        [Display(Name = "Tab Name")]
        public string TabName { get; set; } = string.Empty;

        public string? FacultyName { get; set; }

        public int TotalSections { get; set; }

        public List<SelectListItem> Faculties { get; set; } = new();
    }

     public class SectionManagementViewModel
    {
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Please select a faculty.")]
        [Display(Name = "Faculty")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Please select a tab.")]
        [Display(Name = "Tab")]
        public int TabId { get; set; }

        [Required(ErrorMessage = "Please enter section name.")]
        [StringLength(150, ErrorMessage = "Section name cannot exceed 150 characters.")]
        [Display(Name = "Section Name")]
        public string SectionName { get; set; } = string.Empty;

        public string? FacultyName { get; set; }

        public string? TabName { get; set; }

        public List<SelectListItem> Faculties { get; set; } = new();

        public List<SelectListItem> Tabs { get; set; } = new();
    }

    public class TabAndSectionManagementViewModel
    {
        public List<TabViewModel> Tabs { get; set; } = new();

        public List<SectionManagementViewModel> Sections { get; set; } = new();

        public int? SelectedFacultyId { get; set; }

        public int? SelectedTabId { get; set; }

        public string? SelectedFacultyName { get; set; }

        public string? SelectedTabName { get; set; }
    }
}