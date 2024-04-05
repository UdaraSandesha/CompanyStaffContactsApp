using CompanyStaffContactsApp.Dtos;
using CompanyStaffContactsApp.Models;

namespace CompanyStaffContactsApp.DataAccess.Services
{
    public interface IStaffService
    {
        Task<List<Staff>> GetStaffAsync(bool includeInactive = false);
        Task SaveChangesAsync(List<StaffDto> staffDtos);
    }
}
