using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Helper;
using Asp.NETCoreApi.IRepositories;
using Asp.NETCoreApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==================== Add services to the container ====================

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Book API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// 3. Configure CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowLocalhost", builder => {
        builder.SetIsOriginAllowed(origin =>
            origin.StartsWith("http://localhost") || origin.StartsWith("http://127.0.0.1"))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials(); // Cho phép gửi cookie từ client
    });
});

// 4. Database Context
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

// 5. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 6. Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email phải là duy nhất
})
.AddEntityFrameworkStores<MyDbContext>()
.AddDefaultTokenProviders();

// 7. Configure Authentication and JWT (Read Token from Cookie)
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Đặt 'true' khi dùng HTTPS trong production
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
    };

    // Lấy token từ cookie thay vì Authorization header
    options.Events = new JwtBearerEvents {
        OnMessageReceived = context => {
            var token = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token)) {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };
});

// 8. Add Authorization
builder.Services.AddAuthorization();

// 9. Register Custom Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IToBuyLaterRepository, ToBuyLaterRepository>();


// ==================== Build Application ====================
var app = builder.Build();

// ==================== Configure the HTTP Request Pipeline ====================

// Enable CORS
app.UseCors("AllowLocalhost");

// Swagger (Development Only)
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication(); // Xác thực JWT (Cookie chứa token)
app.UseAuthorization();   // Kiểm tra quyền hạn

//app.UseMiddleware<AccessTokenMiddleware>();

// Map Controllers
app.MapControllers();

// Run Application
app.Run();
