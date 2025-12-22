using AI_DEMO.Domain.Entities;
using AI_DEMO.Domain.Enums;

namespace AI_DEMO.Application.Features.UserManagement.DTOs;

/// <summary>
/// 使用者資料傳輸物件，不包含敏感資訊如密碼雜湊。
/// </summary>
public class UserDto
{
    /// <summary>
    /// 使用者的唯一識別碼。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 使用者帳號名稱。
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 使用者電子郵件地址。
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者角色。
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// 帳號是否處於啟用狀態。
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 帳號建立時間（UTC）。
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 帳號最後更新時間（UTC），可為 null。
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}