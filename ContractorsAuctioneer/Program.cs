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
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
// Reject
builder.Services.AddTransient<IRejectService, RejectService>();




//_____________________________________________________________________________


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

#region IdentityConfiguration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            var roleClaims = claimsIdentity?.FindAll(ClaimTypes.Role).Select(c => c.Value);
            Console.WriteLine($"Token validated for user: {context.Principal.Identity.Name} with roles: {string.Join(", ", roleClaims)}");
            return Task.CompletedTask;
        }
    };
});
#endregion

builder.Services.AddCors(o => o.AddPolicy(name: "MyPolicy", b =>
{
    b.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
}));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.OAuthClientId("swagger");
        options.OAuthAppName("Swagger UI");
    });
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors(policyName: "MyPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
















//____________________________________________________________________________________

//builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();

//#region EfConfiguration
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddHostedService<RequestCheckService>();
//#endregion

//#region IdentityConfiguration

//builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 6;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireLowercase = false;

//})
//.AddRoles<IdentityRole<int>>()
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();

//#endregion

//#region Authentication

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//            ValidAudience = builder.Configuration["JwtSettings:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
//            //TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:EncryptionKey"]))
//        };
//        options.Events = new JwtBearerEvents
//        {
//            OnAuthenticationFailed = context =>
//            {
//                Console.WriteLine("Authentication failed: " + context.Exception.Message);
//                return Task.CompletedTask;
//            },
//            OnTokenValidated = context =>
//            {
//                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
//                var roleClaims = claimsIdentity?.FindAll(ClaimTypes.Role).Select(c => c.Value);
//                Console.WriteLine($"Token validated for user: {context.Principal.Identity.Name} with roles: {string.Join(", ", roleClaims)}");

//                Console.WriteLine("Token validated for user: " + context.Principal.Identity.Name);
//                return Task.CompletedTask;
//            }
//        };
//    });

//builder.Services.AddAuthorization();


//#endregion



//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter JWT with Bearer into field",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] { }
//        }
//    });
//});

//#region CORS

//builder.Services.AddCors(o => o.AddPolicy(name: "MyPolicy", b =>
//{
//    //b.WithOrigins("*")
//    b.AllowAnyOrigin() //WithOrigins("http://localhost:8080")
//    .AllowAnyMethod()
//    .AllowAnyHeader();
//    //.AllowCredentials();
//}));

//#endregion
//builder.Services.AddHttpContextAccessor();
//var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


//}
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
//        options.OAuthClientId("swagger");
//        options.OAuthAppName("Swagger UI");
//    });
//}
//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseRouting();
//app.UseCors(policyName: "MyPolicy");
//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();




//app.MapControllers();


//app.Run();


