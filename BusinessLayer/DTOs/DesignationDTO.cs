using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class DesignationDTO
    {
        public int DesignationID { get; set; }
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public int? DepartmentID { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class CreateUpdateDesignationDto
    {
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        [Required, MaxLength(100)]
        public string DesignationName { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
