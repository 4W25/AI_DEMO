namespace AI_DEMO.Domain.Enums;

/// <summary>
/// 使用者角色列舉，定義系統中的使用者權限等級。
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 管理員角色，具有最高權限。
    /// </summary>
    Admin = 1,

    /// <summary>
    /// 一般使用者角色，具有基本權限。
    /// </summary>
    User = 2
}