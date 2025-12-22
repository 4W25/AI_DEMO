using MediatR;

namespace AI_DEMO.Application.Features.UserManagement.Commands;

/// <summary>
/// 刪除使用者命令，根據使用者 ID 刪除。
/// </summary>
/// <param name="UserId">使用者唯一識別碼。</param>
public record DeleteUserCommand(Guid UserId) : IRequest<bool>;