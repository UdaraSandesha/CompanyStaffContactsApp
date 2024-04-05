using AutoMapper;
using CompanyStaffContactsApp.Dtos;
using CompanyStaffContactsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyStaffContactsApp.DataAccess.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StaffService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Staff>> GetStaffAsync(bool includeInactive = false)
        {
            var query = _context.Staffs.Include(s => s.Manager);

            if (includeInactive)
            {
                return await query.ToListAsync();
            }
            else
            {
                return await query.Where(x => x.Status == Enums.StaffStatus.Active).ToListAsync();
            }

        }

        public async Task SaveChangesAsync(List<StaffDto> staffDtos)
        {
            var staffEntities = _mapper.Map<List<Staff>>(staffDtos);

            foreach (var staff in staffEntities)
            {
                var existingStaff = await _context.Staffs.FindAsync(staff.Id);
                if (existingStaff != null)
                {
                    var manager = existingStaff.Manager;
                    _context.Entry(existingStaff).CurrentValues.SetValues(staff);
                    existingStaff.Manager = manager;
                    existingStaff.ManagerId = manager?.Id;

                }
                else
                {
                    _context.Staffs.Add(staff);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
