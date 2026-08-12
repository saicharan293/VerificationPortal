namespace VerificationPortal.Models
{
    public class FacultyDetailsVerificationViewModel
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationType { get; set; }

        public List<FacultyDetailsVerificationRow> FacultyDetails { get; set; }
            = new();
    }

    public class FacultyDetailsVerificationRow
    {
        public int Id { get; set; }

        public string? NameOfFaculty { get; set; }

        public string? Designation { get; set; }

        public string? DepartmentDetails { get; set; }

        public string? RecognizedPgTeacher { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? Pan { get; set; }

        public string? Aadhaar { get; set; }

        public string? RecognizedPhDteacher { get; set; }

        public string? LitigationPending { get; set; }

        public string? IsExaminer { get; set; }

        public string? ExaminerFor { get; set; }

        public string? GuideRecognitionDocPath { get; set; }

        public string? PhDrecognitionDocPath { get; set; }

        public string? LitigationDocPath { get; set; }

        public DateOnly? From { get; set; }

        public DateOnly? To { get; set; }

        public string? RemoveRemarks { get; set; }
    }
}