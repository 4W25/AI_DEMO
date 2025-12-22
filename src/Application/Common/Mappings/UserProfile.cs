using AutoMapper;
using AI_DEMO.Application.Features.UserManagement.DTOs;
using AI_DEMO.Domain.Entities;

namespace AI_DEMO.Application.Common.Mappings;

/// <summary>
/// 使用者實體與 DTO 的對應配置。
/// </summary>
public class UserProfile : Profile
{
    /// <summary>
    /// 初始化 UserProfile 的新實例。
    /// </summary>
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}