using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace BusinessLayer.Implementations
{
  public class DailyWorkingHourService : IDailyWorkingHourService
  {
    private readonly HRMSContext _context;

    public DailyWorkingHourService(HRMSContext context)
    {
      _context = context;
    }

    // SAVE
    public async Task SaveDailyWorkingHourAsync(List<DailyWorkingHourDto> data)
    {
      foreach (var item in data)
      {
        var user = _context.Users
            .FirstOrDefault(x => x.EmployeeCode == item.EmployeeCode);

        if (user == null) continue;

        var existing = _context.DailyWorkingHours
            .FirstOrDefault(x =>
                x.UserId == user.UserId &&
                x.DayOfWeek == item.DayOfWeek);

        if (existing != null)
        {
          existing.ShiftId = item.ShiftId;
          existing.IsWorking = item.IsWorking;
          existing.StartTime = item.StartTime;
          existing.EndTime = item.EndTime;
          existing.TotalHours = item.TotalHours;
          existing.ModifiedDate = DateTime.Now;
        }
        else
        {
          DailyWorkingHour obj = new DailyWorkingHour
          {
            CompanyId = item.CompanyId,
            RegionId = item.RegionId,
            UserId = user.UserId,
            EmployeeCode = item.EmployeeCode,
            ShiftId = item.ShiftId,
            DayOfWeek = item.DayOfWeek,
            IsWorking = item.IsWorking,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            TotalHours = item.TotalHours,
            IsActive = true,
            CreatedDate = DateTime.Now
          };

          _context.DailyWorkingHours.Add(obj);
        }
      }

      await _context.SaveChangesAsync();
    }

    // GET
    public async Task<List<DailyWorkingHourDto>> GetAllDailyWorkingHourAsync()
    {
      return await _context.DailyWorkingHours
          .Include(x => x.User)
          .Include(x => x.Shift)
          .Where(x => x.IsActive != false)
          .Select(x => new DailyWorkingHourDto
          {
            DailyWorkingHourId = x.DailyWorkingHourId,
            EmployeeCode = x.EmployeeCode,
            EmployeeName = x.User.FullName,
            ShiftId = x.ShiftId,
            ShiftName = x.Shift.ShiftName,
            DayOfWeek = x.DayOfWeek,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            TotalHours = x.TotalHours,
            IsWorking = x.IsWorking
          })
          .ToListAsync();
    }

    public async Task UpdateDailyWorkingHourAsync(int id, DailyWorkingHourDto dto)
    {
      var entity = await _context.DailyWorkingHours
          .FirstOrDefaultAsync(x => x.DailyWorkingHourId == id && x.IsActive != false);

      if (entity == null)
        throw new Exception("Record not found");

      entity.ShiftId = dto.ShiftId;
      entity.DayOfWeek = dto.DayOfWeek;
      entity.IsWorking = dto.IsWorking;
      entity.StartTime = dto.StartTime;
      entity.EndTime = dto.EndTime;
      entity.TotalHours = dto.TotalHours;
      entity.ModifiedDate = DateTime.Now;

      await _context.SaveChangesAsync();
    }
    public async Task DeleteDailyWorkingHourAsync(int id)
    {
      var entity = await _context.DailyWorkingHours
          .FirstOrDefaultAsync(x => x.DailyWorkingHourId == id);

      if (entity == null)
        throw new Exception("Record not found");

      entity.IsActive = false;
      entity.ModifiedDate = DateTime.Now;

      await _context.SaveChangesAsync();
    }




  }

}
