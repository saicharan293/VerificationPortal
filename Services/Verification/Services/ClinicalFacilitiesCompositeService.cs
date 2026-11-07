using Azure.Core;
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
        private readonly IVerificationPageService _verificationPageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClinicalFacilitiesCompositeService(ApplicationDbContext context, IVerificationPageService verificationPageService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _verificationPageService = verificationPageService;
            _httpContextAccessor = httpContextAccessor;
        }

        private int AffiliationTypeId => _httpContextAccessor.HttpContext?.Session.GetInt32("AffiliationTypeId") ?? 0;

        private string CourseLevel
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.Session.GetString("CourseLevel");
                return value ?? "";
            }
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
                },

                HospitalMedicalCertificates = new HospitalMedicalCertificatesVm
                {
                    Documents = new List<AffiliatedHospitalDocumentVm>()
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

                model.HospitalCertificates = await GetHospitalCertificatesAsync(collegeCode, facultyCodeInt, hospital.HospitalDetailsId);

                model.AnatomyActRegistration = await GetAnatomyActRegistrationAsync(collegeCode, facultyCodeInt, hospital.HospitalDetailsId);
                model.HospitalTieUp = await GetHospitalTieUpAsync(collegeCode, hospital.HospitalDetailsId);

                var hospitalDocuments = await _context.AffiliatedHospitalDocuments
                    .Where(x => x.CollegeCode == collegeCode)
                    .ToListAsync();

                model.HospitalMedicalCertificates = new HospitalMedicalCertificatesVm
                {
                    Documents = hospitalDocuments.Select(x => new AffiliatedHospitalDocumentVm
                    {
                        DocumentId = x.DocumentId,
                        CollegeCode = x.CollegeCode,
                        HospitalType = x.HospitalType,
                        HospitalName = x.HospitalName,
                        TotalBeds = x.TotalBeds,
                        DocumentName = x.DocumentName,
                        CreatedDate = x.CreatedDate,
                        CourseLevel = x.CourseLevel,
                        DocumentFilePath = x.DocumentFilePth
                    }).ToList()
                };

                var mastersBySection = await _context.MstIndoorInfrastructureRequirementsMasters
                    .Where(m =>
                        m.FacultyCode.ToString() == facultyCode &&
                        m.AffiliationTypeId == AffiliationTypeId &&
                        m.IsActive)
                    .ToListAsync();

                var existingBySection = await _context.IndoorInfrastructureRequirementsCompliances
                    .Where(r =>
                        r.CollegeCode == collegeCode &&
                        r.FacultyCode.ToString() == facultyCode &&
                        r.AffiliationTypeId == AffiliationTypeId &&
                        r.CourseLevel == CourseLevel &&
                        hospital != null &&
                        r.HospitalDetailsId == hospital.HospitalDetailsId)
                    .ToListAsync();

                // Section 1 - Indoor Department
                var indoorDeptReqMaster = mastersBySection.Where(r => r.SectionCode == "1").ToList();
                var existingIndoorRequirement = existingBySection.Where(r => r.SectionCode == "1").ToList();

                // Section 2 - Operation Theatre
                var otReqMaster = mastersBySection.Where(r => r.SectionCode == "2").ToList();
                var existingOTRequirement = existingBySection.Where(r => r.SectionCode == "2").ToList();

                // Section 3 - Casualty
                var casualityReqMaster = mastersBySection.Where(r => r.SectionCode == "3").ToList();
                var existingCasualityRequirement = existingBySection.Where(r => r.SectionCode == "3").ToList();

                // Section 4 - CSSD and Laundry
                var CSSDandLaundryReqMaster = mastersBySection.Where(r => r.SectionCode == "4").ToList();
                var existingCSSDandLaundryRequirement = existingBySection.Where(r => r.SectionCode == "4").ToList();

                // Section 5 - Radio Diagnosis
                var RadioDiagnosisReqMaster = mastersBySection.Where(r => r.SectionCode == "5").ToList();
                var existingRadioDiagnosisRequirement = existingBySection.Where(r => r.SectionCode == "5").ToList();

                // Section 6 - Anaesthesiology
                var AnaesthesiologyReqMaster = mastersBySection.Where(r => r.SectionCode == "6").ToList();
                var existingAnaesthesiologyRequirement = existingBySection.Where(r => r.SectionCode == "6").ToList();

                // Section 7 - Central Laboratory
                var CentralLaboratoryReqMaster = mastersBySection.Where(r => r.SectionCode == "7").ToList();
                var existingCentralLaboratoryRequirement = existingBySection.Where(r => r.SectionCode == "7").ToList();

                // Section 8 - Blood Bank
                var BloodBankReqMaster = mastersBySection.Where(r => r.SectionCode == "8").ToList();
                var existingBloodBankRequirement = existingBySection.Where(r => r.SectionCode == "8").ToList();

                // Section 9 - Yoga
                var YogaReqMaster = mastersBySection.Where(r => r.SectionCode == "9").ToList();
                var existingYogaRequirement = existingBySection.Where(r => r.SectionCode == "9").ToList();

                // Section 10 - Radiation Oncology
                var RadiationOncologyReqMaster = mastersBySection.Where(r => r.SectionCode == "10").ToList();
                var existingRadiationOncologyRequirement = existingBySection.Where(r => r.SectionCode == "10").ToList();

                // Section 11 - Art Center
                var ArtCenterReqMaster = mastersBySection.Where(r => r.SectionCode == "11").ToList();
                var existingArtCenterRequirement = existingBySection.Where(r => r.SectionCode == "11").ToList();

                // Section 12 - Pharmacy
                var PharmacyReqMaster = mastersBySection.Where(r => r.SectionCode == "12").ToList();
                var existingPharmacyRequirement = existingBySection.Where(r => r.SectionCode == "12").ToList();

                // Section 13 - Utilities
                var UtilitiesReqMaster = mastersBySection.Where(r => r.SectionCode == "13").ToList();
                var existingUtilitiesRequirement = existingBySection.Where(r => r.SectionCode == "13").ToList();

                // Section 14 - Indoor Beds and Units
                var IndoorBedsUnitsReqMaster = mastersBySection.Where(r => r.SectionCode == "14").ToList();
                var existingIndoorBedsUnitsRequirement = existingBySection.Where(r => r.SectionCode == "14").ToList();

                // Section 15 - Outpatient Area
                var OutpatientAreaReqMaster = mastersBySection.Where(r => r.SectionCode == "15").ToList();
                var existingOutpatientAreaRequirement = existingBySection.Where(r => r.SectionCode == "15").ToList();

                var indoorBase = new IndoorInfrastructureVerificationBaseVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel
                };

                model.IndoorDepartmentRequirements = new IndoorDepartmentRequirementsVerificationVM
                {
                    CollegeCode = indoorBase.CollegeCode,
                    FacultyCode = indoorBase.FacultyCode,
                    AffiliationTypeId = indoorBase.AffiliationTypeId,
                    HospitalDetailsId = indoorBase.HospitalDetailsId,
                    CourseLevel = indoorBase.CourseLevel,
                    Requirements = BuildVerificationRequirements<IndoorDepartmentRequirementVerificationVM>(
                        indoorDeptReqMaster, existingIndoorRequirement)
                };

                model.OTRequirements = new OTRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<OTRequirementVerificationVM>(
                        otReqMaster, existingOTRequirement)
                };

                model.CasualityRequirements = new CasualityRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<CasualityRequirementVerificationVM>(
                        casualityReqMaster, existingCasualityRequirement)
                };

                model.CSSDandLaundryRequirements = new CSSDandLaundryRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<CSSDandLaundryRequirementVerificationVM>(
                        CSSDandLaundryReqMaster, existingCSSDandLaundryRequirement)
                };

                model.RadioDiagnosisRequirements = new RadioDiagnosisRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<RadioDiagnosisRequirementVerificationVM>(
                        RadioDiagnosisReqMaster, existingRadioDiagnosisRequirement)
                };

                model.AnaesthesiologyRequirements = new AnaesthesiologyRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<AnaesthesiologyRequirementVerificationVM>(
                        AnaesthesiologyReqMaster, existingAnaesthesiologyRequirement)
                };

                model.CentralLaboratoryRequirements = new CentralLaboratoryRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<CentralLaboratoryRequirementVerificationVM>(
                        CentralLaboratoryReqMaster, existingCentralLaboratoryRequirement)
                };

                model.BloodBankRequirements = new BloodBankRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<BloodBankRequirementVerificationVM>(
                        BloodBankReqMaster, existingBloodBankRequirement)
                };

                model.YogaRequirements = new YogaRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<YogaRequirementVerificationVM>(
                        YogaReqMaster, existingYogaRequirement)
                };

                model.RadiationOncologyRequirements = new RadiationOncologyRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<RadiationOncologyRequirementVerificationVM>(
                        RadiationOncologyReqMaster, existingRadiationOncologyRequirement)
                };

                model.ArtCenterRequirements = new ArtCenterRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<ArtCenterRequirementVerificationVM>(
                        ArtCenterReqMaster, existingArtCenterRequirement)
                };

                model.PharmacyRequirements = new PharmacyRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<PharmacyRequirementVerificationVM>(
                        PharmacyReqMaster, existingPharmacyRequirement)
                };

                model.UtilitiesRequirements = new UtilitiesRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<UtilitiesRequirementVerificationVM>(
                        UtilitiesReqMaster, existingUtilitiesRequirement)
                };

                model.IndoorBedsUnitsRequirements = new IndoorBedsUnitsRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<IndoorBedsUnitsRequirementVerificationVM>(
                        IndoorBedsUnitsReqMaster, existingIndoorBedsUnitsRequirement)
                };

                model.OutpatientAreaRequirements = new OutpatientAreaRequirementsVerificationVM
                {
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCodeInt,
                    AffiliationTypeId = AffiliationTypeId,
                    HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                    CourseLevel = CourseLevel,
                    Requirements = BuildVerificationRequirements<OutpatientAreaRequirementVerificationVM>(
                        OutpatientAreaReqMaster, existingOutpatientAreaRequirement)
                };


                var existingSuperVisionInFpa = await _context.SuperVisionInFieldPracticeAreas
                    .AsNoTracking()
                    .Where(f =>
                        f.CollegeCode == collegeCode &&
                        f.FacultyCode == facultyCodeInt &&
                        f.AffiliationTypeId == AffiliationTypeId &&
                        f.CourseLevel == CourseLevel &&
                        hospital != null &&
                        f.HospitalDetailsId == hospital.HospitalDetailsId)
                    .ToListAsync();

                    model.SuperVisionInFieldPracticeArea = new SuperVisionInFieldPracticeAreaVerificationVM
                    {
                        CollegeCode = collegeCode,
                        FacultyCode = facultyCodeInt,
                        AffiliationTypeId = AffiliationTypeId,
                        HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                        CourseLevel = CourseLevel,

                        Supervisors = existingSuperVisionInFpa.Select(f => new SuperVisionInFieldPracticeAreaVerificationItemVM
                        {
                            Id = f.Id,
                            Post = f.Post,
                            Name = f.Name,
                            Qualification = f.Qualification,
                            YearOfQualification = f.YearOfQualification,
                            University = f.University,
                            UgFromDate = f.UgFromDate,
                            UgToDate = f.UgToDate,
                            PgFromDate = f.PgFromDate,
                            PgToDate = f.PgToDate,
                            Responsibilities = f.Responsibilities
                        }).ToList()
                    };

                    // 1. Fetch Indoor Beds Department Master
                    var departmentsMaster = await _context.MstIndoorBedsDepartmentMasters
                        .AsNoTracking()
                        .Where(d =>
                            d.FacultyCode == facultyCodeInt &&
                            d.AffiliationTypeId == AffiliationTypeId)
                        .OrderBy(d => d.DeptId)
                        .ToListAsync();


                    // 2. Fetch Seat Slab Master
                    var seatSlabs = await _context.SeatSlabMasters
                        .AsNoTracking()
                        .Where(s => s.FacultyCode == facultyCodeInt)
                        .OrderBy(s => s.SeatSlab)
                        .ToListAsync();


                    // 3. Fetch Existing College Indoor Beds Occupancy
                    var existingBeds = await _context.IndoorBedsOccupancies
                        .AsNoTracking()
                        .Where(x =>
                            x.CollegeCode == collegeCode &&
                            x.FacultyCode == facultyCodeInt &&
                            x.AffiliationTypeId == AffiliationTypeId &&
                            x.CourseLevel == CourseLevel)
                        .ToListAsync();


                    // 4. Identify Seat Slab from Existing Records
                    var seatSlabId = existingBeds
                        .Select(x => x.SeatSlabId)
                        .FirstOrDefault();


                    // 5. Filter Applicable Seat Slab
                    var filteredSeatSlabs = seatSlabs
                        .Where(s => s.SeatSlabId == seatSlabId)
                        .ToList();


                    // 6. Fetch RGUHS Indoor Beds Master Requirements
                    var bedMasterRequirements = await _context.MstIndoorBedsOccupancyMasters
                        .AsNoTracking()
                        .Where(m =>
                            m.FacultyCode == facultyCodeInt &&
                            m.AffiliationTypeId == AffiliationTypeId &&
                            m.SeatSlabId == seatSlabId)
                        .ToListAsync();


                    // 7. Create Lookup Dictionaries
                    var masterLookup = bedMasterRequirements
                        .ToDictionary(m => (m.DepartmentCode, m.SeatSlabId));

                    var savedLookup = existingBeds
                        .ToDictionary(x => (x.DepartmentId, x.SeatSlabId));


                    // 8. Build Indoor Beds Verification Items
                    var indoorBedsItems =
                        (
                            from d in departmentsMaster
                            from s in filteredSeatSlabs

                            let master = masterLookup.GetValueOrDefault((d.DeptId, s.SeatSlabId))

                            let saved = savedLookup.GetValueOrDefault((d.DeptId, s.SeatSlabId))

                            select new IndoorBedsOccupancyItemVM
                            {
                                DepartmentId = d.DeptId,
                                DepartmentName = d.DepartmentName,
                                SeatSlabId = s.SeatSlabId,
                                SeatSlab = s.SeatSlab,
                                RGUHSintake = saved?.Rguhsintake > 0 ? saved.Rguhsintake : master?.RequiredBeds ?? 0,
                                CollegeIntake = saved?.CollegeIntake ?? 0
                            }
                        ).ToList();


                    // 9. Assign to Clinical Facilities Verification ViewModel
                    model.IndoorBedsRequirements = new IndoorBedsRequirementsVerificationVM
                    {
                        CollegeCode = collegeCode,
                        FacultyCode = facultyCodeInt,
                        AffiliationTypeId = AffiliationTypeId,
                        CourseLevel = CourseLevel,
                        Items = indoorBedsItems
                    };
            }

            return model;
        }


        private List<T> BuildVerificationRequirements<T>( List<MstIndoorInfrastructureRequirementsMaster> masters, List<IndoorInfrastructureRequirementsCompliance> existing)
            where T : RequirementVerificationBaseVM, new()
        {
            return masters.Select(m =>
            {
                var compliance = existing.FirstOrDefault(
                    x => x.RequirementId == m.Id);

                return new T
                {
                    RequirementId = m.Id,
                    RequirementName = m.RequirementName,
                    IsAvailable = compliance?.IsCompliant ?? false,
                    Remarks = compliance?.Remarks,
                    IsVerified = false
                };
            }).ToList();
        }

        //private async Task<int?> GetDocumentIdAsync(string documentName)
        //{
        //    if (string.IsNullOrWhiteSpace(documentName)) return null;

        //    return await _context.MstDocuments.AsNoTracking().Where(d => d.DocumentName == documentName)
        //        .Select(e => (int?)e.DocumentId)
        //        .FirstOrDefaultAsync();
        //}


        private async Task<HospitalCertificatesVm> GetHospitalCertificatesAsync(string collegeCode, int facultyCode, int hospitalDetailsId)
        {

            var hospital = await _context.HospitalDetailsForAffiliations.AsNoTracking()
                .FirstOrDefaultAsync(e => e.CollegeCode == collegeCode && e.HospitalDetailsId == hospitalDetailsId);

            if (hospital == null) return new HospitalCertificatesVm();

            var pageContext = await GetPageContextAsync(collegeCode);
            var model = new HospitalCertificatesVm
            {
                PageContext = pageContext,
                KPMECertificatePath = hospital.KpmecertificatePdfPath,
                KPMECertificateFileId = await _verificationPageService.GetDocumentIdAsync("KPME Certificate"),
                PollutionControlBoardCertificatePath = hospital.PollutionControlBoardCertificatePdfPath,
                PollutionControlBoardCertificateFileID = await _verificationPageService.GetDocumentIdAsync("Pollution Control Board Certificate"),
                BioMedicalCertificatePath = hospital.BioMedicalCertificatePdfPath,
                BioMedicalCertificateFileId = await _verificationPageService.GetDocumentIdAsync("Bio-Medical Waste Certificate"),
                DrugFreeCampusCertificationPath = hospital.DrugFreeCampusCertificationPdfPath,
                DrugFreeCampusCertificationFileId = await _verificationPageService.GetDocumentIdAsync("Drug Free Campus Certification"),
                ProposedPlansForFutureDevelopmentsPath = hospital.ProposedPlansForFutureDevelopmentsPdfPath,
                ProposedPlansForFutureDevelopmentsId = await _verificationPageService.GetDocumentIdAsync("Proposed Plans for Future Developments"),
            };

            return model;
        }

        private async Task<VerificationPageContext> GetPageContextAsync(string collegeCode)
        {
            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            if (institution == null)
                throw new Exception($"Institution not found for college code '{collegeCode}'.");

            if (string.IsNullOrWhiteSpace(institution.FacultyCode))
                throw new Exception("Faculty code not found.");

            var faculty = await _context.Faculties.AsNoTracking().FirstOrDefaultAsync(x => x.FacultyId.ToString() == institution.FacultyCode);

            return new VerificationPageContext
            {
                Institution = institution,
                Faculty = faculty
            };
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

        private async Task<AnatomyActRegistrationVm> GetAnatomyActRegistrationAsync(string collegeCode, int facultyCode, int hospitalDetailsId)
        {
            var hospital = await _context.HospitalDetailsForAffiliations
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.CollegeCode == collegeCode &&
                    e.HospitalDetailsId == hospitalDetailsId);

            if (hospital == null)
                return new AnatomyActRegistrationVm();

            var pageContext =
                await GetPageContextAsync(collegeCode);

            var sectionFeedback = await _verificationPageService.GetSectionFeedbackByTabNameAsync(collegeCode, "Clinical Facilities");

            return new AnatomyActRegistrationVm
            {
                PageContext = pageContext,

                HasAnatomyActRegistration = hospital.HasAnatomyActRegistration,

                AnatomyActRegistrationDetails = hospital.AnatomyActRegistrationDetails,

                AnatomyActRegistrationPdfPath = hospital.AnatomyActRegistrationPdfPath,
                AnatomyActRegistrationPdfFileId = await _verificationPageService.GetDocumentIdAsync("Anatomy Act Registration"),
                SectionFeedback = sectionFeedback,
            };
        }

        private async Task<HospitalTieUpVm> GetHospitalTieUpAsync(string collegeCode, int hospitalDetailsId)
        {
            var hospital = await _context.HospitalDetailsForAffiliations.AsNoTracking().Include(e => e.HospitalTieUpDetails)
                .FirstOrDefaultAsync(e => e.CollegeCode == collegeCode && e.HospitalDetailsId == hospitalDetailsId);

            if (hospital == null) return new HospitalTieUpVm();

            var pageContext = await GetPageContextAsync(collegeCode);

            var returnUrl = $"/VerificationDashboard/ClinicalFacilities?collegeCode={collegeCode}";

            var sectionFeedback = await _verificationPageService.GetSectionFeedbackByTabNameAsync(collegeCode, "Clinical Facilities");

            var tieUps = new List<HospitalTieUpDetailVM>();

            foreach (var e in hospital.HospitalTieUpDetails
                         .Where(e => !e.IsDeleted))
            {
                var documentId =
                    await _verificationPageService.GetDocumentIdAsync("Hospital Tie up Document");

                tieUps.Add(new HospitalTieUpDetailVM
                {
                    Id = e.Id,
                    HospitalDetailsId = e.HospitalDetailsId,
                    CollegeCode = e.CollegeCode,
                    FacultyCode = e.FacultyCode,
                    CourseLevel = e.CourseLevel,
                    TieUpType = e.TieUpType,
                    HospitalName = e.HospitalName,
                    HospitalAddress = e.HospitalAddress,
                    TieUpDetails = e.TieUpDetails,

                    SupportingDocumentContentType = e.SupportingDocumentContentType,

                    SupportingDocumentFileId =
                        documentId,

                    SupportingDocumentName =
                        e.SupportingDocumentName,

                    SupportingDocumentPath =
                        e.SupportingDocumentPath,

                    IsDeleted = e.IsDeleted,
                    CreatedOn = e.CreatedOn,
                    ModifiedOn = e.ModifiedOn
                });
            }

            return new HospitalTieUpVm
            {
                PageContext = pageContext,
                HasHospitalTieUp = hospital.HasHospitalTieUp,
                HospitalTieUps = tieUps,
                SectionFeedback = sectionFeedback
            };

        }

    }
}