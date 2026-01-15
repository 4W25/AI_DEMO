using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 註冊 JWT 認證服務
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // 從設定檔讀取 JWT 相關設定
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is required");
    var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is required");
    var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is required");

    // 設定 JWT 令牌驗證參數
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,          // 驗證發行者
        ValidIssuer = issuer,
        ValidateAudience = true,        // 驗證受眾
        ValidAudience = audience,
        ValidateLifetime = true,        // 驗證令牌有效期
        ValidateIssuerSigningKey = true, // 驗證簽署金鑰
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 註冊授權服務
builder.Services.AddAuthorization();

// 註冊 HTTP 客戶端工廠，用於呼叫內部 API
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001"); // 假設本機 API 地址
});

// 註冊 MVC 與 API 控制器
builder.Services.AddControllersWithViews();

// 註冊 MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 註冊 AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 註冊應用程式服務
builder.Services.AddScoped<AI_DEMO.Domain.Interfaces.IUserRepository, AI_DEMO.Infrastructure.Data.Repositories.UserRepository>();
builder.Services.AddScoped<AI_DEMO.Domain.Interfaces.IPasswordHasher, AI_DEMO.Infrastructure.Services.PasswordHasher>();

var app = builder.Build();

// 使用認證中介軟體
app.UseAuthentication();

// 使用授權中介軟體
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
