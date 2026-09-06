namespace CampusServicesPortal.Api.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalStudents { get; set; }
    public int TotalHostelApplications { get; set; }
    public int TotalLabBookings { get; set; }
    public int TotalEventRegistrations { get; set; }
    public int TotalComplaints { get; set; }
    public int PendingComplaints { get; set; }
    public int TotalCertificateRequests { get; set; }
    public int PendingCertificateRequests { get; set; }
    public int TotalFeePayments { get; set; }
    public int PaidFeePayments { get; set; }
    public int UnreadNotifications { get; set; }
}