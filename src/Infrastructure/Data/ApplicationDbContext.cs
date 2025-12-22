using AI_DEMO.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI_DEMO.Infrastructure.Data;

/// <summary>
/// 應用程式資料庫上下文，負責與資料庫的互動。
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// 初始化 ApplicationDbContext 的新實例。
    /// </summary>
    /// <param name="options">資料庫上下文選項。</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 使用者實體的 DbSet。
    /// </summary>
    public DbSet<User> Users { get; set; } = default!;

    /// <summary>
    /// 配置模型。
    /// </summary>
    /// <param name="modelBuilder">模型建構器。</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 User 實體
        modelBuilder.Entity<User>(entity =>
        {
            // 主鍵設定
            entity.HasKey(u => u.Id);

            // Username：必填，最大長度 50，建立唯一索引
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(u => u.Username).IsUnique();

            // Email：必填，最大長度 100，建立唯一索引
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();

            // PasswordHash：必填，最大長度 500
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);

            // Role：使用 HasConversion 儲存為整數
            entity.Property(u => u.Role).HasConversion<int>();

            // IsActive：必填
            entity.Property(u => u.IsActive).IsRequired();

            // CreatedAt：必填，預設值為 UTC 現在時間（SQLite）
            entity.Property(u => u.CreatedAt).IsRequired().HasDefaultValueSql("datetime('now')");

            // UpdatedAt：可為 null
            entity.Property(u => u.UpdatedAt);
        });
    }
}