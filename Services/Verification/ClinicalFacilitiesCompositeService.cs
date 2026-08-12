using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification
{
    public class ClinicalFacilitiesCompositeService
        : IClinicalFacilitiesCompositeService
    {
        private readonly ApplicationDbContext _context;

        public ClinicalFacilitiesCompositeService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<ClinicalFacilitiesVerificationViewModel> GetClinicalFacilitiesAsync( string collegeCode, VerificationPageContext pageContext)
        {
            // =========================================================
            // PAGE CONTEXT
            // =========================================================

            if (pageContext == null)
                throw new ArgumentNullException(nameof(pageContext));

            if (pageContext.Institution == null)
                throw new Exception("Institution not found.");

            if (string.IsNullOrWhiteSpace(pageContext.FacultyCode))
                throw new Exception("Faculty code not found.");

            var facultyCode = pageContext.FacultyCode;

            if (!int.TryParse(facultyCode, out var facultyCodeInt))
                throw new Exception("Invalid faculty code.");

            // =========================================================
            // COMPOSITE VM
            // =========================================================

            var model = new ClinicalFacilitiesVerificationViewModel
            {
                CollegeCode = collegeCode,

                FacultyCode = facultyCodeInt,

                HospitalDetails = new ClinicalHospitalVerificationRow
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCode
                },

                ClinicalStatistics = new ClinicalCapacityFormVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCode
                },

                Disciplines = new List<DisciplineVm>(),

                NptaRequirementPostvm = new NPTARequirementsPostVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    HospitalDetailsId = 0,
                    AffiliationTypeId = 0,
                    Requirements = new List<NPTAServicesItemVM>()
                },

                EngAlliedRequirementPostvm = new EngAlliedRequirementsPostVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    HospitalDetailsId = 0,
                    AffiliationTypeId = 0,
                    Requirements = new List<EngAlliedServicesItemVM>()
                }
            };

            // =========================================================
            // HOSPITAL DETAILS
            // =========================================================

            var academicIntake = await _context.AcademicIntakes
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode.ToString());

            

            int seatIntake = academicIntake.Ay2026TotalIntake;
            int seatSlab = GetSeatSlab(seatIntake);

            var hospital = await _context.HospitalDetailsForAffiliations
                .AsNoTracking()
                .Include(x => x.HospitalFacilities)
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode);

            // =========================================================
            // HOSPITAL TYPE MASTER
            // =========================================================

            var hospitalTypes = await _context.MstHospitalTypes
                .AsNoTracking()
                .Where(x => x.FacultyCode.ToString() == facultyCode)
                .ToListAsync();

            // =========================================================
            // HOSPITAL OWNED BY MASTER
            // =========================================================

            var hospitalOwnedBy = await _context.MstHospitalOwnedBies
                .AsNoTracking()
                .Where(x => x.FacultyCode.ToString() == facultyCode)
                .ToListAsync();

            // =========================================================
            // DISTRICTS
            // =========================================================

            var districts = await _context.DistrictMasters
                .AsNoTracking()
                .ToListAsync();

            // =========================================================
            // TALUKS
            // =========================================================

            var taluks = await _context.TalukMasters
                .AsNoTracking()
                .ToListAsync();

            // =========================================================
            // HOSPITAL FACILITIES MASTER
            // =========================================================

            var facilities = await _context.HospitalFacilitiesMasters
                .AsNoTracking()
                .Where(x => x.FacultyCode == facultyCode)
                .ToListAsync();

            // =========================================================
            // LOOKUPS
            // =========================================================

            var hospitalTypeLookup = hospitalTypes
                .ToDictionary(
                    x => x.Id.ToString(),
                    x => x.HospitalType);

            var hospitalOwnedByLookup = hospitalOwnedBy
                .ToDictionary(
                    x => x.Id.ToString(),
                    x => x.OwnedBy);

            var districtLookup = districts
                .ToDictionary(
                    x => x.DistrictId,
                    x => x.DistrictName);

            var talukLookup = taluks
                .ToDictionary(
                    x => x.TalukId,
                    x => x.TalukName);

            // =========================================================
            // BUILD HOSPITAL VERIFICATION SECTION
            // =========================================================

            if (hospital != null)
            {
                // -----------------------------------------------------
                // Resolve Hospital Type
                // -----------------------------------------------------

                hospitalTypeLookup.TryGetValue(
                    hospital.HospitalType ?? string.Empty,
                    out var hospitalTypeName);

                // -----------------------------------------------------
                // Resolve Hospital Owned By
                // -----------------------------------------------------

                hospitalOwnedByLookup.TryGetValue(
                    hospital.HospitalOwnedBy ?? string.Empty,
                    out var hospitalOwnedByName);

                // -----------------------------------------------------
                // Resolve District
                // -----------------------------------------------------

                districtLookup.TryGetValue(
                    hospital.HospitalDistrictId ?? string.Empty,
                    out var districtName);

                // -----------------------------------------------------
                // Resolve Taluk
                // -----------------------------------------------------

                talukLookup.TryGetValue(
                    hospital.HospitalTalukId ?? string.Empty,
                    out var talukName);

                // -----------------------------------------------------
                // Selected Facilities
                // -----------------------------------------------------

                var selectedFacilityIds = hospital.HospitalFacilities
                    .Select(x => x.FacilityId)
                    .ToHashSet();

                // -----------------------------------------
                //  DISCIPLINES 
                // -----------------------------------------
                var disciplines = await _context.MedicalAlliedDisciplineDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeInt &&
                        x.HospitalDetailsId == hospital.HospitalDetailsId)
                    .ToListAsync();

                // -----------------------------------------------------
                // Hospital Verification VM
                // -----------------------------------------------------

                model.HospitalDetails = new ClinicalHospitalVerificationRow
                {
                    HospitalDetailsId = hospital.HospitalDetailsId,

                    CollegeCode = hospital.CollegeCode,

                    FacultyCode = hospital.FacultyCode,

                    CourseLevel = hospital.CourseLevel,

                    ParentMedicalCollegeExists =
                        hospital.ParentMedicalCollegeExists,

                    HospitalType =
                        hospitalTypeName ??
                        hospital.HospitalType,

                    HospitalOwnedBy =
                        hospitalOwnedByName ??
                        hospital.HospitalOwnedBy,

                    HospitalOwnerName =
                        hospital.HospitalOwnerName,

                    HospitalName =
                        hospital.HospitalName,

                    HospitalDistrict =
                        districtName ??
                        hospital.HospitalDistrictId,

                    HospitalTaluk =
                        talukName ??
                        hospital.HospitalTalukId,

                    Location =
                        hospital.Location,

                    IsParentHospitalForOtherNursingInstitution =
                        hospital.IsParentHospitalForOtherNursingInstitution,

                    SupportingDocumentExists =
                        hospital.HospitalParentSupportingDoc != null,

                    Facilities = facilities
                        .Select(f => new HospitalFacilityVerificationRow
                        {
                            FacilityId = f.FacilityId,

                            FacilityName = f.FacilityName,

                            IsSelected =
                                selectedFacilityIds.Contains(f.FacilityId)

                        })
                        .ToList()
                };

                model.ClinicalStatistics = new ClinicalCapacityFormVM
                {
                    HospitalDetailsId = hospital.HospitalDetailsId,

                    CollegeCode = hospital.CollegeCode,

                    FacultyCode = hospital.FacultyCode,

                    AffiliationTypeId = hospital.AffiliationTypeId,

                    TotalBeds = hospital.TotalBeds,

                    OpdperDay = hospital.OpdperDay,

                    DentalChairsCount = hospital.DentalChairsCount,

                    Has24HourEmergency = hospital.Has24HourEmergency,

                    HasCriticalCareServices = hospital.HasCriticalCareServices,

                    IpdbedOccupancyPercent =
                        hospital.IpdbedOccupancyPercent,

                    AnnualOpdprevYear =
                        hospital.AnnualOpdprevYear,

                    AnnualIpdprevYear =
                        hospital.AnnualIpdprevYear,

                    DistanceBetweenCollegeAndHospitalKm =
                        hospital.DistanceBetweenCollegeAndHospitalKm,

                    IsOwnerAmemberOfTrust =
                        hospital.IsOwnerAmemberOfTrust
                };

                model.Disciplines = disciplines
                    .Select(x => new DisciplineVm
                    {
                        HospitalDetailsId = x.HospitalDetailsId,

                        CollegeCode = x.CollegeCode,

                        FacultyCode = x.FacultyCode.ToString(),

                        AffiliationTypeId = x.AffiliationTypeId,

                        DisciplineCode = x.DisciplineCode,

                        DisciplineName = x.DisciplineName,

                        SeatIntake = x.Intake ?? 0,

                        SeatSlab = x.SeatSlab,

                        IsSelected = x.IsActive
                    })
                    .ToList();


                // =========================================================
                // NPTA REQUIREMENTS
                // Nursing, Paramedical, Technical & Allied Services
                // =========================================================

                var nptaMasters = await _context.MstDentalServices
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode == facultyCodeInt &&
                        x.SectionCode == 1 &&
                        x.IsActive)
                    .OrderBy(x => x.RequirementName)
                    .ToListAsync();

                var nptaExisting = await _context.DentalServices
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.SectionCode == 1)
                    .ToListAsync();

                // =========================================================
                // ENGINEERING & ALLIED SERVICES
                // Section Code = 2
                // =========================================================

                var engAlliedMasters = await _context.MstDentalServices
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode == facultyCodeInt &&
                        x.SectionCode == 2 )
                    .OrderBy(x => x.RequirementName)
                    .ToListAsync();

                var engAlliedExisting = await _context.DentalServices
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.SectionCode == 2)
                    .ToListAsync();

                model.NptaRequirementPostvm = new NPTARequirementsPostVM
                {
                    CollegeCode = collegeCode,

                    HospitalDetailsId = hospital.HospitalDetailsId,

                    FacultyCode = facultyCodeInt,

                    AffiliationTypeId = model.AffiliationTypeId,

                    SeatSlab = hospital.DentalChairsCount ?? 0,

                    Requirements = nptaMasters
                        .Select(m =>
                        {
                            var existingItem = nptaExisting
                                .FirstOrDefault(x =>
                                    x.RequirementId == m.Id);

                            return new NPTAServicesItemVM
                            {
                                RequirementId = m.Id,

                                RequirementName = m.RequirementName,

                                SectionCode = m.SectionCode,

                                HospitalDetailsId = hospital.HospitalDetailsId,

                                SeatSlab = hospital.DentalChairsCount ?? 0,

                                IsAvailable = existingItem?.AvailabilityStatus
                            };
                        })
                        .ToList()
                };

                model.EngAlliedRequirementPostvm = new EngAlliedRequirementsPostVM
                {
                    CollegeCode = collegeCode,

                    HospitalDetailsId = hospital.HospitalDetailsId,

                    FacultyCode = facultyCodeInt,

                    AffiliationTypeId = hospital.AffiliationTypeId,

                    Requirements = engAlliedMasters
                    .Select(m =>
                    {
                        var existingItem = engAlliedExisting
                            .FirstOrDefault(x =>
                                x.RequirementId == m.Id);

                        return new EngAlliedServicesItemVM
                        {
                            RequirementId = m.Id,

                            RequirementName = m.RequirementName,

                            SectionCode = m.SectionCode,

                            HospitalDetailsId = hospital.HospitalDetailsId,

                            IsAvailable = existingItem?.AvailabilityStatus
                        };
                    })
                    .ToList()
                };

                var mstDentalBedDistribution =
                    await _context.MstDentalBedDistributions
                        .AsNoTracking()
                        .Where(x => x.FacultyCode == facultyCodeInt && x.SeatSlab == seatSlab)
                        .ToListAsync();

                var existingDentalBedDistribution =
                    await _context.DentalWardBedDistributions
                        .AsNoTracking()
                        .Where(x =>
                            x.CollegeCode == collegeCode &&
                            x.FacultyCode == facultyCodeInt &&
                            x.SeatSlab == seatSlab &&
                            x.HospitalDetailsId == hospital.HospitalDetailsId)
                        .ToListAsync();

                model.DentalWardBedDistribution =
                    mstDentalBedDistribution
                        .Select(mst =>
                        {
                            var existing =
                                existingDentalBedDistribution
                                    .FirstOrDefault(x => x.WardId == mst.Id);

                            return new DentalWardBedDistributionVm
                            {
                                WardId = mst.Id,
                                WardName = mst.WardName,
                                SeatSlab = mst.SeatSlab,
                                BedsRequired = mst.BedRequirement,

                                BedsPresent = existing?.BedsPresent ?? 0,

                                FacultyCode = facultyCodeInt,
                                CollegeCode = collegeCode,
                                HospitalDetailsId =
                                    hospital?.HospitalDetailsId ?? 0
                            };
                        })
                        .ToList();

            }

            // =========================================================
            // NEXT CLINICAL FACILITIES SECTIONS
            // =========================================================

            // Later we will populate these from the same composite service:
            //
            // model.ClinicalCapacity = ...
            // model.HospitalFacilities = ...
            // model.HospitalDocumentsToBeUploadedList = ...
            // model.AffiliatedDocumentsPostVM = ...
            // model.FieldPracticeAreaPostVM = ...
            // model.IndoorDepartment = ...
            // model.OTRequirements = ...
            // model.CasualityRequirements = ...
            // model.CSSDandLaundryRequirements = ...
            // model.RadioDiagnosisRequirements = ...
            // model.AnaesthesiologyRequirements = ...
            // model.CentralLaboratoryRequirements = ...
            // model.BloodBankRequirements = ...
            // model.YogaRequirements = ...
            // model.RadiationOncologyRequirements = ...
            // model.ArtCenterRequirements = ...
            // model.PharmacyRequirements = ...
            // model.UtilitiesRequirements = ...
            // model.OutPatientRequirements = ...
            // model.IndoorBedsUnitsRequirements = ...
            // model.IndoorBedsOccupancy = ...
            // model.SuperVisionInFieldPracticeArea = ...
            // model.NptaRequirementPostvm = ...
            // model.EngAlliedRequirementPostvm = ...
            // model.AdmAncRequirementPostvm = ...
            // model.DisciplineVm = ...
            // model.DentalWardBedDistribution = ...

            return model;
        }

        private int GetSeatSlab(int seatIntake)
        {
            return seatIntake switch
            {
                <= 50 => 50,
                <= 100 => 100,
                <= 150 => 150,
                <= 200 => 200,
                <= 250 => 250,
                <= 300 => 300,
                _ => 300
            };
        }

    }
}