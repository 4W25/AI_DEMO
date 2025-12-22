using AI_DEMO.Domain.Interfaces;

namespace AI_DEMO.Infrastructure.Services;

/// <summary>
/// 密碼雜湊實作類別，使用 BCrypt 演算法提供安全的密碼雜湊與驗證。
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// 雜湊密碼，使用適中的工作因子 (WorkFactor) 12 以平衡安全與效能。
    /// </summary>
    /// <param name="password">原始密碼字串。</param>
    /// <returns>雜湊後的密碼字串。</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// 驗證密碼與雜湊值是否相符。
    /// </summary>
    /// <param name="password">原始密碼字串。</param>
    /// <param name="passwordHash">儲存的雜湊值。</param>
    /// <returns>若密碼與雜湊相符則返回 true，否則 false。</returns>
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}