using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Services.Verification.Handlers
{
    public class AffInstitutionVerificationHandler : IVerificationHandler<AffInstitutionsDetail>
    {
        public static class UserRoles
        {
            public const string DataEntryOperator = "Data Entry Operator";
            public const string JuniorAssistant = "Junior Assistant";
            public const string SectionOfficer = "Section Officer";
            public const string AssistantRegistrar = "Assistant Registrar";
            public const string Registrar = "Registrar";
            public const string RegistrarEvaluation = "Registrar Evaluation";
            public const string Director = "Director";
            public const string ViceChancellor = "Vice Chancellor";
        }

        public void ApplyVerification(AffInstitutionsDetail entity, VerificationRequest request)
        {
            bool isApproved = request.Status.Equals("accept", StringComparison.OrdinalIgnoreCase);

            switch (request.Role)
            {
                case UserRoles.DataEntryOperator:
                    entity.IsDeoVerified = isApproved;
                    entity.DeoRemarks = request.Remarks;
                    entity.DeoVerifiedDate = DateTime.Now;
                    entity.DeoName = request.VerifiedBy;
                    break;

                case UserRoles.JuniorAssistant:
                    entity.IsJrVerified = isApproved;
                    entity.JrRemarks = request.Remarks;
                    entity.JrVerifiedDate = DateTime.Now;
                    entity.JrName = request.VerifiedBy;
                    break;

                case UserRoles.SectionOfficer:
                    entity.IsSoVerified = isApproved;
                    entity.SoRemarks = request.Remarks;
                    entity.SoVerifiedDate = DateTime.Now;
                    entity.SoName = request.VerifiedBy;
                    break;

                case UserRoles.AssistantRegistrar:
                    entity.IsArVerified = isApproved;
                    entity.ArRemarks = request.Remarks;
                    entity.ArVerifiedDate = DateTime.Now;
                    entity.ArName = request.VerifiedBy;
                    break;

                case UserRoles.Registrar:
                    entity.IsRgVerified = isApproved;
                    entity.RgRemarks = request.Remarks;
                    entity.RgVerifiedDate = DateTime.Now;
                    entity.RgName = request.VerifiedBy;
                    break;

                case UserRoles.RegistrarEvaluation:
                    entity.IsReVerified = isApproved;
                    entity.ReRemarks = request.Remarks;
                    entity.ReVerifiedDate = DateTime.Now;
                    entity.ReName = request.VerifiedBy;
                    break;

                case UserRoles.Director:
                    entity.IsDrVerified = isApproved;
                    entity.DrRemarks = request.Remarks;
                    entity.DrVerifiedDate = DateTime.Now;
                    entity.DrName = request.VerifiedBy;
                    break;

                case UserRoles.ViceChancellor:
                    entity.IsVcVerified = isApproved;
                    entity.VcRemarks = request.Remarks;
                    entity.VcVerifiedDate = DateTime.Now;
                    entity.VcName = request.VerifiedBy;
                    break;

                default:
                    throw new ArgumentException($"Unsupported verification role: {request.Role}");
            }
        }


        public VerificationDisplayModel GetVerification(AffInstitutionsDetail entity, string role)
        {
            return role switch
            {
                UserRoles.DataEntryOperator => new VerificationDisplayModel
                {
                    Remarks = entity.DeoRemarks,
                    VerifiedBy = entity.DeoName,
                    VerifiedDate = entity.DeoVerifiedDate,
                    IsVerified = entity.IsDeoVerified
                },

                UserRoles.JuniorAssistant => new VerificationDisplayModel
                {
                    Remarks = entity.JrRemarks,
                    VerifiedBy = entity.JrName,
                    VerifiedDate = entity.JrVerifiedDate,
                    IsVerified = entity.IsJrVerified
                },

                UserRoles.SectionOfficer => new VerificationDisplayModel
                {
                    Remarks = entity.SoRemarks,
                    VerifiedBy = entity.SoName,
                    VerifiedDate = entity.SoVerifiedDate,
                    IsVerified = entity.IsSoVerified
                },

                UserRoles.AssistantRegistrar => new VerificationDisplayModel
                {
                    Remarks = entity.ArRemarks,
                    VerifiedBy = entity.ArName,
                    VerifiedDate = entity.ArVerifiedDate,
                    IsVerified = entity.IsArVerified
                },

                UserRoles.Registrar => new VerificationDisplayModel
                {
                    Remarks = entity.RgRemarks,
                    VerifiedBy = entity.RgName,
                    VerifiedDate = entity.RgVerifiedDate,
                    IsVerified = entity.IsRgVerified
                },

                UserRoles.RegistrarEvaluation => new VerificationDisplayModel
                {
                    Remarks = entity.ReRemarks,
                    VerifiedBy = entity.ReName,
                    VerifiedDate = entity.ReVerifiedDate,
                    IsVerified = entity.IsReVerified
                },

                UserRoles.Director => new VerificationDisplayModel
                {
                    Remarks = entity.DrRemarks,
                    VerifiedBy = entity.DrName,
                    VerifiedDate = entity.DrVerifiedDate,
                    IsVerified = entity.IsDrVerified
                },

                UserRoles.ViceChancellor => new VerificationDisplayModel
                {
                    Remarks = entity.VcRemarks,
                    VerifiedBy = entity.VcName,
                    VerifiedDate = entity.VcVerifiedDate,
                    IsVerified = entity.IsVcVerified
                },

                _ => new VerificationDisplayModel()
            };
        }

    }
}
