using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vnpt.Application.Interfaces;
using Vnpt.Application.Services;
using Vnpt.Persistence;
using Vnpt.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Lắp ráp Interface và Class thực thi
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>(); 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDocumentRepository, Vnpt.Persistence.Repositories.OrderDocumentRepository>();
builder.Services.AddScoped<IOrderDocumentService, Vnpt.Application.Services.OrderDocumentService>();
builder.Services.AddScoped<IApprovalLogRepository, Vnpt.Persistence.Repositories.ApprovalLogRepository>();
builder.Services.AddScoped<IApprovalService, Vnpt.Application.Services.ApprovalService>();
builder.Services.AddScoped<IDashboardRepository, Vnpt.Persistence.Repositories.DashboardRepository>();
builder.Services.AddScoped<IExcelService, Vnpt.Application.Services.ExcelService>();

// --- CẤU HÌNH JWT AUTHENTICATION ---
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers();

// --- CẤU HÌNH SWAGGER CHUẨN (KHÔNG LỖI VERSION) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 1. THÊM CẤU HÌNH CORS Ở ĐÂY ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Cổng của ReactJS
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- Bật giao diện Swagger ở môi trường Development ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 2. KÍCH HOẠT CORS TRONG MIDDLEWARE (Phải đặt trước UseAuthentication) ---
app.UseCors("AllowReactApp");

app.UseAuthentication(); // Xác thực Token
app.UseAuthorization();  // Phân quyền
app.UseStaticFiles();
app.MapControllers();
app.Run();