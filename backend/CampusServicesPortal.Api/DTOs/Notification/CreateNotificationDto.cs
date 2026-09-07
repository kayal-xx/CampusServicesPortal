namespace CampusServicesPortal.Api.DTOs.Notification
{
    public class CreateNotificationDto
    {
        public int StudentId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
