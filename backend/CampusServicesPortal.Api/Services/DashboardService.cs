using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.DTOs.Dashboard;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        return new DashboardSummaryDto
        {
            TotalStudents = await _context.Students.CountAsync(),

            TotalHostelApplications =
                await _context.HostelApplications.CountAsync(),

            TotalLabBookings =
                await _context.LabBookings.CountAsync(),

            TotalEventRegistrations =
                await _context.EventRegistrations.CountAsync(),

            TotalComplaints =
                await _context.Complaints.CountAsync(),

            PendingComplaints =
                await _context.Complaints
                    .CountAsync(x => x.Status == "Pending"),

            TotalCertificateRequests =
                await _context.CertificateRequests.CountAsync(),

            PendingCertificateRequests =
                await _context.CertificateRequests
                    .CountAsync(x => x.Status == "Pending"),

            TotalFeePayments =
                await _context.FeePayments.CountAsync(),

            PaidFeePayments =
                await _context.FeePayments
                    .CountAsync(x => x.IsPaid),

            UnreadNotifications =
                await _context.Notifications
                    .CountAsync(x => !x.IsRead)
        };
    }
}