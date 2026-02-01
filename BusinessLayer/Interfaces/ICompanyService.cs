using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(object filter);
        Task<CompanyDto> AddCompanyAsync(CompanyDto dto);
        Task<CompanyDto> UpdateCompanyAsync(int id, CompanyDto dto);
        Task<bool> DeleteCompanyAsync(int id);
        Task<IEnumerable<DataAccessLayer.DBContext.Company>> AddCompaniesAsync(List<CompanyDto> dtos);
    }
}
