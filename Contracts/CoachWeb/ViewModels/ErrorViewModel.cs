namespace Contracts.CoachWeb.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? ErrorMessage { get; set; }
        public bool ShowReportButton { get; set; } = false;
        public string? Context { get; set; }
    }
}