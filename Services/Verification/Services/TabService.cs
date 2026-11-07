using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class TabService: ITabService
    {
        private readonly ApplicationDbContext _context;
        public TabService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<int?> GetTabIdAsync(string tabName, int facultyId)
        {
            return await _context.MstTabs.AsNoTracking().Where(e => e.TabName == tabName && e.FacultyId == facultyId).Select(e => (int?)e.TabId).FirstOrDefaultAsync();
        }

        public string GetDatabaseTabName(string activeTab)
        {
            return activeTab switch
            {
                "InstitutionDetails" => "Institution Details",
                "TrustDetails" => "Trust Details",
                "TrustMemberDetails" => "Trust Member Details",
                "DeanDirectorDetails" => "Dean or Director Details",
                "PrincipalDetails" => "Principal Details",
                "UGCourseDetails" => "UG Course Details",
                "UgCourseDetails" => "UG Course Details",
                "CollegeDesignation" => "Faculty Repository",
                "ClinicalFacilities" => "Clinical Facilities",
                "LandAndBuildingDetails" => "Land And Building Details",
                "ClassroomAndLaboratory" => "Classroom And Laboratory",
                "ChairDistribution" => "Chair Distribution",
                "BedDistribution" => "Bed Distribution",
                "HostelDetails" => "Hostel Details",
                "AssociatedInstitution" => "Associated Institution",
                "DepartmentOfficesAndEducationalUnit" => "Department Offices And Educational Unit",
                "EquipmentList" => "Equipment List",
                "CA_VehicleDetails" => "Vehicle Details",
                "WorkShopDetails" => "Workshop Details",
                "AnimalHouseDetails" => "Animal House Details",
                "UgAcademicMatters" => "UG Academic Matters",
                "StaffPayScale" => "Staff Pay Scale",
                "StaffOtherDetails" => "Staff Other Details",
                "FinanceDetails" => "Finance Details",
                "LibraryServices" => "Library Services",
                "ResearchPublications" => "Research and Publications",
                "LibraryDetails" => "Library Details",
                "LibraryExpenditure" => "Library Expenditure and Services",
                "LibraryStaffDetails" => "Library Staff Details",
                "UserDetails" => "Library User Details",
                "DentalFeePaymentVerification" => "Payment Details",
                "PaymentDetails" => "Payment Details",
                "FacultyDetails" => "Faculty Details",
                "NonTeachingStaffDetails" => "Non Teaching Staff Details",
                "TeachingExperience" => "Teaching Experience",
                "ActionTakenDeficiencyReport" => "Action Taken Report",
                "GeoTaggedPhotos" => "Geo Tagged Photos",




                // Add other mappings here later
                // "OtherTab" => "Other Tab",

                _ => activeTab
            };
        }
    }
}
