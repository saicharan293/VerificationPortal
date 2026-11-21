using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class GeoTaggedPhotosService : IGeoTaggedPhotosService
    {
        private readonly string _connectionString;

        public GeoTaggedPhotosService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
        }


        public async Task<GeoTaggedPhotosVm> GetPhotosAsync( string collegeCode, string facultyCode)
        {
            var vm = new GeoTaggedPhotosVm
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode
            };

            var categories = new Dictionary<int, GeoTaggedPhotoCategoryVm>();

            await using var con = new SqlConnection(_connectionString);

            await using var cmd = new SqlCommand( "dbo.usp_GeoPhoto_GetByCollege", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@CollegeCode", SqlDbType.VarChar, 20).Value = collegeCode;
            cmd.Parameters.Add("@FacultyCode", SqlDbType.VarChar, 20).Value = facultyCode;

            await con.OpenAsync();

            await using var rd = await cmd.ExecuteReaderAsync();

            int oCatId = rd.GetOrdinal("CategoryId");
            int oCatCode = rd.GetOrdinal("CategoryCode");
            int oCatName = rd.GetOrdinal("CategoryName");
            int oReq = rd.GetOrdinal("RequiredImages");
            int oOrder = rd.GetOrdinal("DisplayOrder");
            int oSlot = rd.GetOrdinal("ImageSlotNo");
            int oPhotoId = rd.GetOrdinal("PhotoId");
            int oPath = rd.GetOrdinal("ImagePath");
            int oLat = rd.GetOrdinal("Latitude");
            int oLon = rd.GetOrdinal("Longitude");
            int oAcc = rd.GetOrdinal("AccuracyMeters");
            int oCap = rd.GetOrdinal("CapturedOn");
            int oUp = rd.GetOrdinal("UploadedOn");

            while (await rd.ReadAsync())
            {
                int categoryId = rd.GetInt32(oCatId);

                if (!categories.TryGetValue(categoryId, out var category))
                {
                    category = new GeoTaggedPhotoCategoryVm
                    {
                        CategoryId = categoryId,
                        CategoryCode = rd.GetString(oCatCode),
                        CategoryName = rd.GetString(oCatName),
                        RequiredImages = rd.GetByte(oReq),
                        DisplayOrder = rd.GetByte(oOrder)
                    };

                    categories.Add(categoryId, category);
                    vm.Categories.Add(category);
                }

                category.Slots.Add(new GeoTaggedPhotoSlotVm
                {
                    CategoryId = categoryId,
                    SlotNo = rd.GetByte(oSlot),

                    PhotoId = rd.IsDBNull(oPhotoId) ? null : rd.GetInt64(oPhotoId),
                    ImagePath = rd.IsDBNull(oPath) ? null : rd.GetString(oPath),

                    Latitude = rd.IsDBNull(oLat) ? null : rd.GetDecimal(oLat),
                    Longitude = rd.IsDBNull(oLon) ? null : rd.GetDecimal(oLon),
                    AccuracyMeters = rd.IsDBNull(oAcc) ? null : rd.GetDecimal(oAcc),

                    CapturedOn = rd.IsDBNull(oCap) ? null : rd.GetDateTime(oCap),
                    UploadedOn = rd.IsDBNull(oUp) ? null : rd.GetDateTime(oUp)
                });
            }

            return vm;
        }

    }
}