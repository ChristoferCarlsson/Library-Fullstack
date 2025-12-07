using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}
