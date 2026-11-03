using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public class GeoTaggedPhotosViewModel
{
    public VerificationPageContextVm PageContext { get; set; } = new();

    public List<GeoPhotoItemVm> Photos { get; set; } = new();

    public List<GeoPhotoCategoryMaster> Categories { get; set; } = new();

    // Grouped data from backend
    public Dictionary<string, List<GeoPhotoItemVm>> PhotosByCategory { get; set; } = new();

    // Section feedback grouped by category name
    public Dictionary<string, SectionFeedbackViewModel> FeedbackByCategory { get; set; } = new();

    public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
}

public class GeoPhotoItemVm
{
    public long PhotoId { get; set; }
    public string? CollegeCode { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryCode { get; set; }
    public byte ImageSlotNo { get; set; }
    public string? ImagePath { get; set; }
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? AccuracyMeters { get; set; }
    public DateTime? CapturedOn { get; set; }
    public DateTime UploadedOn { get; set; }
    public string? UploadedBy { get; set; }
    public int? FileSizeKb { get; set; }
}