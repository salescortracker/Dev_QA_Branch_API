using BusinessLayer.Common;
using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;

namespace BusinessLayer.Interfaces
{
    public interface IBloodGroupService
    {
        Task<ApiResponse<IEnumerable<BloodGroupDto>>> GetAllbloodgroupAsync(
            int companyId, int regionId);
        Task<ApiResponse<BloodGroupDto?>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<BloodGroupDto>>> SearchbloodgroupAsync(
            BloodGroupDto filter);
        Task<ApiResponse<BloodGroupDto>> AddbloodgroupAsync(BloodGroupDto dto);
        Task<ApiResponse<BloodGroupDto>> UpdatebloodgroupAsync(BloodGroupDto dto);
        Task<ApiResponse<bool>> DeletebloodgroupAsync(int id);
    }
}
