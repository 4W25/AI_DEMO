using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AI_DEMO.Application.Common.Models;
using AI_DEMO.Application.Features.UserManagement.Commands;
using AI_DEMO.Application.Features.UserManagement.DTOs;
using AI_DEMO.Application.Features.UserManagement.Queries;

namespace AI_DEMO.Web.Controllers;

/// <summary>
/// 使用者管理 API 控制器，提供使用者 CRUD 操作。
/// 預設需要登入授權，管理員操作需要 Admin 角色。
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 初始化 UsersController 的新實例。
    /// </summary>
    /// <param name="mediator">MediatR 介面。</param>
    public UsersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// 取得使用者清單，支援分頁。
    /// </summary>
    /// <param name="query">分頁查詢參數。</param>
    /// <returns>分頁結果，包含使用者清單。</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 根據 ID 取得單一使用者。
    /// </summary>
    /// <param name="id">使用者唯一識別碼。</param>
    /// <returns>使用者資料。</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var query = new GetUserQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 建立新使用者。需要 Admin 角色授權。
    /// </summary>
    /// <param name="command">建立使用者命令。</param>
    /// <returns>新建立使用者的 ID。</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateUser(CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = userId }, userId);
    }

    /// <summary>
    /// 更新使用者資訊。需要 Admin 角色授權。
    /// </summary>
    /// <param name="id">使用者唯一識別碼。</param>
    /// <param name="command">更新使用者命令。</param>
    /// <returns>無內容。</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserCommand command)
    {
        if (command.UserId != id)
        {
            return BadRequest("使用者 ID 不匹配。");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// 刪除使用者。需要 Admin 角色授權。
    /// </summary>
    /// <param name="id">使用者唯一識別碼。</param>
    /// <returns>無內容。</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}