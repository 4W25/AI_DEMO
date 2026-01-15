using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AI_DEMO.Domain.Entities;
using AI_DEMO.Domain.Enums;
using AI_DEMO.Infrastructure.Data;

namespace Web.Tests.Controllers;

/// <summary>
/// UsersController Web API 整合測試
/// </summary>
public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // 替換資料庫為 InMemoryDatabase 以便測試
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // 初始化測試資料
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();

                // 添加測試使用者資料
                if (!db.Users.Any())
                {
                    var user = User.Create("testuser", "test@example.com", "hashedpassword", UserRole.User);
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            });
        });
    }

    /// <summary>
    /// 測試未提供 Token 時，GetUsers 應返回 Unauthorized
    /// </summary>
    [Fact]
    public async Task GetUsers_WithoutToken_ShouldReturnUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// 測試提供有效 Token 時，GetUsers 應返回 OK
    /// </summary>
    [Fact]
    public async Task GetUsers_WithValidToken_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var token = GenerateTestToken("User");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<UserDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    /// <summary>
    /// 測試非 Admin 角色建立使用者時，應返回 Forbidden
    /// </summary>
    [Fact]
    public async Task CreateUser_WithNonAdminRole_ShouldReturnForbidden()
    {
        var client = _factory.CreateClient();
        var token = GenerateTestToken("User");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var command = new
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123",
            Role = UserRole.User
        };

        var response = await client.PostAsJsonAsync("/api/users", command);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// 產生測試用的 JWT Token
    /// </summary>
    private string GenerateTestToken(string role)
    {
        var jwtSettings = new
        {
            SecretKey = "YourSuperSecretKeyHere12345678901234567890",
            Issuer = "AI_DEMO",
            Audience = "AI_DEMO_Users",
            ExpiryMinutes = 60
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// 簡化 PagedResult 與 UserDto 定義（實際應從專案引用）
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}