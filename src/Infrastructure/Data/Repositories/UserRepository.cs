using AI_DEMO.Domain.Entities;
using AI_DEMO.Domain.Exceptions;
using AI_DEMO.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AI_DEMO.Infrastructure.Data.Repositories;

/// <summary>
/// 使用者資料存取實作類別，使用 EF Core 實作 IUserRepository。
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 初始化 UserRepository 的新實例。
    /// </summary>
    /// <param name="context">應用程式資料庫上下文。</param>
    public UserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// 根據使用者 Id 查詢單筆使用者資訊。
    /// </summary>
    /// <param name="id">使用者唯一識別碼。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>找到的 User 物件，若不存在則返回 null。</returns>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// 根據帳號名稱查詢單筆使用者資訊。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>找到的 User 物件，若不存在則返回 null。</returns>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    /// <summary>
    /// 根據電子郵件地址查詢單筆使用者資訊。
    /// </summary>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>找到的 User 物件，若不存在則返回 null。</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// 分頁查詢使用者清單。
    /// </summary>
    /// <param name="pageNumber">頁碼，從 1 開始。</param>
    /// <param name="pageSize">每頁筆數。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>指定頁數的使用者集合。</returns>
    public async Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 檢查指定的帳號名稱或電子郵件是否已存在於系統。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>若帳號名稱或電子郵件已存在則返回 true，否則返回 false。</returns>
    public async Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username || u.Email == email, cancellationToken);
    }

    /// <summary>
    /// 新增一個使用者到資料庫。
    /// </summary>
    /// <param name="user">待新增的使用者實體。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>表示操作完成的 Task。</returns>
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 更新資料庫中的使用者資訊。
    /// </summary>
    /// <param name="user">待更新的使用者實體。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>表示操作完成的 Task。</returns>
    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users.FindAsync(new object[] { user.Id }, cancellationToken);
        if (existingUser == null)
        {
            throw new EntityNotFoundException($"使用者 Id {user.Id} 不存在。");
        }

        _context.Entry(existingUser).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 從資料庫中刪除指定 Id 的使用者。
    /// </summary>
    /// <param name="id">待刪除使用者的唯一識別碼。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>表示操作完成的 Task。</returns>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException($"使用者 Id {id} 不存在。");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 取得使用者總數。
    /// </summary>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>使用者總數。</returns>
    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        return await _context.Users.CountAsync(cancellationToken);
    }

    /// <summary>
    /// 檢查指定的帳號名稱或電子郵件是否已存在於系統（排除指定使用者）。
    /// </summary>
    /// <param name="username">帳號名稱。</param>
    /// <param name="email">電子郵件地址。</param>
    /// <param name="excludeUserId">排除的使用者 ID。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>若帳號名稱或電子郵件已存在則返回 true，否則 false。</returns>
    public async Task<bool> ExistsAsync(string username, string email, Guid excludeUserId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AnyAsync(u => (u.Username == username || u.Email == email) && u.Id != excludeUserId, cancellationToken);
    }
}