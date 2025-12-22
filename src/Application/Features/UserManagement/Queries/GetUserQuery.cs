using MediatR;
using AI_DEMO.Application.Features.UserManagement.DTOs;

namespace AI_DEMO.Application.Features.UserManagement.Queries;

/// <summary>
/// 取得單一使用者查詢，根據使用者 ID 查詢。
/// </summary>
/// <param name="UserId">使用者唯一識別碼。</param>
public record GetUserQuery(Guid UserId) : IRequest<UserDto>;