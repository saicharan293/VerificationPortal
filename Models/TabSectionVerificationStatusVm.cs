namespace VerificationPortal.Models
{
    public class TabSectionVerificationStatusVm
    {
        public string TabName { get; set; } = string.Empty;
        public bool IsTabVerified { get; set; }
        public List<string> PendingSections { get; set; } = new();
        public bool IsCompleted { get; set; }
    }
}
