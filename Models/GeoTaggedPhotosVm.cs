
using System;
using System.Collections.Generic;
using System.Linq;

namespace VerificationPortal.Models
{
    public class GeoTaggedPhotosVm
    {
        public string CollegeCode { get; set; } = string.Empty;
        public string FacultyCode { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;

        public List<GeoTaggedPhotoCategoryVm> Categories { get; set; } = new();

        public int TotalRequired => Categories.Sum(c => c.RequiredImages);

        public int TotalUploaded => Categories.Sum(c => c.UploadedCount);

        public int TotalPending => TotalRequired - TotalUploaded;
    }

    public class GeoTaggedPhotoCategoryVm
    {
        public int CategoryId { get; set; }

        public string CategoryCode { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public int RequiredImages { get; set; }

        public int DisplayOrder { get; set; }

        public List<GeoTaggedPhotoSlotVm> Slots { get; set; } = new();

        public int UploadedCount => Slots.Count(s => s.HasPhoto);

        public int PendingCount => RequiredImages - UploadedCount;
    }

    public class GeoTaggedPhotoSlotVm
    {
        public int CategoryId { get; set; }

        public int SlotNo { get; set; }

        public long? PhotoId { get; set; }

        public string? ImagePath { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? AccuracyMeters { get; set; }

        public DateTime? CapturedOn { get; set; }

        public DateTime? UploadedOn { get; set; }

        public bool HasPhoto => PhotoId.HasValue;
    }
}
