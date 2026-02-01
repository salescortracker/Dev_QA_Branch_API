using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EmployeeResignationService: IEmployeeResignationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeResignationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===================== OLD REQUIRED METHOD =====================
        public async Task<IEnumerable<EmployeeResignationDto>> GetAllResignationsAsync()
        {
            var list = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();
            return list.Select(MapToDto);
        }

        // ===================== OLD REQUIRED METHOD =====================
        public async Task<IEnumerable<EmployeeResignationDto>> SearchResignationsAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var all = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();
            var query = all.AsQueryable();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(filter);
                if (value == null) continue;

                switch (name)
                {
                    case nameof(EmployeeResignation.EmployeeId):
                        query = query.Where(x => x.EmployeeId != null && x.EmployeeId.Contains(value.ToString()!));
                        break;

                    case nameof(EmployeeResignation.ResignationType):
                        query = query.Where(x => x.ResignationType != null && x.ResignationType.Contains(value.ToString()!));
                        break;

                    case nameof(EmployeeResignation.Status):
                        query = query.Where(x => x.Status != null &&
                            x.Status.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase));
                        break;

                    case nameof(EmployeeResignation.CompanyId):
                        query = query.Where(x => x.CompanyId == Convert.ToInt32(value));
                        break;

                    case nameof(EmployeeResignation.RegionId):
                        query = query.Where(x => x.RegionId == Convert.ToInt32(value));
                        break;
                }
            }

            return query.Select(MapToDto).ToList();
        }

        // ===================== OLD REQUIRED METHOD (Bulk Insert) =====================
        public async Task<IEnumerable<EmployeeResignation>> AddMultipleResignationsAsync(List<EmployeeResignationDto> dtos)
        {
            var entities = dtos.Select(dto => new EmployeeResignation
            {
                EmployeeId = dto.EmployeeId,
                ResignationType = dto.ResignationType,
                NoticePeriod = dto.NoticePeriod,
                LastWorkingDay = dto.LastWorkingDay,
                ResignationReason = dto.ResignationReason,
                Status = dto.Status,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                UserId = dto.UserId
            }).ToList();

            await _unitOfWork.Repository<EmployeeResignation>().AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();

            return entities;
        }

        // ===================== NEW FILTERED GET ALL =====================
        public async Task<IEnumerable<EmployeeResignationDto>> GetResignationsByCompanyRegionAsync(int companyId, int regionId)
        {
            var list = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();
            return list
                .Where(e => e.CompanyId == companyId && e.RegionId == regionId)
                .Select(MapToDto)
                .ToList();
        }

        // ===================== NEW FILTERED GET BY ID =====================
        public async Task<EmployeeResignationDto?> GetResignationByIdFilteredAsync(int id, int companyId, int regionId)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);

            if (entity == null) return null;
            if (entity.CompanyId != companyId || entity.RegionId != regionId) return null;

            return MapToDto(entity);
        }

        // ===================== GET BY ID =====================
        public async Task<EmployeeResignationDto?> GetResignationByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        // ===================== CREATE =====================
        public async Task<EmployeeResignationDto> AddResignationAsync(EmployeeResignationDto dto)
        {
            var entity = new EmployeeResignation
            {
                EmployeeId = dto.UserId.ToString(),
                ResignationType = dto.ResignationType,
                NoticePeriod = dto.NoticePeriod,
                LastWorkingDay = dto.LastWorkingDay,
                ResignationReason = dto.ResignationReason,
                Status = dto.Status ?? "Pending",
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                UserId = dto.UserId,
               
            };

            await _unitOfWork.Repository<EmployeeResignation>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        // ===================== UPDATE =====================
        public async Task<EmployeeResignationDto> UpdateResignationAsync(int id, EmployeeResignationDto dto)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);
            if (entity == null) throw new Exception("Record not found.");

            if (entity.CompanyId != dto.CompanyId || entity.RegionId != dto.RegionId)
                throw new Exception("Not allowed to update this record.");

            entity.EmployeeId = dto.EmployeeId;
            entity.ResignationType = dto.ResignationType;
            entity.NoticePeriod = dto.NoticePeriod;
            entity.LastWorkingDay = dto.LastWorkingDay;
            entity.ResignationReason = dto.ResignationReason;
            entity.Status = dto.Status;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.ModifiedAt = DateTime.UtcNow;
            if (entity.Status == "Approved by Manager")
                entity.ManagerApprovedDate = DateTime.UtcNow;
            if (entity.Status == "Rejected by Manager")
                entity.ManagerRejectedDate = DateTime.UtcNow;
                _unitOfWork.Repository<EmployeeResignation>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        // ===================== DELETE =====================
        public async Task<bool> DeleteResignationAsync(int id, int companyId, int regionId)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);

            if (entity == null) return false;
            if (entity.CompanyId != companyId || entity.RegionId != regionId) return false;

            _unitOfWork.Repository<EmployeeResignation>().Remove(entity);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ===================== MAPPER =====================
        private EmployeeResignationDto MapToDto(EmployeeResignation e)
        {
            return new EmployeeResignationDto
            {
                ResignationId = e.ResignationId,
                EmployeeId = e.EmployeeId,
                ResignationType = e.ResignationType,
                NoticePeriod = e.NoticePeriod,
                LastWorkingDay = e.LastWorkingDay,
                ResignationReason = e.ResignationReason,
                Status = e.Status,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt,
                ModifiedBy = e.ModifiedBy,
                ModifiedAt = e.ModifiedAt,
                CompanyId = e.CompanyId,
                RegionId = e.RegionId,
                UserId = e.UserId,
               
            };
        }
        public async Task<bool> UpdateResignationStatusAsync(
        int resignationId,
        int companyId,
        int regionId,
        string status,
        bool isManagerApprove = false,
        bool isManagerReject = false,
        bool isHRApprove = false,
        bool isHRReject = false
       )
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(resignationId);

            if (entity == null) return false;
            if (entity.CompanyId != companyId || entity.RegionId != regionId) return false;

            entity.Status = status;
            entity.ModifiedAt = DateTime.UtcNow;

            if (isManagerApprove) entity.ManagerApprovedDate = DateTime.UtcNow;
            if (isManagerReject) entity.ManagerRejectedDate = DateTime.UtcNow;

            if (isHRApprove) entity.HrApprovedDate = DateTime.UtcNow;
            if (isHRReject) entity.HrRejectedDate = DateTime.UtcNow;


            _unitOfWork.Repository<EmployeeResignation>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
