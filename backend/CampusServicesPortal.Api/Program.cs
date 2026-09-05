using System.Text;
using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Interfaces.Repositories;
using CampusServicesPortal.Api.Interfaces.Services;
using CampusServicesPortal.Api.Repositories;
using CampusServicesPortal.Api.Security;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI and Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        );

    options.UseSqlServer(connectionString);
});

// Member 1 dependencies
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<JwtTokenService>();

// Member 2 Event dependencies
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

// Member 2 Complaint dependencies
builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();

// Member 2 Certificate dependencies
builder.Services.AddScoped<
    ICertificateRepository,
    CertificateRepository
>();

// Member 3 Fee dependencies
builder.Services.AddScoped<
    IFeeRepository, 
    FeeRepository
    >();

builder.Services.AddScoped<
    ICertificateService,
    CertificateService
>();

builder.Services.AddScoped<
    IFeeRepository, 
    FeeRepository
    >();


builder.Services.AddScoped<
    IFeeService, 
    FeeService
    >();

// Angular CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key is not configured."
    );

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// OpenAPI and Swagger UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();