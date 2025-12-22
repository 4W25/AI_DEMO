using MediatR;
using AI_DEMO.Application.Common.Exceptions;
using AI_DEMO.Domain.Entities;
using AI_DEMO.Domain.Interfaces;

namespace AI_DEMO.Application.Features.UserManagement.Commands;

/// <summary>
/// 更新使用者命令處理器，負責處理 UpdateUserCommand。
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// 初始化 UpdateUserCommandHandler 的新實例。
    /// </summary>
    /// <param name="userRepository">使用者倉儲介面。</param>
    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// 處理更新使用者命令。
    /// </summary>
    /// <param name="request">更新使用者命令請求。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>更新成功時返回 true。</returns>
    /// <exception cref="NotFoundException">當使用者不存在時拋出。</exception>
    /// <exception cref="DuplicateUserException">當帳號名稱或電子郵件與其他使用者衝突時拋出。</exception>
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // 取得目標使用者
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"使用者 ID {request.UserId} 不存在。");
        }

        // 檢查帳號名稱或電子郵件是否與其他使用者衝突
        if (await _userRepository.ExistsAsync(request.Username, request.Email, request.UserId, cancellationToken))
        {
            throw new DuplicateUserException("帳號名稱或電子郵件已存在。");
        }

        // 更新使用者資訊
        user.UpdateProfile(request.Username, request.Email, request.Role, request.IsActive);

        // 儲存更新
        await _userRepository.UpdateAsync(user, cancellationToken);

        return true;
    }
}