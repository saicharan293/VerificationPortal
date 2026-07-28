namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationEntity
    {
        bool? IsDeoVerified { get; set; }
        string? DeoRemarks { get; set; }
        DateTime? DeoVerifiedDate { get; set; }
        string? DeoName { get; set; }

        bool? IsJrVerified { get; set; }
        string? JrRemarks { get; set; }
        DateTime? JrVerifiedDate { get; set; }
        string? JrName { get; set; }

        bool? IsSoVerified { get; set; }
        string? SoRemarks { get; set; }
        DateTime? SoVerifiedDate { get; set; }
        string? SoName { get; set; }

        bool? IsArVerified { get; set; }
        string? ArRemarks { get; set; }
        DateTime? ArVerifiedDate { get; set; }
        string? ArName { get; set; }

        bool? IsRgVerified { get; set; }
        string? RgRemarks { get; set; }
        DateTime? RgVerifiedDate { get; set; }
        string? RgName { get; set; }

        bool? IsReVerified { get; set; }
        string? ReRemarks { get; set; }
        DateTime? ReVerifiedDate { get; set; }
        string? ReName { get; set; }

        bool? IsDrVerified { get; set; }
        string? DrRemarks { get; set; }
        DateTime? DrVerifiedDate { get; set; }
        string? DrName { get; set; }

        bool? IsVcVerified { get; set; }
        string? VcRemarks { get; set; }
        DateTime? VcVerifiedDate { get; set; }
        string? VcName { get; set; }
    }
}
