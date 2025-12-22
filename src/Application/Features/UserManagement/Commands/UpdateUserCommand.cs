using MediatR;
using AI_DEMO.Domain.Enums;

namespace AI_DEMO.Application.Features.UserManagement.Commands;

/// <summary>
/// 更新使用者命令，包含更新使用者資訊。
/// </summary>
/// <param name="UserId">使用者唯一識別碼。</param>
/// <param name="Username">新的帳號名稱。</param>
/// <param name="Email">新的電子郵件地址。</param>
/// <param name="Role">新的使用者角色。</param>
/// <param name="IsActive">帳號是否啟用。</param>
public record UpdateUserCommand(Guid UserId, string Username, string Email, UserRole Role, bool IsActive) : IRequest<bool>;