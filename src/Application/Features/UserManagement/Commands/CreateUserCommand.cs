using MediatR;
using AI_DEMO.Domain.Enums;

namespace AI_DEMO.Application.Features.UserManagement.Commands;

/// <summary>
/// 新增使用者命令，包含使用者基本資訊。
/// </summary>
/// <param name="Username">使用者名稱。</param>
/// <param name="Email">電子郵件地址。</param>
/// <param name="Password">密碼。</param>
/// <param name="Role">使用者角色。</param>
public record CreateUserCommand(string Username, string Email, string Password, UserRole Role) : IRequest<Guid>;