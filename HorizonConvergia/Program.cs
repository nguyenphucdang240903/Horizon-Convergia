using Azure.Core;
using Azure.Identity;
using DataAccessObjects;
using DataAccessObjects.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Repositories.Interfaces;
using Services;
using Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    // Chính sách cho vai trò admin
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "Admin")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho vai trò seller
    options.AddPolicy("Seller", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "Seller")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho vai trò shipper
    options.AddPolicy("Shipper", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "Shipper")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho vai trò buyer
    options.AddPolicy("Buyer", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && roleClaim.Value == "Buyer")
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho các tài nguyên mà c? manager và seller có th? truy c?p
    options.AddPolicy("AdminOrSellerAccessPolicy", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && (roleClaim.Value == "Admin" || roleClaim.Value == "Seller"))
            {
                return true;
            }
            return false;
        });
    });

    // Chính sách cho các tài nguyên mà c? manager, seller, và shipper ??u có th? truy c?p
    options.AddPolicy("AdminSellerOrShipperAccessPolicy", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst("Role");
            if (roleClaim != null && (roleClaim.Value == "Admin" || roleClaim.Value == "Seller" || roleClaim.Value == "Shipper"))
            {
                return true;
            }
            return false;
        });
    });
});


// builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter the JWT token obtained from the login endpoint",
//        Name = "Authorization"
//    });
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            }
//                        },
//                        Array.Empty<string>()
//                    }
//                });
//});
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU=")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var sqlConnection = new SqlConnection(connectionString);

    // Lấy access token cho Azure SQL bằng Managed Identity
    var credential = new DefaultAzureCredential();
    var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
    var accessToken = credential.GetToken(tokenRequestContext).Token;

    sqlConnection.AccessToken = accessToken;

    options.UseSqlServer(sqlConnection);
});



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/api/Auth/google-response"; // default
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // adjust for your frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});





builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HorizonConvergia",
        Version = "1.0"
    });

    options.AddServer(new OpenApiServer
    {
        Url = "https://horizonconvergia20250530124748-b3h6hjdxe0dya2g3.canadacentral-01.azurewebsites.net"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token obtained from the login endpoint",
        Name = "Authorization"
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
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HorizonConvergia API V1");
    c.RoutePrefix = "swagger"; // Có thể truy cập tại /swagger
});


app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
