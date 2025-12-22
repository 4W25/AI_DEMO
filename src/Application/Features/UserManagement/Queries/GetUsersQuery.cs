using MediatR;
using AI_DEMO.Application.Common.Models;
using AI_DEMO.Application.Features.UserManagement.DTOs;

namespace AI_DEMO.Application.Features.UserManagement.Queries;

/// <summary>
/// 取得使用者清單查詢，支援分頁。
/// </summary>
/// <param name="PageNumber">頁碼，從 1 開始。</param>
/// <param name="PageSize">每頁項目數。</param>
public record GetUsersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<UserDto>>;