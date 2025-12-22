using MediatR;
using AutoMapper;
using AI_DEMO.Application.Common.Models;
using AI_DEMO.Application.Features.UserManagement.DTOs;
using AI_DEMO.Domain.Interfaces;

namespace AI_DEMO.Application.Features.UserManagement.Queries;

/// <summary>
/// 取得使用者清單查詢處理器，負責處理 GetUsersQuery。
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// 初始化 GetUsersQueryHandler 的新實例。
    /// </summary>
    /// <param name="userRepository">使用者倉儲介面。</param>
    /// <param name="mapper">AutoMapper 介面。</param>
    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 處理取得使用者清單查詢。
    /// </summary>
    /// <param name="request">取得使用者清單查詢請求。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>分頁結果，包含使用者 DTO 清單。</returns>
    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // 取得分頁使用者清單
        var users = await _userRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        // 取得總數
        var totalCount = await _userRepository.GetTotalCountAsync(cancellationToken);

        // 使用 AutoMapper 轉換為 DTO 清單
        var userDtos = _mapper.Map<IReadOnlyList<UserDto>>(users);

        // 返回分頁結果
        return new PagedResult<UserDto>(userDtos, totalCount, request.PageNumber, request.PageSize);
    }
}