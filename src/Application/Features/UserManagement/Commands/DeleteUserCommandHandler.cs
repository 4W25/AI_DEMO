using MediatR;
using AI_DEMO.Application.Common.Exceptions;
using AI_DEMO.Domain.Interfaces;

namespace AI_DEMO.Application.Features.UserManagement.Commands;

/// <summary>
/// 刪除使用者命令處理器，負責處理 DeleteUserCommand。
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// 初始化 DeleteUserCommandHandler 的新實例。
    /// </summary>
    /// <param name="userRepository">使用者倉儲介面。</param>
    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// 處理刪除使用者命令。
    /// </summary>
    /// <param name="request">刪除使用者命令請求。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>刪除成功時返回 true。</returns>
    /// <exception cref="NotFoundException">當使用者不存在時拋出。</exception>
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // 刪除使用者（若不存在，DeleteAsync 會拋出例外）
        await _userRepository.DeleteAsync(request.UserId, cancellationToken);

        return true;
    }
}