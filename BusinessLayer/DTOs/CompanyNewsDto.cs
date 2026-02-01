using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CompanyNewsDto
    {
        public int NewsId { get; set; }          // for edit/update

        public string Title { get; set; }
        public string Category { get; set; }     // roleName from dropdown
        public string Description { get; set; }

        // 🔹 File upload
        public IFormFile? UploadFile { get; set; }
        public string? AttachmentName { get; set; }
        public string? AttachmentPath { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        // 🔹 AUDIT
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public DateTime DisplayDate { get; set; }
    }
}
