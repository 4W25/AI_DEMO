using MediatR;
using AutoMapper;
using AI_DEMO.Application.Common.Exceptions;
using AI_DEMO.Application.Features.UserManagement.DTOs;
using AI_DEMO.Domain.Interfaces;

namespace AI_DEMO.Application.Features.UserManagement.Queries;

/// <summary>
/// 取得單一使用者查詢處理器，負責處理 GetUserQuery。
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// 初始化 GetUserQueryHandler 的新實例。
    /// </summary>
    /// <param name="userRepository">使用者倉儲介面。</param>
    /// <param name="mapper">AutoMapper 介面。</param>
    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 處理取得單一使用者查詢。
    /// </summary>
    /// <param name="request">取得使用者查詢請求。</param>
    /// <param name="cancellationToken">取消操作的令牌。</param>
    /// <returns>使用者資料傳輸物件。</returns>
    /// <exception cref="NotFoundException">當使用者不存在時拋出。</exception>
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        // 根據 ID 取得使用者
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        // 若找不到，拋出例外
        if (user == null)
        {
            throw new NotFoundException($"使用者 ID {request.UserId} 不存在。");
        }

        // 使用 AutoMapper 轉換為 DTO
        return _mapper.Map<UserDto>(user);
    }
}