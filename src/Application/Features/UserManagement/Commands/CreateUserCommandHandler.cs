using MediatR;
using AI_DEMO.Application.Common.Exceptions;
using AI_DEMO.Domain.Entities;
using AI_DEMO.Domain.Interfaces;

namespace AI_DEMO.Application.Features.UserManagement.Commands;

/// <summary>
/// 新增使用者命令處理器，負責處理 CreateUserCommand。
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// 初始化 CreateUserCommandHandler 的新實例。
    /// </summary>
    /// <param name="userRepository">使用者倉儲介面。</param>
    /// <param name="passwordHasher">密碼雜湊介面。</param>
    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// 處理新增使用者命令。
    /// </summary>
    /// <param name="request">新增使用者命令請求。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>新建立使用者的唯一識別碼。</returns>
    /// <exception cref="DuplicateUserException">當使用者名稱或電子郵件已存在時拋出。</exception>
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 檢查使用者名稱或電子郵件是否已存在
        if (await _userRepository.ExistsAsync(request.Username, request.Email, cancellationToken))
        {
            throw new DuplicateUserException("使用者名稱或電子郵件已存在。");
        }

        // 雜湊密碼
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 建立新使用者實體
        var user = User.Create(request.Username, request.Email, passwordHash, request.Role);

        // 新增使用者到資料庫
        await _userRepository.AddAsync(user, cancellationToken);

        // 返回新使用者的 ID
        return user.Id;
    }
}