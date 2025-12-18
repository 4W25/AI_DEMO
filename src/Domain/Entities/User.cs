namespace AI_DEMO.Domain.Entities;

using AI_DEMO.Domain.Enums;

/// <summary>
/// 使用者實體，代表系統中的一個使用者帳號。
/// 此類別封裝了使用者的基本資訊與狀態變更邏輯。
/// </summary>
public class User
{
    /// <summary>
    /// 使用者的唯一識別碼。
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 使用者帳號名稱。
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// 使用者電子郵件地址。
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// 密碼雜湊值，不直接儲存密碼純文字。
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// 使用者角色。
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// 帳號是否處於啟用狀態。
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 帳號建立時間（UTC）。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 帳號最後更新時間（UTC），可為 null。
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// 私有建構子，防止直接實例化。
    /// 必須透過 <see cref="Create"/> 工廠方法建立新實例。
    /// </summary>
    private User()
    {
    }

    /// <summary>
    /// 工廠方法，建立新的使用者實體。
    /// 確保必填欄位（Username、Email、PasswordHash、Role）在建立時即為有效值。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="passwordHash">密碼雜湊值。</param>
    /// <param name="role">使用者角色。</param>
    /// <returns>新建立的 User 實體。</returns>
    /// <exception cref="ArgumentException">當必填參數為 null 或空字串時拋出。</exception>
    public static User Create(string username, string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("帳號名稱不可為空。", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("電子郵件地址不可為空。", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("密碼雜湊值不可為空。", nameof(passwordHash));
        }

        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    /// <summary>
    /// 更新使用者個人資料。
    /// </summary>
    /// <param name="username">新的帳號名稱。</param>
    /// <param name="email">新的電子郵件地址。</param>
    /// <param name="role">新的使用者角色。</param>
    /// <param name="isActive">帳號是否啟用。</param>
    /// <exception cref="ArgumentException">當參數無效時拋出。</exception>
    public void UpdateProfile(string username, string email, UserRole role, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("帳號名稱不可為空。", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("電子郵件地址不可為空。", nameof(email));
        }

        Username = username;
        Email = email;
        Role = role;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 變更使用者密碼。
    /// </summary>
    /// <param name="newPasswordHash">新的密碼雜湊值。</param>
    /// <exception cref="ArgumentException">當密碼雜湊值為空時拋出。</exception>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new ArgumentException("新密碼雜湊值不可為空。", nameof(newPasswordHash));
        }

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 啟用使用者帳號。
    /// </summary>
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 停用使用者帳號。
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}