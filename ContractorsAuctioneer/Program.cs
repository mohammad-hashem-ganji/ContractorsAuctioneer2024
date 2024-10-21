using App.Infra.Db.SqlServer.EF.DbContractorsAuctioneerEF;
using ContractorsAuctioneer.Entites;
using ContractorsAuctioneer.Interfaces;
using ContractorsAuctioneer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthService, AuthService>();
// Request
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRequestStatusService, RequestStatusService>();
builder.Services.AddSingleton<IHostedService, RequestCheckService>();
// Client
builder.Services.AddScoped<IClientService, ClientService>();
// Region
builder.Services.AddScoped<IRegionService, RegionService>();
// Contractor
builder.Services.AddScoped<IContractorService, ContractorService>();
// BidOfContractor
builder.Services.AddScoped<IBidOfContractorService, BidOfContractorService>();
builder.Services.AddScoped<IBidStatusService, BidStatusService>();
builder.Services.AddSingleton<IHostedService, BidOfContractorCheckService>();

// Project
builder.Services.AddScoped<IProjectService, ProjectService>();
// FileAttachment
builder.Services.AddScoped<IFileAttachmentService, FileAttachmentService>();
// (2Fa) VerificationService
builder.Services.AddTransient<IVerificationService, VerificationService>();
// LoginHistory
builder.Services.AddTransient<ILastLoginHistoryService, LastLoginHistoryService>();




#region EfConfiguration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<RequestCheckService>();
#endregion

#region IdentityConfiguration

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

#endregion

#region Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
        //TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:EncryptionKey"]))
    };
});
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(45);
});
builder.Services.AddAuthorization();


#endregion



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(optins =>
{
    optins.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter token",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    optins.OperationFilter<SecurityRequirementsOperationFilter>();

}
);

#region CORS

builder.Services.AddCors(o => o.AddPolicy(name: "MyPolicy", b =>
{
    //b.WithOrigins("*")
        b.AllowAnyOrigin() //WithOrigins("http://localhost:8080")
        .AllowAnyMethod()
        .AllowAnyHeader();
    //.AllowCredentials();
}));

#endregion
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors(policyName: "MyPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


