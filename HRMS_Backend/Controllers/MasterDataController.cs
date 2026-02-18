using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IGenderService _genderService;
        private readonly IadminService _adminService;
        private readonly ILogger<MasterDataController> _logger;
        private readonly IDesignationService _designationService;
        private readonly IKpiCategoryService _kpiCategoryService;
        private readonly IEmployeeMasterService _employeeService;
        private readonly ICertificationTypeService _certificationTypeService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IExpenseCategoryService _expensecategoryservice;
        private readonly IAssetStatusService _assetStatusService;
        private readonly IModeOfStudyService _modeOfStudyService;
        public MasterDataController(IExpenseCategoryService expenseCategoryservice,IDepartmentService service, IDesignationService designationService, IGenderService genderService,IadminService adminService, ILeaveTypeService leaveTypeService,  ILogger<MasterDataController> logger, IKpiCategoryService kpiCategoryService, IEmployeeMasterService employeeService, ICertificationTypeService certificationTypeService, IAssetStatusService assetStatusService, IModeOfStudyService modeOfStudyService)
        {
            _service = service;
            _designationService = designationService;
            _genderService = genderService;
            _adminService= adminService;
            _logger = logger;
            _expensecategoryservice = expenseCategoryservice;
            _leaveTypeService = leaveTypeService;
            _kpiCategoryService = kpiCategoryService;
            _employeeService = employeeService;
            _certificationTypeService = certificationTypeService;
            _assetStatusService = assetStatusService;
            _modeOfStudyService = modeOfStudyService;
        }
        #region Departments
        // ✅ GET ALL (with optional filters later)
        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var result = await _service.GetAllAsync();

                if (result == null )
                    return NotFound(new { success = false, message = "No departments found." });

                return Ok(new { success = true, message = "Departments retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department list.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching department list." });
            }
        }

        // ✅ GET BY ID
        [HttpGet("GetDepartmentsById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Department with ID {id} not found." });

                return Ok(new { success = true, message = "Department details retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching department details." });
            }
        }

        // ✅ CREATE
        [HttpPost("createDepartment")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

            try
            {
                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.CreateAsync(dto, createdBy);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new department.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the department." });
            }
        }

        // ✅ UPDATE
        [HttpPost("updateDepartment/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data." });

            try
            {
                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.UpdateAsync(id, dto, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the department." });
            }
        }

        // ✅ SOFT DELETE
        [HttpDelete("deleteDepartment/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.SoftDeleteAsync(id, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the department." });
            }
        }

        // ✅ BULK INSERT
        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDepartmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No records found to upload." });

            try
            {
                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.BulkInsertAsync(dtos, createdBy);

                return Ok(new { success = true, message = result.Message, insertedCount = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk department upload.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
            }
        }
        #endregion
        #region Designations
        // ✅ GET ALL
        [HttpGet("GetDesignations")]
        public async Task<IActionResult> GetDesignations()
        {
            try
            {
                var result = await _designationService.GetAllAsync();

                if (result == null )
                    return NotFound(new { success = false, message = "No designations found." });

                return Ok(new { success = true, message = "Designations retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching designation list.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching designations." });
            }
        }

        // ✅ GET BY ID
        [HttpGet("GetDesignationById/{id:int}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var result = await _designationService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Designation with ID {id} not found." });

                return Ok(new { success = true, message = "Designation details retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching designation details." });
            }
        }

        // ✅ CREATE
        [HttpPost("CreateDesignation")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

            try
            {
                var createdBy = 1; // 🔒 TODO: Replace with logged-in user later
                var result = await _designationService.CreateAsync(dto, createdBy);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new designation.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the designation." });
            }
        }

        // ✅ UPDATE
        [HttpPost("UpdateDesignation/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data." });

            try
            {
                var modifiedBy =1; // 🔒 TODO: Replace with logged-in user later
                var result = await _designationService.UpdateAsync(id, dto, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the designation." });
            }
        }

        // ✅ SOFT DELETE
        [HttpPost("DeleteDesignation/{id:int}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var modifiedBy = 1; // 🔒 TODO: Replace with JWT user later
                var result = await _designationService.SoftDeleteAsync(id, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the designation." });
            }
        }
        // ✅ BULK INSERT
        [HttpPost("DesignationBulkInsert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDesignationDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No records found to upload." });

            try
            {
                var createdBy = 1; // TODO: Replace with JWT username later
                var result = await _designationService.BulkInsertAsync(dtos, createdBy);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    data = new
                    {
                        inserted = result.Data.inserted,
                        duplicates = result.Data.duplicates,
                        failed = result.Data.failed
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk designation upload.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
            }
        }

        #endregion
        #region Gender
        /// <summary>
        /// Gender Detail Retrieve
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGenderAll")]
        public async Task<IActionResult> GetGenderAll(int companyId,int regionId)
        {
            var result = await _genderService.GetAllAsync(companyId, regionId);

            if (result==null)
                return NotFound("No gender records found.");

            return Ok(result);
        }
        /// <summary>
        /// Retrieve Gender details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetGenderById/{id}")]
        public async Task<IActionResult> GetGenderById(int id)
        {
            var gender = await _genderService.GetGenderByIdAsync(id);
            if (gender == null) return NotFound("Gender not found");
            return Ok(gender);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>

        [HttpPost("GetGendersearch")]
        public async Task<IActionResult> Search([FromBody] object filter)
        {
            return Ok(await _genderService.SearchGenderAsync(filter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("CreateGender")]
        public async Task<IActionResult> CreateGender([FromBody] GenderDto dto)
        {
            var result = await _genderService.AddGenderAsync(dto);
            return Ok(new { message = "Gender created successfully", data = result });
        }

        [HttpPost("UpdateGender")]
        public async Task<IActionResult> UpdateGender([FromBody] GenderDto dto)
        {
            var result = await _genderService.UpdateGenderAsync(dto);
            return Ok(new { message = "Gender updated successfully", data = result });
        }

        [HttpPost("DeleteGender")]
        public async Task<IActionResult> DeleteGender([FromQuery] int id)
        {
            bool success = await _genderService.DeleteGenderAsync(id);
            if (!success) return NotFound("Gender not found");

            return Ok(new { message = "Gender deleted successfully" });
        }
        #endregion
        #region KPICategory
        // =====================================================
        // KPI CATEGORY
        // =====================================================

        // GET ALL KPI CATEGORIES
        [HttpGet("kpi-categories")]
        public async Task<IActionResult> GetKpiCategories()
        {
            var result = await _kpiCategoryService.GetAll();
            return Ok(result);
        }


        // GET KPI CATEGORY BY ID
        [HttpGet("kpi-categories/{id:int}")]
        public async Task<IActionResult> GetKpiCategoryById(int id)
        {
            var result = await _kpiCategoryService.GetByIdAsync(id);
            return Ok(result);
        }

        // CREATE KPI CATEGORY
        [HttpPost("CreateKpiCategory")]
        public async Task<IActionResult> CreateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        {
            var result = await _kpiCategoryService.CreateAsync(dto);
            return Ok(result);
        }

        // UPDATE KPI CATEGORY
        [HttpPost("UpdateKpiCategory")]
        public async Task<IActionResult> UpdateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        {
            var result = await _kpiCategoryService.UpdateAsync(dto);
            return Ok(result);
        }

        // DELETE KPI CATEGORY
        [HttpPost("DeleteKpiCategory")]
        public async Task<IActionResult> DeleteKpiCategory([FromQuery] int id)
        {
            var result = await _kpiCategoryService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        #endregion


        #region ModeOfStudy
        [HttpGet("GetAllModeOfStudy")]
        public async Task<IActionResult> GetAllModeOfStudy([FromQuery] int userId)
        {
            var data = await _modeOfStudyService.GetAllModeOfStudtAsync(userId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModeOfStudy(int id)
        {
            var data = await _modeOfStudyService.GetByIdModeOfStudtAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("CreateModeOfStudy")]
        public async Task<IActionResult> CreateModeOfStudy([FromBody]ModeOfStudyDto dto)
        {
            var result = await _modeOfStudyService.CreateModeOfStudtAsync(dto);
            //return result ? Ok("Created Successfully") : BadRequest();
            return result
    ? Ok(new { success = true, message = "Created Successfully" })
    : BadRequest(new { success = false, message = "Creation Failed" });
        }

        [HttpPut("UpdateModeOfStudy")]
        public async Task<IActionResult> UpdateModeOfStudy([FromBody] ModeOfStudyDto dto)
        {
            var result = await _modeOfStudyService.UpdateModeOfStudtAsync(dto);
            return result ? Ok("Updated Successfully") : NotFound();
        }

        [HttpDelete("DeleteModeOfStudy/{id}")]
        public async Task<IActionResult> DeleteModeOfStudy(int id)
        {
            var result = await _modeOfStudyService.DeleteModeOfStudtAsync(id);
            return result ? Ok("Deleted Successfully") : NotFound();
        }
        #endregion

        //---------------------------------Employee Master Details---------------------------------//
        #region Employee Master Details


        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var data = await _employeeService.GetAllEmployees();
            return Ok(data);
        }

        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeMasterDto dto)
        {
            var data = await _employeeService.CreateEmployee(dto);
            return Ok(data);
        }

        [HttpPost("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeMasterDto dto)
        {
            var data = await _employeeService.UpdateEmployee(id, dto);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee([FromQuery] int id)
        {
            var success = await _employeeService.DeleteEmployee(id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }

        [HttpGet("GetManagers")]
        public async Task<IActionResult> GetManagers()
        {
            var data = await _employeeService.GetManagers();
            return Ok(data);
        }

        #endregion

        //----------------------MY TEAM SECTION----------------------//
        [HttpGet("MyTeam/{managerUserId}")]
        public async Task<IActionResult> GetMyTeam(int managerUserId)
        {
            var tree = await _employeeService.GetMyTeamTreeAsync(managerUserId);
            if (tree == null) return NotFound(new { message = "Manager not found" });
            return Ok(tree);
        }
        // ===================== ASSET STATUS =====================

        /// <summary>
        /// Asset Status CRUD APIs
        /// </summary>
        [HttpGet("asset-status")]
        public async Task<IActionResult> GetAllAssetStatuses(
        [FromQuery] int companyId,
        [FromQuery] int regionId)
        {
            var result = await _assetStatusService.GetAllAsync(companyId, regionId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new asset status
        /// </summary>
        [HttpPost("asset-status")]
        public async Task<IActionResult> CreateAssetStatus([FromBody] AssetStatusDto dto)
        {
            var id = await _assetStatusService.CreateAsync(dto);
            return Ok(id);
        }

        /// <summary>
        /// Updates an existing asset status
        /// </summary>
        [HttpPut("asset-status/{id}")]
        public async Task<IActionResult> UpdateAssetStatus(int id, [FromBody] AssetStatusDto dto)
        {
            dto.AssetStatusId = id;
            var updated = await _assetStatusService.UpdateAsync(dto);
            return updated ? Ok() : NotFound();
        }


        /// <summary>
        /// Deletes (soft delete) an asset status
        /// </summary>
        [HttpDelete("asset-status/{id}")]
        public async Task<IActionResult> DeleteAssetStatus(int id)
        {
            var deleted = await _assetStatusService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }

        #region ===================== CERTIFICATION TYPES =====================

        [HttpGet("certification-types")]
        public async Task<IActionResult> GetCertificationTypes(
            int companyId,
            int regionId)
        {
            var result = await _certificationTypeService
                .GetAllAsync(companyId, regionId);

            return Ok(result!=null?result.Data:result);
        }

        [HttpGet("certification-types/{id:int}")]
        public async Task<IActionResult> GetCertificationTypeById(int id)
        {
            var result = await _certificationTypeService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("CreateCertificationType")]
        public async Task<IActionResult> CreateCertificationType(
            [FromBody] CreateUpdateCertificationTypeDto dto
            )
        {
            var result = await _certificationTypeService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("UpdateCertificationType")]
        public async Task<IActionResult> UpdateCertificationType(
           
            [FromBody] CreateUpdateCertificationTypeDto dto
            )
        {
            var result = await _certificationTypeService
                .UpdateAsync( dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("DeleteCertificationType")]
        public async Task<IActionResult> DeleteCertificationType([FromQuery] int id)
        {
            var result = await _certificationTypeService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("certification-types/bulk")]
        public async Task<IActionResult> BulkInsertCertificationTypes(
            [FromBody] IEnumerable<CreateUpdateCertificationTypeDto> dtos,
            [FromQuery] int createdBy)
        {
            var result = await _certificationTypeService
                .BulkInsertAsync(dtos, createdBy);

            return Ok(result);
        }

        #endregion
        #region LeaveType
        [HttpGet("GetLeaveType")]
        public async Task<IActionResult> GetLeaveType()
        {
            // call service without parameters
            var data = await _leaveTypeService.GetLeaveTypesAsync();
            return Ok(data);
        }
        [HttpGet("GetCRLeaveTypesAsync")]
        public async Task<IActionResult> GetCRLeaveTypesAsync(
    int companyId,
    int regionId)
        {
            var result = await _leaveTypeService.GetCRLeaveTypesAsync(companyId, regionId);
            return Ok(result);
        }

        [HttpPost("CreateLeaveType")]
        public async Task<IActionResult> CreateLeaveType([FromBody] LeaveTypeDto dto)
        {
            var result = await _leaveTypeService.CreateLeaveTypeAsync(dto);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("UpdateLeaveType")]
        public async Task<IActionResult> UpdateLeaveType([FromBody] LeaveTypeDto dto)
        {
            var result = await _leaveTypeService.UpdateLeaveTypeAsync(dto);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("DeleteLeaveType")]
        public async Task<IActionResult> DeleteLeaveType([FromQuery] int id)
        {
            var result = await _leaveTypeService.DeleteLeaveTypeAsync(id);

            if (!result)
                return NotFound("Leave Type not found or already deleted");

            return Ok(new { message = "Leave Type deleted successfully" });
        }



        #endregion
        #region expenseCategory
        [HttpGet("GetexpenseCategoryAll")]
        public async Task<IActionResult> GetexpenseCategoryAll(int companyId, int regionId)
        {
            var result = await _expensecategoryservice.GetAllAsync(companyId, regionId);
            return Ok(result);
        }

        [HttpPost("AddexpenseCategory")]
        public async Task<IActionResult> AddexpenseCategory([FromBody] ExpenseCategoryDto dto)
        {
            var result = await _expensecategoryservice.AddAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateexpenseCategory")]
        public async Task<IActionResult> UpdateexpenseCategory([FromBody] ExpenseCategoryDto dto)
        {
            var result = await _expensecategoryservice.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteexpenseCategory")]
        public async Task<IActionResult> DeleteexpenseCategory([FromQuery] int id)
        {
            var result = await _expensecategoryservice.DeleteAsync(id);
            return Ok(result);
        }
        #endregion
    }
}
