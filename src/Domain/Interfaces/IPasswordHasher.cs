namespace AI_DEMO.Domain.Interfaces;

/// <summary>
/// 密碼雜湊介面，提供密碼雜湊與驗證功能。
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// 雜湊密碼，將原始密碼轉換為安全的雜湊值。
    /// </summary>
    /// <param name="password">原始密碼字串。</param>
    /// <returns>雜湊後的密碼字串。</returns>
    string HashPassword(string password);

    /// <summary>
    /// 驗證密碼與雜湊值是否相符。
    /// </summary>
    /// <param name="password">原始密碼字串。</param>
    /// <param name="passwordHash">儲存的雜湊值。</param>
    /// <returns>若密碼與雜湊相符則返回 true，否則 false。</returns>
    bool VerifyPassword(string password, string passwordHash);
}