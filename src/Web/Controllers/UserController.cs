using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AI_DEMO.Application.Common.Models;
using AI_DEMO.Application.Features.UserManagement.Commands;
using AI_DEMO.Application.Features.UserManagement.DTOs;

namespace AI_DEMO.Web.Controllers;

/// <summary>
/// 使用者管理 MVC 控制器，提供使用者 CRUD 操作的網頁介面。
/// 整體需要登入授權。
/// </summary>
[Authorize]
public class UserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// 初始化 UserController 的新實例。
    /// </summary>
    /// <param name="httpClientFactory">HTTP 客戶端工廠。</param>
    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <summary>
    /// 顯示使用者列表頁面。
    /// </summary>
    /// <param name="pageNumber">頁碼。</param>
    /// <param name="pageSize">每頁項目數。</param>
    /// <returns>使用者列表視圖。</returns>
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"/api/users?pageNumber={pageNumber}&pageSize={pageSize}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PagedResult<UserDto>>();
            return View(result);
        }

        TempData["Error"] = "無法載入使用者列表。";
        return View(new PagedResult<UserDto>(new List<UserDto>(), 0, pageNumber, pageSize));
    }

    /// <summary>
    /// 顯示新增使用者頁面。
    /// </summary>
    /// <returns>新增使用者視圖。</returns>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// 處理新增使用者請求。
    /// </summary>
    /// <param name="model">新增使用者命令。</param>
    /// <returns>重新導向到列表頁或返回視圖。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserCommand model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("/api/users", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "使用者已成功建立。";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "建立使用者失敗。";
        return View(model);
    }

    /// <summary>
    /// 顯示編輯使用者頁面。
    /// </summary>
    /// <param name="id">使用者 ID。</param>
    /// <returns>編輯使用者視圖。</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"/api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            if (user == null)
            {
                TempData["Error"] = "無法讀取使用者資料。";
                return RedirectToAction(nameof(Index));
            }
            var model = new UpdateUserCommand(id, user.Username, user.Email, user.Role, user.IsActive);
            return View(model);
        }

        TempData["Error"] = "找不到使用者。";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 處理編輯使用者請求。
    /// </summary>
    /// <param name="id">使用者 ID。</param>
    /// <param name="model">更新使用者命令。</param>
    /// <returns>重新導向到列表頁或返回視圖。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateUserCommand model)
    {
        if (id != model.UserId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PutAsJsonAsync($"/api/users/{id}", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "使用者已成功更新。";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "更新使用者失敗。";
        return View(model);
    }

    /// <summary>
    /// 顯示使用者詳細資訊頁面。
    /// </summary>
    /// <param name="id">使用者 ID。</param>
    /// <returns>使用者詳細資訊視圖。</returns>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"/api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            return View(user);
        }

        TempData["Error"] = "找不到使用者。";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 處理刪除使用者請求。
    /// </summary>
    /// <param name="id">使用者 ID。</param>
    /// <returns>重新導向到列表頁。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.DeleteAsync($"/api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "使用者已成功刪除。";
        }
        else
        {
            TempData["Error"] = "刪除使用者失敗。";
        }

        return RedirectToAction(nameof(Index));
    }
}