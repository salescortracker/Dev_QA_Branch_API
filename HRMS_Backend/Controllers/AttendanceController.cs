using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IShiftAllocationService _shiftAllocationService;
        private readonly IClockInOutService _clockInOutService;
        private readonly ITimesheetService _timesheetService;
        private readonly IMissedPunchService _service;
    private readonly IDailyWorkingHourService _dailyWorkingHourService;
        public AttendanceController(IShiftAllocationService shiftAllocationService, IDailyWorkingHourService dailyWorkingHourService,IClockInOutService clockInOutService, ITimesheetService timesheetService, IMissedPunchService service)
        {
            
            _shiftAllocationService = shiftAllocationService;
            _clockInOutService = clockInOutService;
            _timesheetService = timesheetService;
            _service = service;
          _dailyWorkingHourService = dailyWorkingHourService;

        }
        #region ShiftAllocation

        [HttpGet("GetAllShifts")]
        public async Task<IActionResult> GetAllShifts()
        {
            var data = await _shiftAllocationService.GetAllShiftsAsync();
            return Ok(data);
        }

        [HttpGet("GetShiftById/{shiftId}")]
        public async Task<IActionResult> GetShiftById(int shiftId)
        {
            var result = await _shiftAllocationService.GetShiftByIdAsync(shiftId);
            if (result == null) return NotFound("Shift not found");
            return Ok(result);
        }

        [HttpPost("AddShift")]
        public async Task<IActionResult> AddShift([FromBody] ShiftMasterDto dto)
        {
            var status = await _shiftAllocationService.AddShiftAsync(dto);
            return status ? Ok("Shift added successfully") : BadRequest("Failed to add shift");
        }

        [HttpPut("UpdateShift")]
        public async Task<IActionResult> UpdateShift([FromBody] ShiftMasterDto dto)
        {
            var status = await _shiftAllocationService.UpdateShiftAsync(dto);
            return status ? Ok("Shift updated successfully") : NotFound("Shift not found");
        }

        [HttpDelete("DeleteShift/{shiftId}")]
        public async Task<IActionResult> DeleteShift(int shiftId)
        {
            var status = await _shiftAllocationService.DeleteShiftAsync(shiftId);
            return status ? Ok("Shift deleted successfully") : NotFound("Shift not found");
        }

        [HttpPut("ActivateShift/{shiftId}")]
        public async Task<IActionResult> ActivateShift(int shiftId)
        {
            var status = await _shiftAllocationService.ActivateShiftAsync(shiftId);
            return status ? Ok("Shift activated") : NotFound("Shift not found");
        }

        [HttpPut("DeactivateShift/{shiftId}")]
        public async Task<IActionResult> DeactivateShift(int shiftId)
        {
            var status = await _shiftAllocationService.DeactivateShiftAsync(shiftId);
            return status ? Ok("Shift deactivated") : NotFound("Shift not found");
        }


        // ===========================================================
        //                  SHIFT ALLOCATION API
        // ===========================================================

        [HttpGet("GetAllAllocations")]
        public async Task<IActionResult> GetAllAllocations()
        {
            var list = await _shiftAllocationService.GetAllAllocationsAsync();
            return Ok(list);
        }

        [HttpGet("GetAllocationById/{id}")]
        public async Task<IActionResult> GetAllocationById(int id)
        {
            var result = await _shiftAllocationService.GetAllocationByIdAsync(id);
            if (result == null) return NotFound("Allocation not found");
            return Ok(result);
        }

        [HttpPost("AllocateShift")]
        public async Task<IActionResult> AllocateShift([FromBody] ShiftAllocationDto dto)
        {
            var status = await _shiftAllocationService.AllocateShiftAsync(dto);
            return   Ok(status) ;
        }

        [HttpPost("UpdateAllocation")]
        public async Task<IActionResult> UpdateAllocation([FromBody] ShiftAllocationDto dto)
        {
            var status = await _shiftAllocationService.UpdateAllocationAsync(dto);
            return status ? Ok("Allocation updated successfully") : NotFound("Allocation not found");
        }

        [HttpDelete("DeleteAllocation/{id}")]
        public async Task<IActionResult> DeleteAllocation(int id)
        {
            var status = await _shiftAllocationService.DeleteAllocationAsync(id);
            return status ? Ok("Allocation deleted") : NotFound("Allocation not found");
        }





    #endregion

        #region
        // 🔹 GET: api/ClockInOut
        [HttpGet("GetclockinoutAll")]
            public async Task<IActionResult> GetclockinoutAll()
            {
                var result = await _clockInOutService.GetAllAsync();
                return Ok(result);
            }

            // 🔹 GET: api/ClockInOut/5
            [HttpGet("GetclockinoutById{id:int}")]
            public async Task<IActionResult> GetclockinoutById(int id)
            {
                var result = await _clockInOutService.GetByIdAsync(id);

                if (result == null)
                    return NotFound(new { message = "Record not found" });

                return Ok(result);
            }

            // 🔹 GET: api/ClockInOut/today
            [HttpGet("GetTodayByEmployee")]
            public async Task<IActionResult> GetTodayByEmployee(
                [FromQuery] string employeeCode,
                [FromQuery] int companyId,
                [FromQuery] int regionId)
            {
                var result = await _clockInOutService
                    .GetTodayByEmployeeAsync(employeeCode, companyId, regionId);

                return Ok(result);
            }

            // 🔹 POST: api/ClockInOut
            [HttpPost("AddclockinOut")]
            public async Task<IActionResult> AddclockinOut(
                [FromBody] ClockInOutCreateDto dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 🔐 Example: get logged-in userId
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                var result = await _clockInOutService.AddAsync(dto, userId);

                return Ok(result);
            }

            // 🔹 DELETE: api/ClockInOut/5
            [HttpPost("DeleteClockinOut")]
            public async Task<IActionResult> DeleteClockinOut([FromQuery]int id)
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                var success = await _clockInOutService.DeleteAsync(id, userId);

                if (!success)
                    return NotFound(new { message = "Record not found" });

                return Ok(new { message = "Deleted successfully" });
            }
            [HttpGet("ShiftallocationName/{employeeCode}")]
            public async Task<IActionResult> GetEmployeeShift(string employeeCode)
            {
                var result = await _shiftAllocationService.GetEmployeeShiftByEmployeeCodeAsync(employeeCode);

                if (result == null)
                    return NotFound(new { message = "Shift not allocated for this employee" });

                return Ok(result);
            }

            [HttpGet("GetLoggedInUser/{userId}")]
            public async Task<IActionResult> GetLoggedInUser(int userId)
            {
                var data = await _timesheetService.GetLoggedInUserAsync(userId);
                return Ok(data);
            }
            [HttpPost("SaveTimesheet")]
            public async Task<IActionResult> SaveTimesheet([FromForm] TimesheetRequestDto dto)
            {
                string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string path = Path.Combine(root, "Uploads", "Timesheets");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // 📎 File upload
                if (dto.Attachment != null && dto.Attachment.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}_{dto.Attachment.FileName}";
                    string fullPath = Path.Combine(path, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.Attachment.CopyToAsync(stream);

                    dto.FileName = fileName;
                    dto.FilePath = $"Uploads/Timesheets/{fileName}";
                }

                int id = await _timesheetService.SaveTimesheetAsync(dto);
                return Ok(new { message = "Timesheet saved successfully", timesheetId = id });
            }

            // ✅ USER LISTING
            [HttpGet("GetMyTimesheets/{userId}")]
            public async Task<IActionResult> GetMyTimesheets(int userId)
            {
                var data = await _timesheetService.GetMyTimesheetsAsync(userId);
                return Ok(data);
            }
            [HttpPost("SendSelectedTimesheets")]
            public async Task<IActionResult> SendSelectedTimesheets([FromBody] List<int> ids)
            {
                var result = await _timesheetService.SendSelectedTimesheetsAsync(ids);

                return Ok(new { success = true, message = "Timesheets submitted successfully" });
            }

            [HttpGet("GetManagerTimesheets/{managerUserId}")]
            public async Task<IActionResult> GetManagerTimesheets(int managerUserId)
            {
                var result = await _timesheetService.GetTimesheetsForManagerAsync(managerUserId);
                return Ok(result);
            }

            [HttpGet("GetTimesheetDetail/{timesheetId}")]
            public async Task<IActionResult> GetTimesheetDetail(int timesheetId)
            {
                var result = await _timesheetService.GetTimesheetDetailAsync(timesheetId);
                return Ok(result);
            }
            [HttpPost("ApproveTimesheets")]
            public async Task<IActionResult> ApproveTimesheets([FromBody] ApproveTimesheetRequestDto dto)
            {
                var result = await _timesheetService.ApproveTimesheetsAsync(dto.Ids, dto.Comments);
                return Ok(new { success = result });
            }

            [HttpPost("RejectTimesheets")]
            public async Task<IActionResult> RejectTimesheets([FromBody] ApproveTimesheetRequestDto dto)
            {
                var result = await _timesheetService.RejectTimesheetsAsync(dto.Ids, dto.Comments);
                return Ok(new { success = result });
            }
        #endregion
        #region missedpunchrequests
        [HttpPost("createmissedpunchrequest")]
            public async Task<IActionResult> CreateMissedPunchRequest(
            CreateMissedPunchRequestDto dto)
            {
                var result = await _service.CreateMissedPunchRequest(dto);
                return Ok(result);
            }

            [HttpGet("getmissedpunchrequest")]
            public async Task<IActionResult> GetMissedPunchRequest(
                int companyId, int? regionId)
            {
                return Ok(await _service.GetMissedPunchRequest(companyId, regionId));
            }

            [HttpGet("getapprovalmissedpunchrequest")]
            public async Task<IActionResult> GetApprovalMissedPunchRequest(
                int companyId, int? regionId, int managerId)
            {
                var result=await _service.GetApprovalMissedPunchRequest(
                    companyId, regionId, managerId);
                return Ok(result);
            }

            [HttpPut("updatemissedpunch")]
            public async Task<IActionResult> UpdateMissedPunch(
                UpdateMissedPunchDto dto)
            {
                var success = await _service.UpdateMissedPunch(dto);
                if (!success)
                    return BadRequest("Record not found or already processed.");

                return Ok(new { message = "Missed punch updated successfully" });
            }

            [HttpPut("bulkapproverejectpunch")]
            public async Task<IActionResult> BulkApproveRejectPunch(
                BulkApproveRejectPunchDto dto)
            {
                var count = await _service.BulkApproveRejectPunch(dto);

                if (count == 0)
                    return BadRequest("No pending records found.");

                return Ok(new
                {
                    message = $"{count} records {dto.Status.ToLower()} successfully"
                });
            }
    #endregion


    #region DailyWorkingHours

        [HttpPost("saveDailyWorkingHour")]
        public async Task<IActionResult> SaveDailyWorkingHour(
          [FromBody] List<DailyWorkingHourDto> data)
        {
          if (!ModelState.IsValid)  
            return BadRequest(ModelState);

          await _dailyWorkingHourService.SaveDailyWorkingHourAsync(data);
          return Ok("Saved Successfully");
        }


      [HttpGet("get-all-DailyWorkingHour")]
        public async Task<IActionResult> GetAllDailyWorkingHour()
        {
          var result = await _dailyWorkingHourService.GetAllDailyWorkingHourAsync();
          return Ok(result);
        }

      //[HttpDelete("deleteDailyWorkingHour")]
      //public async Task<IActionResult> DeleteDailyWorkingHours()
      //{

      //}
        [HttpPut("update-DailyWorkingHour/{id}")]
        public async Task<IActionResult> UpdateDailyWorkingHour(int id,[FromBody] DailyWorkingHourDto dto)
        {
          await _dailyWorkingHourService.UpdateDailyWorkingHourAsync(id, dto);
          return Ok("Updated Successfully");
        }

        [HttpDelete("delete-DailyWorkingHour/{id}")]
        public async Task<IActionResult> DeleteDailyWorkingHour(int id)
        {
          await _dailyWorkingHourService.DeleteDailyWorkingHourAsync(id);
          return Ok("Deleted Successfully");
        }


      #endregion

  }
}
