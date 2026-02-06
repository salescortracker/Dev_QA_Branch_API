using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
  public class DailyWorkingHourDto
  {
    public int DailyWorkingHourId { get; set; }

    // ===== SAVE + GET =====
    public int CompanyId { get; set; }
    public int RegionId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public int ShiftId { get; set; }

    public byte DayOfWeek { get; set; }

    public bool IsWorking { get; set; }

    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public decimal? TotalHours { get; set; }

    // ===== GET ONLY (OPTIONAL) =====
    public string? EmployeeName { get; set; }
    public string? ShiftName { get; set; }
  }

}
