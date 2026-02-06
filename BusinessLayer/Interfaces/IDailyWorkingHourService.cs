using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
  public interface IDailyWorkingHourService
  {
    Task SaveDailyWorkingHourAsync(List<DailyWorkingHourDto> data);

    Task<List<DailyWorkingHourDto>> GetAllDailyWorkingHourAsync();

    Task UpdateDailyWorkingHourAsync(int id, DailyWorkingHourDto dto);
    Task DeleteDailyWorkingHourAsync(int id);
  }
}
