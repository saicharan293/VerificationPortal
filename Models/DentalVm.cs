using Microsoft.AspNetCore.Mvc.Rendering;

namespace VerificationPortal.Models
{
    public class EquipmentPageVM
    {
        public string CollegeCode { get; set; } = null!;
        public int FacultyCode { get; set; }
        public List<EquipmentRowVM> Equipments { get; set; } = new();
    }

    public class AddEquipmentVM
    {
        public string DepartmentCode { get; set; }

        public string EquipmentName { get; set; }

        public string? Specification { get; set; }

        // RGUHS Norms
        public int? OneUnitRequirement { get; set; }
        public int? TwoUnitRequirement { get; set; }

        // Existing in College
        public int? OneUnitExisting { get; set; }
        public int? TwoUnitExisting { get; set; }
    }

    public class DepartmentVM
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class EquipmentRowVM
    {
        public int EquipmentId { get; set; }

        public string DepartmentCode { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public string EquipmentName { get; set; } = null!;

        public string? Specification { get; set; }

        public int? OneUnitReq { get; set; }

        public int? TwoUnitReq { get; set; }

        public int? OneUnitExisting { get; set; }

        public int? TwoUnitExisting { get; set; }
    }
}