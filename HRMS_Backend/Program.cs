
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HRMSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// 2️⃣ Configure CORS
// --------------------
var corsPolicyName = "AllowAngular";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy
        //.WithOrigins("https://qa-hr.cortracker360.com")
             .WithOrigins("http://localhost:4200") // 👈 exact frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // 👈 REQUIRED for withCredentials
    });
});
// Add services to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuMasterService, MenuMasterService>();
builder.Services.AddScoped<IRoleMasterService, RoleMasterService>();
builder.Services.AddScoped<IMenuRoleService, MenuRoleService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<IEmployeeResignationService, EmployeeResignationService>();

builder.Services.AddScoped<IemployeeService, employeeService>();
builder.Services.AddScoped<IadminService, adminService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IEmployeeKpiService, EmployeeKpiService>();
builder.Services.AddScoped<IManagerKpiReviewService, ManagerKpiReviewService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IKpiCategoryService, KpiCategoryService>();
builder.Services.AddScoped<IEmployeeMasterService, EmployeeMasterService>();
builder.Services.AddScoped<ICertificationTypeService, CertificationTypeService>();
builder.Services.AddScoped<IClockInOutService, ClockInOutService>();
builder.Services.AddScoped<IShiftAllocationService, ShiftAllocationService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAssetStatusService, AssetStatusService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssetApprovalService, AssetApprovalService>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMissedPunchService,MissedPunchService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// --------------------
// 4️⃣ Use CORS
// --------------------
app.UseCors(corsPolicyName);
app.UseHttpsRedirection();
// 🔹 Enable static files (wwwroot)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "Uploads")),
    RequestPath = "/Uploads"
});
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
