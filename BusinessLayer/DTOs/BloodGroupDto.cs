using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class BloodGroupDto
    {
        public int BloodGroupId { get; set; }
        public string BloodGroupName { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public bool IsActive { get; set; }
    }
}
