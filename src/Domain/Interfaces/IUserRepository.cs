namespace AI_DEMO.Domain.Interfaces;

using AI_DEMO.Domain.Entities;

/// <summary>
/// 使用者資料存取介面，定義對 User 實體的持久化操作。
/// 所有操作均為非同步且支援 CancellationToken。
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據使用者 Id 查詢單筆使用者資訊。
    /// </summary>
    /// <param name="id">使用者唯一識別碼。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>找到的 User 物件，若不存在則返回 null。</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// 根據帳號名稱查詢單筆使用者資訊。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>找到的 User 物件，若不存在則返回 null。</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// 根據電子郵件地址查詢單筆使用者資訊。
    /// </summary>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>找到的 User 物件，若不存在則返回 null。</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// 分頁查詢使用者清單。
    /// </summary>
    /// <param name="pageNumber">頁碼，從 1 開始。</param>
    /// <param name="pageSize">每頁筆數。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>指定頁數的使用者集合。</returns>
    Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// 檢查指定的帳號名稱或電子郵件是否已存在於系統。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>若帳號名稱或電子郵件已存在則返回 true，否則返回 false。</returns>
    Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken);

    /// <summary>
    /// 新增一個使用者到資料庫。
    /// </summary>
    /// <param name="user">待新增的使用者實體。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>表示操作完成的 Task。</returns>
    Task AddAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// 更新資料庫中的使用者資訊。
    /// </summary>
    /// <param name="user">待更新的使用者實體。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>表示操作完成的 Task。</returns>
    Task UpdateAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// 從資料庫中刪除指定 Id 的使用者。
    /// </summary>
    /// <param name="id">待刪除使用者的唯一識別碼。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>表示操作完成的 Task。</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// 取得使用者總數。
    /// </summary>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>使用者總數。</returns>
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 檢查指定的帳號名稱或電子郵件是否已存在於系統（排除指定使用者）。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="excludeUserId">排除的使用者 ID。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>若帳號名稱或電子郵件已存在則返回 true，否則 false。</returns>
    Task<bool> ExistsAsync(string username, string email, Guid excludeUserId, CancellationToken cancellationToken);
}