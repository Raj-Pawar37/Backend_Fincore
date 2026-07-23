using Backend_Fincore.Application.Interface;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Application.Services;
using Backend_Fincore.Data;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.Interface;
using Backend_Fincore.Mapper;
using Backend_Fincore.Middleware;
using Backend_Fincore.Service;
using Backend_Fincore.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Infrastructure.Service;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.Services;


using Backend_Fincore.Interface;
using Backend_Fincore.Service;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Infrastucture.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingData));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IBudgetCategoryService, BudgetCategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IBudgetLineService, BudgetLineService>();

builder.Services.AddScoped<IPurchaseRequisitionService, PurchaseRequisitionService>();
builder.Services.AddScoped<IRFQService, RFQService>();



builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));
builder.Services.AddAutoMapper(typeof(MappingData));
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IPurchaseOrderItemService, PurchaseOrderItemService>();
builder.Services.AddScoped<IGRNService, GRNService>();
builder.Services.AddScoped<IAPInvoiceService, APInvoiceService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

//Jwt
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOpexRequestService, OpexRequestService>();
builder.Services.AddScoped<IExpenseClaimService, ExpenseClaimService>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();

builder.Services.AddScoped<IAccountMasterService, AccountMasterService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




builder.Services.AddScoped<IDocumentNumberService, DocumentNumberService>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
