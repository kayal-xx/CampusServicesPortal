using CampusServicesPortal.Api.DTOs.Dashboard;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}