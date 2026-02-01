using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IDesignationService
    {
        Task<ApiResponse<IEnumerable<DesignationDTO>>> GetAllAsync();
        Task<ApiResponse<DesignationDTO?>> GetByIdAsync(int id);
        Task<ApiResponse<DesignationDTO>> CreateAsync(CreateUpdateDesignationDto dto, int createdBy);
        Task<ApiResponse<DesignationDTO>> UpdateAsync(int id, CreateUpdateDesignationDto dto, int modifiedBy);
        Task<ApiResponse<object>> SoftDeleteAsync(int id, int modifiedBy);
        Task<ApiResponse<(int inserted, int duplicates, int failed)>> BulkInsertAsync(IEnumerable<CreateUpdateDesignationDto> items, int createdBy);

    }
}
