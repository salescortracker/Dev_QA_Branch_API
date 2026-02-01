using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IDepartmentService
    {
        Task<ApiResponse<IEnumerable<DepartmentDto>>> GetAllAsync();
        Task<ApiResponse<DepartmentDto?>> GetByIdAsync(int id);
        Task<ApiResponse<DepartmentDto>> CreateAsync(CreateUpdateDepartmentDto dto, string createdBy);
        Task<ApiResponse<DepartmentDto>> UpdateAsync(int id, CreateUpdateDepartmentDto dto, string modifiedBy);
        Task<ApiResponse<object>> SoftDeleteAsync(int id, string modifiedBy);
        Task<ApiResponse<(int inserted, int duplicates, int failed)>> BulkInsertAsync(IEnumerable<CreateUpdateDepartmentDto> items, string createdBy);
    }
}
