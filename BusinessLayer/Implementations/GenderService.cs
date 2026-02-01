using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class GenderService:IGenderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<IEnumerable<GenderDto>>> GetAllAsync(int companyId, int regionId)
        {
            //&&            g.CompanyId == companyId &&            g.RegionId == regionId
            try
            {
                var list = await _unitOfWork.Repository<Gender>().FindAsync(g =>
                    !g.IsDeleted );

                var dto = list.Select(g => new GenderDto
                {
                    genderID = g.GenderId,
                    genderName = g.GenderName,
                    companyID = g.CompanyId,
                    regionId = g.RegionId,                   
                    IsActive = g.IsActive
                });

                return new ApiResponse<IEnumerable<GenderDto>>(dto, "Genders retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<GenderDto>>(null!,
                    $"Failed to get genders. {ex.Message}", false);
            }
        }
        public async Task<IEnumerable<GenderDto>> GetAllGendersAsync()
        {
            var genders = await _unitOfWork.Repository<Gender>().GetAllAsync();
            return genders.Select(g => MapToDto(g));
        }
        

        public async Task<GenderDto?> GetGenderByIdAsync(int id)
        {
            var gender = await _unitOfWork.Repository<Gender>().GetByIdAsync(id);
            return gender == null ? null : MapToDto(gender);
        }

        public async Task<IEnumerable<GenderDto>> SearchGenderAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var allGenders = await _unitOfWork.Repository<Gender>().GetAllAsync();
            var query = allGenders.AsQueryable();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(filter);

                if (value != null)
                {
                    switch (name)
                    {
                        case nameof(Gender.GenderName):
                            query = query.Where(g => g.GenderName != null &&
                                                     g.GenderName.Contains(value.ToString()!));
                            break;

                        case nameof(Gender.IsActive):
                            bool active = Convert.ToBoolean(value);
                            query = query.Where(g => g.IsActive == active);
                            break;

                        case nameof(Gender.CompanyId):
                            int company = Convert.ToInt32(value);
                            query = query.Where(g => g.CompanyId == company);
                            break;

                        case nameof(Gender.RegionId):
                            int region = Convert.ToInt32(value);
                            query = query.Where(g => g.RegionId == region);
                            break;
                    }
                }
            }

            return query.Select(g => MapToDto(g)).ToList();
        }

        public async Task<GenderDto> AddGenderAsync(GenderDto dto)
        {
            var entity = new Gender
            {
                GenderName = dto.genderName,
                IsActive = dto.IsActive,
                CompanyId = dto.companyID,
                RegionId = dto.regionId                
            };

            await _unitOfWork.Repository<Gender>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<GenderDto> UpdateGenderAsync(GenderDto dto)
        {
            var entity = await _unitOfWork.Repository<Gender>().GetByIdAsync(dto.genderID);
            if (entity == null) throw new Exception("Gender not found");

            entity.GenderName = dto.genderName;
            entity.IsActive = dto.IsActive;
            entity.CompanyId = dto.companyID;
            entity.RegionId = dto.regionId;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Gender>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteGenderAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Gender>().GetByIdAsync(id);
            if (entity == null)
                return false;

            _unitOfWork.Repository<Gender>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<Gender>> AddGendersAsync(List<GenderDto> dtos)
        {
            var entities = dtos.Select(dto => new Gender
            {
                GenderName = dto.genderName,
                IsActive = dto.IsActive,
                CompanyId = dto.companyID,
                RegionId = dto.regionId               
            }).ToList();

            await _unitOfWork.Repository<Gender>().AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();

            return entities;
        }

        private GenderDto MapToDto(Gender g)
        {
            return new GenderDto
            {
                genderID = g.GenderId,
                genderName = g.GenderName,
                IsActive = g.IsActive,
                companyID = g.CompanyId,
                regionId = g.RegionId
            };
        }
    }
}
