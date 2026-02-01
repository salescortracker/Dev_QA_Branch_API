using BusinessLayer.Common;
using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class BloodGroupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BloodGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<IEnumerable<BloodGroupDto>> GetAllAsync()
        //{
        //    var data = await _unitOfWork.Repository<BloodGroup>().GetAllAsync();
        //    return data.Select(MapToDto).ToList();
        //}

        //public async Task<BloodGroupDto?> GetByIdAsync(int id)
        //{
        //    var entity = await _unitOfWork.Repository<BloodGroup>().GetByIdAsync(id);
        //    return entity == null ? null : MapToDto(entity);
        //}

        //public async Task<IEnumerable<BloodGroupDto>> SearchAsync(object filter)
        //{
        //    var props = filter.GetType().GetProperties();
        //    var all = (await _unitOfWork.Repository<BloodGroup>().GetAllAsync()).AsQueryable();

        //    foreach (var prop in props)
        //    {
        //        var name = prop.Name;
        //        var value = prop.GetValue(filter);

        //        if (value != null)
        //        {
        //            switch (name)
        //            {
        //                case nameof(BloodGroup.BloodGroupName):
        //                    all = all.Where(x => x.BloodGroupName.Contains(value.ToString()!));
        //                    break;
        //                case nameof(BloodGroup.CompanyId):
        //                    all = all.Where(x => x.CompanyId == (int)value);
        //                    break;
        //                case nameof(BloodGroup.RegionId):
        //                    all = all.Where(x => x.RegionId == (int)value);
        //                    break;
        //                case nameof(BloodGroup.IsActive):
        //                    all = all.Where(x => x.IsActive == (bool)value);
        //                    break;
        //            }
        //        }
        //    }

        //    return all.Select(MapToDto).ToList();
        //}

        //public async Task<BloodGroupDto> AddAsync(BloodGroupDto dto)
        //{
        //    var entity = new BloodGroup
        //    {
        //        BloodGroupName = dto.BloodGroupName,
        //        CompanyId = dto.CompanyId,
        //        RegionId = dto.RegionId,
        //        IsActive = dto.IsActive,

        //    };

        //    await _unitOfWork.Repository<BloodGroup>().AddAsync(entity);
        //    await _unitOfWork.CompleteAsync();
        //    return MapToDto(entity);
        //}

        //public async Task<BloodGroupDto> UpdateAsync(BloodGroupDto dto)
        //{
        //    var entity = await _unitOfWork.Repository<BloodGroup>().GetByIdAsync(dto.BloodGroupId);
        //    if (entity == null) throw new Exception("Blood group not found");

        //    entity.BloodGroupName = dto.BloodGroupName;
        //    entity.CompanyId = dto.CompanyId;
        //    entity.RegionId = dto.RegionId;
        //    entity.IsActive = dto.IsActive;

        //    _unitOfWork.Repository<BloodGroup>().Update(entity);
        //    await _unitOfWork.CompleteAsync();

        //    return MapToDto(entity);
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    var entity = await _unitOfWork.Repository<BloodGroup>().GetByIdAsync(id);
        //    if (entity == null) return false;

        //    _unitOfWork.Repository<BloodGroup>().Remove(entity);
        //    await _unitOfWork.CompleteAsync();
        //    return true;
        //}

        //private BloodGroupDto MapToDto(BloodGroup bg)
        //{
        //    return new BloodGroupDto
        //    {
        //        BloodGroupId = bg.BloodGroupId,
        //        BloodGroupName = bg.BloodGroupName,
        //        CompanyId = bg.CompanyId,
        //        RegionId = bg.RegionId,
        //        IsActive = bg.IsActive
        //    };
        //}

        // ============================
        // GET ALL (Company + Region)
        // ============================
        public async Task<ApiResponse<IEnumerable<BloodGroupDto>>> GetAllbloodgroupAsync(
            int companyId, int regionId)
        {
            try
            {
                var bloodGroups = await _unitOfWork.Repository<BloodGroup>()
                    .FindAsync(bg =>
                        !bg.IsDeleted &&
                        bg.CompanyId == companyId &&
                        bg.RegionId == regionId);

                var dto = bloodGroups.Select(MapToDto);

                return new ApiResponse<IEnumerable<BloodGroupDto>>(
                    dto, "Blood groups retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<BloodGroupDto>>(
                    null!, $"Failed to get blood groups. {ex.Message}", false);
            }
        }
        // ============================
        // GET BY ID
        // ============================
        public async Task<ApiResponse<BloodGroupDto?>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<BloodGroup>()
                .GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<BloodGroupDto?>(
                    null, "Blood group not found", false);

            return new ApiResponse<BloodGroupDto?>(
                MapToDto(entity), "Blood group retrieved successfully");
        }

        // ============================
        // SEARCH
        // ============================
        public async Task<ApiResponse<IEnumerable<BloodGroupDto>>> SearchbloodgroupAsync(
            BloodGroupDto filter)
        {
            var query = (await _unitOfWork.Repository<BloodGroup>()
                .FindAsync(bg => !bg.IsDeleted))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.BloodGroupName))
                query = query.Where(bg =>
                    bg.BloodGroupName!.Contains(filter.BloodGroupName));

            if (filter.IsActive)
                query = query.Where(bg => bg.IsActive == filter.IsActive);

            if (filter.CompanyId!=null)
                query = query.Where(bg => bg.CompanyId == filter.CompanyId);

            if (filter.RegionId!=null)
                query = query.Where(bg => bg.RegionId == filter.RegionId);

            return new ApiResponse<IEnumerable<BloodGroupDto>>(
                query.Select(MapToDto).ToList(),
                "Search completed successfully");
        }

        // ============================
        // ADD
        // ============================
        public async Task<ApiResponse<BloodGroupDto>> AddbloodgroupAsync(BloodGroupDto dto)
        {
            var entity = new BloodGroup
            {
                BloodGroupName = dto.BloodGroupName,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                IsActive = dto.IsActive
            };

            await _unitOfWork.Repository<BloodGroup>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<BloodGroupDto>(
                MapToDto(entity), "Blood group added successfully");
        }

        // ============================
        // UPDATE
        // ============================
        public async Task<ApiResponse<BloodGroupDto>> UpdatebloodgroupAsync(BloodGroupDto dto)
        {
            var entity = await _unitOfWork.Repository<BloodGroup>()
                .GetByIdAsync(dto.BloodGroupId);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<BloodGroupDto>(
                    null!, "Blood group not found", false);

            entity.BloodGroupName = dto.BloodGroupName;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<BloodGroup>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<BloodGroupDto>(
                MapToDto(entity), "Blood group updated successfully");
        }

        // ============================
        // DELETE (SOFT)
        // ============================
        public async Task<ApiResponse<bool>> DeletebloodgroupAsync(int id)
        {
            var entity = await _unitOfWork.Repository<BloodGroup>()
                .GetByIdAsync(id);

            if (entity == null)
                return new ApiResponse<bool>(
                    false, "Blood group not found", false);

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<BloodGroup>().Remove(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<bool>(
                true, "Blood group deleted successfully");
        }

        // ============================
        // BULK ADD
        // ============================
        public async Task<ApiResponse<IEnumerable<BloodGroupDto>>> AddbloodgroupRangeAsync(
            List<BloodGroupDto> dtos)
        {
            var entities = dtos.Select(dto => new BloodGroup
            {
                BloodGroupName = dto.BloodGroupName,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                IsActive = dto.IsActive
            }).ToList();

            await _unitOfWork.Repository<BloodGroup>()
                .AddRangeAsync(entities);

            await _unitOfWork.CompleteAsync();

            return new ApiResponse<IEnumerable<BloodGroupDto>>(
                entities.Select(MapToDto),
                "Blood groups added successfully");
        }

        // ============================
        // MAPPER
        // ============================
        private static BloodGroupDto MapToDto(BloodGroup bg)
        {
            return new BloodGroupDto
            {
                BloodGroupId = bg.BloodGroupId,
                BloodGroupName = bg.BloodGroupName,
                CompanyId = bg.CompanyId,
                RegionId = bg.RegionId,
                IsActive = bg.IsActive
            };
        }
    }
}
