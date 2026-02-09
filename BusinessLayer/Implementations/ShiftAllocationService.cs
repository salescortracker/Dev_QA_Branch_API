using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class ShiftAllocationService: IShiftAllocationService
    {
        private readonly HRMSContext _context;

        public ShiftAllocationService(HRMSContext context)
        {
            _context = context;
        }

        // ======================================================
        //              SHIFT MASTER SERVICES
        // ======================================================

        public async Task<IEnumerable<ShiftMasterDto>> GetAllShiftsAsync()
        {
            return await _context.ShiftMasters
                .Select(x => new ShiftMasterDto
                {
                    ShiftID = x.ShiftId,
                    ShiftName = x.ShiftName,
                    ShiftStartTime = x.ShiftStartTime.ToTimeSpan(),
                    ShiftEndTime = x.ShiftEndTime.ToTimeSpan(),
                    GraceTime = x.GraceTime,
                  OvertimeAllowed = x.OvertimeAllowed,
                  IsActive = x.IsActive,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    ModifiedAt = x.ModifiedAt,
                    ModifiedBy = x.ModifiedBy
                }).ToListAsync();
        }

        public async Task<ShiftMasterDto?> GetShiftByIdAsync(int shiftId)
        {
            return await _context.ShiftMasters
                .Where(x => x.ShiftId == shiftId)
                .Select(x => new ShiftMasterDto
                {
                    ShiftID = x.ShiftId,
                    ShiftName = x.ShiftName,
                    ShiftStartTime = x.ShiftStartTime.ToTimeSpan(),
                    ShiftEndTime = x.ShiftEndTime.ToTimeSpan(),
                    GraceTime = x.GraceTime,
                    IsActive = x.IsActive,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    ModifiedAt = x.ModifiedAt,
                    ModifiedBy = x.ModifiedBy
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> AddShiftAsync(ShiftMasterDto dto)
        {
            var entity = new ShiftMaster
            {
                ShiftName = dto.ShiftName,
                ShiftStartTime = TimeOnly.FromTimeSpan(dto.ShiftStartTime),
                ShiftEndTime = TimeOnly.FromTimeSpan(dto.ShiftEndTime),
                GraceTime = dto.GraceTime,
                OvertimeAllowed = dto.OvertimeAllowed, 
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = dto.CreatedBy
            };

            _context.ShiftMasters.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateShiftAsync(ShiftMasterDto dto)
        {
            var entity = await _context.ShiftMasters.FindAsync(dto.ShiftID);
            if (entity == null) return false;

            entity.ShiftName = dto.ShiftName;
            entity.ShiftStartTime = TimeOnly.FromTimeSpan(dto.ShiftStartTime);
            entity.ShiftEndTime = TimeOnly.FromTimeSpan(dto.ShiftEndTime);
            entity.GraceTime = dto.GraceTime;
            entity.OvertimeAllowed = dto.OvertimeAllowed;  // ✅ ADD

      entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = dto.ModifiedBy;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteShiftAsync(int shiftId)
        {
            var entity = await _context.ShiftMasters.FindAsync(shiftId);
            if (entity == null) return false;

            _context.ShiftMasters.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ActivateShiftAsync(int shiftId)
        {
            var entity = await _context.ShiftMasters.FindAsync(shiftId);
            if (entity == null) return false;

            entity.IsActive = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateShiftAsync(int shiftId)
        {
            var entity = await _context.ShiftMasters.FindAsync(shiftId);
            if (entity == null) return false;

            entity.IsActive = false;
            return await _context.SaveChangesAsync() > 0;
        }

        // ======================================================
        //              SHIFT ALLOCATION SERVICES
        // ======================================================

        public async Task<IEnumerable<ShiftAllocationDto>> GetAllAllocationsAsync()
        {
            return await (
                from sa in _context.ShiftAllocations

                join u in _context.Users
                    on sa.UserId equals u.UserId into userGroup
                from u in userGroup.DefaultIfEmpty()

                join sm in _context.ShiftMasters
                    on sa.ShiftId equals sm.ShiftId into shiftGroup
                from sm in shiftGroup.DefaultIfEmpty()

                select new ShiftAllocationDto
                {
                    ShiftAllocationId = sa.ShiftAllocationId,
                    UserID = sa.UserId,
                    EmployeeCode = sa != null ? sa.EmployeeCode : "",
                    FullName = sa != null ? sa.FullName : "",
                    CompanyID = sa.CompanyId,
                    RegionID = sa.RegionId,
                    ShiftID = sa.ShiftId ?? 0,
                    ShiftName = sm != null ? sm.ShiftName : "",

                    StartDate = sa.StartDate,
                    EndDate = sa.EndDate,

                    IsActive = sa.IsActive,
                    CreatedBy = sa.CreatedBy,
                    CreatedDate = sa.CreatedDate,
                    ModifiedBy = sa.ModifiedBy,
                    ModifiedDate = sa.ModifiedDate
                }
            ).ToListAsync();
        }



        public async Task<ShiftAllocationDto?> GetAllocationByIdAsync(int id)
        {
            return await (
                from sa in _context.ShiftAllocations
                join u in _context.Users on sa.UserId equals u.UserId
                join sm in _context.ShiftMasters on sa.ShiftId equals sm.ShiftId
                where sa.ShiftAllocationId == id
                select new ShiftAllocationDto
                {
                    ShiftAllocationId = sa.ShiftAllocationId,
                    UserID = sa.UserId,
                    EmployeeCode = u.EmployeeCode,
                    FullName = u.FullName,
                    CompanyID = sa.CompanyId,
                    RegionID = sa.RegionId,
                    ShiftID = sa.ShiftId ?? 0,
                    ShiftName = sm.ShiftName,
                    StartDate = sa.StartDate,
                    EndDate = sa.EndDate,
                    IsActive = sa.IsActive,
                    CreatedBy = sa.CreatedBy,
                    CreatedDate = sa.CreatedDate,
                    ModifiedBy = sa.ModifiedBy,
                    ModifiedDate = sa.ModifiedDate
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<bool> AllocateShiftAsync(ShiftAllocationDto dto)
        {
            var entity = new ShiftAllocation
            {
                UserId = dto.UserID,
                ShiftId = dto.ShiftID,
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                EmployeeCode = dto.EmployeeCode,
                FullName = dto.FullName,
                ShiftName = dto.ShiftName,

                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate
            };

            _context.ShiftAllocations.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAllocationAsync(ShiftAllocationDto dto)
        {
            var entity = await _context.ShiftAllocations.FindAsync(dto.ShiftAllocationId);
            if (entity == null) return false;

            entity.UserId = dto.UserID;
            entity.FullName = dto.FullName;
            entity.EmployeeCode = dto.EmployeeCode;
            entity.ShiftId = dto.ShiftID;  // FIXED
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.ModifiedDate = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAllocationAsync(int id)
        {
            var entity = await _context.ShiftAllocations.FindAsync(id);
            if (entity == null) return false;

            _context.ShiftAllocations.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ======================================================
        //              USER DETAILS
        // ======================================================

        public async Task<UserReadDto?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Where(x => x.UserId == userId)
                .Select(x => new UserReadDto
                {
                    UserID = x.UserId,
                    EmployeeCode = x.EmployeeCode,
                    FullName = x.FullName,
                    Email = x.Email,
                    CompanyID = x.CompanyId,    // Added
                    RegionID = x.RegionId,
                    Status = x.Status,
                    RoleName = x.RoleId.ToString(),
                    CreatedDate = (DateTime)x.CreatedDate
                }).FirstOrDefaultAsync();
        }
        public async Task<EmployeeShiftDto?> GetEmployeeShiftByEmployeeCodeAsync(string employeeCode)
        {
            return await (
                from s in _context.ShiftAllocations
                join sm in _context.ShiftMasters
                    on s.ShiftId equals sm.ShiftId
                where s.EmployeeCode == employeeCode
                select new EmployeeShiftDto
                {
                    ShiftName = sm.ShiftName,
                    ShiftStartTime = sm.ShiftStartTime,
                    ShiftEndTime = sm.ShiftEndTime
                }
            ).FirstOrDefaultAsync();
        }
    }
}
