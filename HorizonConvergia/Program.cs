using DataAccessObjects;
using DataAccessObjects.Data;
using DataAccessObjects.Setting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Repositories.Interfaces;
using Services;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Authorization policies cho Role
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim("Role", "Admin"));
    options.AddPolicy("Seller", policy =>
        policy.RequireClaim("Role", "Seller"));
    options.AddPolicy("Shipper", policy =>
        policy.RequireClaim("Role", "Shipper"));
    options.AddPolicy("Buyer", policy =>
        policy.RequireClaim("Role", "Buyer"));
    options.AddPolicy("AdminOrSellerAccessPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var roleClaim = context.User.FindFirst("Role")?.Value;
            return roleClaim == "Admin" || roleClaim == "Seller";
        }));
    options.AddPolicy("AdminSellerOrShipperAccessPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            var roleClaim = context.User.FindFirst("Role")?.Value;
            return roleClaim == "Admin" || roleClaim == "Seller" || roleClaim == "Shipper";
        }));
});

// 2. Swagger config (bảo mật JWT Bearer)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HorizonConvergia API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.  
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });
});

// 3. Authentication (Cookie + Google + JWT Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/api/Auth/google-response";
        options.SaveTokens = true;
        options.Scope.Add("email");
    })
    .AddJwtBearer(item =>
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

// 4. DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// 5. Controllers
builder.Services.AddControllers();

// 6. Các service/repository DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<PaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.Configure<PayOSSettings>(
    builder.Configuration.GetSection("PayOS"));



// 7. CORS (bạn nên bật nếu frontend gọi API từ domain khác)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // thay URL frontend của bạn
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 8. Swagger & SwaggerUI
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HorizonConvergia API V1");
    //c.RoutePrefix = string.Empty; // Swagger UI tại root (http://host/)
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
