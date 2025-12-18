# copilot-prompts

## 專案背景與總則

本專案採用 **ASP.NET Core 9 MVC + Entity Framework Core 9 + AdminLTE 5**，實作「使用者管理（註冊、登入、權限）」前後端功能。  
請你（Copilot）在產生程式碼、重構或解釋時，務必遵守下列規則：

- 所有說明與註解一律使用**繁體中文**。
- 後端程式碼使用 **C# 13+ / .NET 9**，遵循專案現有規範。
- 資料存取使用 **EF Core 9**，資料庫為 SQLite Server。
- 採用 **Clean Architecture** 分層（Domain / Application / Infrastructure / Web）。
- 驗證使用 **FluentValidation**，中介層使用 **MediatR**（Commands / Queries）。
- 不要引入專案中未使用的 NuGet 套件，除非有明確指示。
- 公開方法需有 XML 註解，非同步方法名稱以 `Async` 結尾。
- Controller 不可直接存取 DbContext，一律透過 Repository 或 Application 層。

以下各段為可直接貼給你的指令模板（Prompts），分為後端 CRUD 與前端 AdminLTE 5 頁面。

---

## 一、Domain 層 Prompts

### 1. 建立 User Entity

請在 src/Domain/Entities/ 建立 User.cs，遵循本專案 Domain 層規範，實作使用者實體，需求如下：

使用 C# 13+ 語法，符合 .NET 9 慣例。

類別名稱：User。

屬性：

Id：Guid

Username：string

Email：string

PasswordHash：string

Role：UserRole (enum)

IsActive：bool

CreatedAt：DateTime

UpdatedAt：DateTime?

透過建構子或工廠方法確保必填欄位（Username、Email、PasswordHash、Role）在建立時即為有效值。

提供下列方法並封裝狀態變更邏輯：

UpdateProfile(string username, string email, UserRole role, bool isActive)

ChangePassword(string newPasswordHash)

Activate()

Deactivate()

使用 XML 註解說明類別與每個 public 成員用途。

私有欄位若有，使用 _camelCase 命名。避免使用 dynamic 與 object。

不直接暴露密碼純文字，僅存放 PasswordHash。

請直接產生完整 User.cs 檔案程式碼，註解與說明一律使用繁體中文。

text

### 2. 建立 UserRole Enum

請在 src/Domain/Enums/ 建立 UserRole.cs，定義使用者角色列舉，需求如下：

列舉名稱：UserRole。

成員：

Admin = 1

User = 2

使用 XML 註解為每個成員撰寫繁體中文說明。

Enum 檔案需放在對應 namespace 下，符合專案命名慣例。

請直接產生完整 UserRole.cs 程式碼。

text

### 3. 建立 IUserRepository 介面

請在 src/Domain/Interfaces/ 建立 IUserRepository.cs，定義使用者資料存取介面，需求如下：

介面名稱：IUserRepository。

方法（全部為非同步，需包含 CancellationToken）：

Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

Task<IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken);

Task AddAsync(User user, CancellationToken cancellationToken);

Task UpdateAsync(User user, CancellationToken cancellationToken);

Task DeleteAsync(Guid id, CancellationToken cancellationToken);

使用 XML 註解說明每個方法用途與參數。

不要使用 dynamic 與 object，型別明確。

請直接產生完整 IUserRepository.cs 程式碼。

text

---

## 二、Infrastructure 層 Prompts

### 4. 實作 UserRepository（EF Core）

請在 src/Infrastructure/Data/Repositories/ 建立 UserRepository.cs，實作 IUserRepository，需求如下：

類別名稱：UserRepository，實作 IUserRepository。

透過建構子注入 ApplicationDbContext，使用 private readonly 欄位儲存。

使用 EF Core 9 實作各方法：

查詢方法優先使用 AsNoTracking()（除非需要追蹤）。

GetPagedAsync 使用 Skip/Take 實作分頁。

ExistsAsync 以 AnyAsync 檢查 Username 或 Email 是否存在。

刪除與更新前先檢查目標是否存在，不存在時可以回傳不動作或拋出自訂例外（請建立簡單自訂例外類別，如 EntityNotFoundException 放在 Infrastructure 或 Domain）。

所有非同步方法都接受 CancellationToken 並傳遞到 EF Core API。

使用 XML 註解描述類別與公開方法。

禁止在此類別中直接使用 IServiceProvider.GetService。

請直接產生完整 UserRepository.cs 程式碼，包含必要的 using。

text

### 5. 在 ApplicationDbContext 中註冊 User

請修改 src/Infrastructure/Data/ApplicationDbContext.cs，加入 User 實體的 DbSet 與模型配置，需求如下：

新增：

public DbSet<User> Users { get; set; } = default!;

在 OnModelCreating(ModelBuilder modelBuilder) 中加入 User 的 Fluent API 設定：

主鍵：Id。

Username：必填，最大長度 50，建立唯一索引。

Email：必填，最大長度 100，建立唯一索引。

PasswordHash：必填，最大長度 500。

Role：使用 HasConversion<int>() 儲存為整數。

CreatedAt：預設值為 GETUTCDATE()（或 DateTime.UtcNow 的 SQL 對應）。

所有設定需有簡短繁體中文註解。

請顯示 ApplicationDbContext.cs 相關修改後的完整程式碼（可省略不相干的 DbSet）。

text

### 6. 建立 PasswordHasher 服務

請在 src/Infrastructure/Services/ 建立 PasswordHasher.cs 與對應介面 IPasswordHasher.cs（放在 Domain/Interfaces 或 Application/Common/Interfaces），需求如下：

IPasswordHasher 介面：

string HashPassword(string password);

bool VerifyPassword(string password, string passwordHash);

PasswordHasher 類別實作 IPasswordHasher，使用 BCrypt.Net-Next（若專案尚未使用該套件，請僅示範程式碼，不要寫安裝指示）。

HashPassword 使用強雜湊（例如 WorkFactor 適中）。

VerifyPassword 負責驗證密碼與雜湊是否相符。

所有公開成員加入 XML 註解，註解使用繁體中文。

請產生 IPasswordHasher.cs 與 PasswordHasher.cs 完整程式碼。

text

---

## 三、Application 層 Prompts（Commands / Queries / Validators）

### 7. 建立 CreateUserCommand / Handler / Validator

請在 src/Application/Features/UserManagement/Commands/ 建立新增使用者相關檔案，需求如下：

CreateUserCommand.cs：

放在 Commands 子資料夾。

類型可使用 record。

屬性：Username、Email、Password、Role (UserRole)。

實作 IRequest<Guid>（返回新建立 User 的 Id）。

加入 XML 註解。

CreateUserCommandHandler.cs：

實作 IRequestHandler<CreateUserCommand, Guid>。

透過建構子注入 IUserRepository 與 IPasswordHasher。

Handle 流程：
a. 檢查 Username 或 Email 是否重複（呼叫 ExistsAsync）。
b. 若重複，拋出自訂例外（例如 DuplicateUserException，放在 Application/Common/Exceptions）。
c. 使用 IPasswordHasher.HashPassword 雜湊密碼。
d. 建立 User 實體並呼叫 AddAsync。
e. 返回 user.Id。

使用 Guard Clauses 早期返回，避免巢狀過深。

加入 XML 註解。

CreateUserCommandValidator.cs：

放在 Validators 子資料夾。

繼承 AbstractValidator<CreateUserCommand>。

驗證規則：

Username：必填、長度 3–50、僅允許英數字與底線。

Email：必填、Email 格式、最大長度 100。

Password：必填、最少 8 碼、需包含大小寫字母與數字。

Role：必須是有效的 UserRole。

錯誤訊息為繁體中文。

請直接產生上述三個檔案的完整程式碼。

text

### 8. 建立 GetUserQuery / GetUsersQuery 與 Handler

請在 src/Application/Features/UserManagement/Queries/ 建立查詢使用者功能，需求如下：

GetUserQuery.cs：

record GetUserQuery(Guid UserId) : IRequest<UserDto>;

用於取得單一使用者資料。

GetUserQueryHandler.cs：

實作 IRequestHandler<GetUserQuery, UserDto>。

注入 IUserRepository 與 IMapper (AutoMapper)。

Handle 流程：
a. 呼叫 GetByIdAsync。
b. 若找不到，拋出 NotFoundException（放在 Application/Common/Exceptions）。
c. 使用 AutoMapper 將 User 轉為 UserDto。

GetUsersQuery.cs：

包含 PageNumber (int, 預設 1)、PageSize (int, 預設 10)。

實作 IRequest<PagedResult<UserDto>>（PagedResult 為共用分頁模型，如尚未存在請一併建立）。

GetUsersQueryHandler.cs：

實作 IRequestHandler<GetUsersQuery, PagedResult<UserDto>>。

呼叫 IUserRepository.GetPagedAsync。

使用 AutoMapper 轉成 UserDto 集合。

補上 TotalCount（可另寫 repository 方法或簡化為只做假分頁，請以實務為主）。

所有 public 類別與方法加入 XML 註解，註解使用繁體中文。

請產生上述四個檔案（如缺 PagedResult<T>，一併示範其實作）。

text

### 9. 建立 UpdateUserCommand / DeleteUserCommand

請在 src/Application/Features/UserManagement/Commands/ 建立更新與刪除使用者相關檔案，需求如下：

UpdateUserCommand.cs：

屬性：UserId (Guid)、Username、Email、Role、IsActive。

實作 IRequest<bool>。

UpdateUserCommandHandler.cs：

實作 IRequestHandler<UpdateUserCommand, bool>。

注入 IUserRepository。

Handle 流程：
a. 取得目標 User（GetByIdAsync）。
b. 檢查 Username / Email 是否與其他使用者衝突（可另寫 ExistsAsync 排除自身 Id）。
c. 呼叫 User.UpdateProfile(...) 設定新值。
d. 呼叫 UpdateAsync。
e. 返回 true。

UpdateUserCommandValidator.cs：

基本驗證規則與 CreateUserCommandValidator 類似，但可不驗證密碼。

DeleteUserCommand.cs：

屬性：UserId (Guid)。

實作 IRequest<bool>。

DeleteUserCommandHandler.cs：

實作 IRequestHandler<DeleteUserCommand, bool>。

呼叫 IUserRepository.DeleteAsync。

若刪除目標不存在，拋出 NotFoundException。

成功時返回 true。

請產生上述檔案完整程式碼，並加上繁體中文 XML 註解。

text

---

## 四、Web API 層 Prompts（UsersController + Auth）

### 10. 建立 UserDto

請在 src/Web/DTOs/ 建立 UserDto.cs，作為對外傳輸模型，需求如下：

使用 record 類型：UserDto。

屬性：

Guid Id

string Username

string Email

UserRole Role

bool IsActive

DateTime CreatedAt

DateTime? UpdatedAt

不包含 PasswordHash 等敏感資訊。

使用 XML 註解說明用途。

請直接產生完整 UserDto.cs 程式碼。

text

### 11. 建立 UsersController（Web API）

請在 src/Web/Controllers/ 建立 UsersController.cs，作為 Web API 控制器，需求如下：

加上屬性：

[ApiController]

[Route("api/[controller]")]

[Authorize]（預設需登入才能使用）

透過建構子注入 IMediator。

動作方法：

[HttpGet] GetUsers(GetUsersQuery query)：

從 query string 綁定 PageNumber / PageSize。

呼叫 MediatR 取得 PagedResult<UserDto>。

[HttpGet("{id:guid}")] GetUser(Guid id)：

建立 GetUserQuery 並送出。

[HttpPost] [Authorize(Roles = "Admin")] CreateUser(CreateUserCommand command)：

呼叫 MediatR 建立使用者。

[HttpPut("{id:guid}")] [Authorize(Roles = "Admin")] UpdateUser(Guid id, UpdateUserCommand command)：

確保 command.UserId = id。

[HttpDelete("{id:guid}")] [Authorize(Roles = "Admin")] DeleteUser(Guid id)：

呼叫 DeleteUserCommand。

回傳型別使用 ActionResult<>，錯誤時透過全域 Exception Filter 統一轉成 ProblemDetails。

所有公開動作加入 XML 註解，描述用途與授權需求。

請直接產生 UsersController.cs 完整程式碼。

text

### 12. 設定 JWT 認證

請在 src/Web/Program.cs 中設定 JWT 認證與授權，需求如下：

使用 AddAuthentication().AddJwtBearer(...) 註冊 JWT。

從設定檔（如 appsettings.json 的 "Jwt" 區段）讀取：

SecretKey

Issuer

Audience

ExpiryMinutes

設定 TokenValidationParameters：

ValidateIssuer = true

ValidateAudience = true

ValidateLifetime = true

ValidateIssuerSigningKey = true

在中介管線中呼叫 app.UseAuthentication() 與 app.UseAuthorization()。

用繁體中文註解說明主要設定目的。

請示範 Program.cs 中與 JWT 相關的完整程式碼片段（可省略無關設定）。

text

---

## 五、MVC + AdminLTE 5 前端頁面 Prompts

### 13. 建立 UserController（MVC 頁面）

請在 src/Web/Controllers/ 建立 UserController.cs（注意：這是 MVC Controller，不是 API Controller），需求如下：

不使用 [ApiController]。

加上 [Authorize] 屬性，整體須登入才能瀏覽。

透過建構子注入 IHttpClientFactory，用來呼叫本機 /api/users API。

動作方法：

Index()：顯示使用者列表頁（Views/User/Index.cshtml）。

Create() [HttpGet]：顯示新增頁面。

Create(CreateUserCommand model) [HttpPost]：呼叫 /api/users 建立使用者。

Edit(Guid id) [HttpGet]：從 API 取得資料並顯示編輯頁。

Edit(Guid id, UpdateUserCommand model) [HttpPost]：呼叫 /api/users/{id} 更新。

Details(Guid id) [HttpGet]：顯示詳細資訊頁。

Delete(Guid id) [HttpPost]：呼叫 DELETE /api/users/{id}。

使用 TempData 或 ViewData 顯示成功/錯誤訊息。

使用 XML 註解說明每個 Action 用途。

請產生 UserController.cs 完整程式碼（可略過 HttpClient 工廠註冊細節）。

text

### 14. 建立使用者列表頁 Index.cshtml（AdminLTE + DataTables）

請在 src/Web/Views/User/ 建立 Index.cshtml，使用 AdminLTE 5 + DataTables 顯示使用者列表，需求如下：

使用 @model PagedResult<UserDto>（若前端使用 AJAX 取資料，也可以單純使用空 model）。

頁面使用 AdminLTE 的 content header + card 版型：

標題：「使用者管理」

麵包屑：「首頁 / 使用者管理」

Card 內容為一個 table，欄位：

帳號

Email

角色

狀態

建立時間

操作（檢視/編輯/刪除按鈕）

使用 DataTables 初始化表格：

啟用分頁、搜尋、排序。

使用 AJAX 自 /api/users 取得資料（如需可指定 columns 對應）。

刪除按鈕點擊時使用 SweetAlert2 顯示確認視窗，確認後呼叫 API 刪除，再刷新表格。

狀態欄位使用 AdminLTE badge 顯示（啟用為綠色，停用為紅色）。

所有文字與註解使用繁體中文。

請產生完整 Index.cshtml 範例，包含必要的 JavaScript 區塊（可假設 _Layout.cshtml 已引用 AdminLTE 5、jQuery、DataTables、SweetAlert2、Toastr 等資源）。

text

### 15. 建立新增使用者頁面 Create.cshtml

請在 src/Web/Views/User/ 建立 Create.cshtml，使用 AdminLTE 5 表單樣式，需求如下：

使用 @model CreateUserCommand。

使用 AdminLTE 5 card 佈局：

標題：「新增使用者」

表單欄位：

Username：文字輸入，必填。

Email：email 輸入，必填。

Password：password 輸入，必填，顯示密碼強度提示。

ConfirmPassword：password 輸入，必填，需與 Password 一致（可用前端驗證）。

Role：下拉選單（Admin / User）。

使用 ASP.NET Core Tag Helpers（asp-for）綁定欄位，顯示驗證訊息。

使用 jQuery Validation 做基本前端驗證，錯誤訊息顯示在欄位下方。

底部提供「儲存」(btn-success) 與「返回列表」(btn-secondary) 按鈕。

所有標籤與註解使用繁體中文。

請產生完整 Create.cshtml 程式碼。

text

### 16. 建立編輯與詳細頁面 Edit.cshtml / Details.cshtml

請在 src/Web/Views/User/ 建立 Edit.cshtml 與 Details.cshtml，需求如下：

Edit.cshtml：

@model UpdateUserCommand。

佈局與 Create.cshtml 類似，但不包含密碼欄位。

欄位：Username、Email、Role、IsActive（使用 switch 或 checkbox）。

提供「儲存」與「返回列表」按鈕。

使用 ASP.NET Core 驗證標籤顯示錯誤訊息。

Details.cshtml：

@model UserDto。

使用 AdminLTE card + description list (dl/dt/dd) 顯示：

帳號、Email、角色、狀態、建立時間、最後更新時間。

角色與狀態以 badge 顯示。

Card footer 提供「返回列表」與「編輯」按鈕。

請分別產生 Edit.cshtml 與 Details.cshtml 範例程式碼，文字與註解使用繁體中文。

text

### 17. 登入 / 註冊頁面（Login / Register）使用 AdminLTE 5

請在 src/Web/Views/Account/ 建立 Login.cshtml 與 Register.cshtml，需求如下：

Login.cshtml：

使用 AdminLTE 5 login-page 樣板。

欄位：帳號或 Email、密碼、記住我 (checkbox)。

送出到 /api/auth/login（可用 MVC Controller 再轉呼叫 API）。

成功後導向 /User/Index，失敗使用 Toastr 顯示錯誤訊息。

顯示「註冊新帳號」連結到 Register 頁面。

Register.cshtml：

使用 AdminLTE 5 register-page 樣板。

欄位：Username、Email、Password、ConfirmPassword、同意條款 (checkbox)。

前端驗證：所有欄位必填、Email 格式、密碼長度與強度、確認密碼一致。

送出後呼叫 /api/users 建立使用者（或 /api/auth/register，依專案實作而定）。

成功後顯示成功訊息並導向 Login 頁。

請產生 Login.cshtml 與 Register.cshtml 範例程式碼（可略寫部分標準 AdminLTE 標記，但需包含主要區塊與表單）。

text

---

## 六、Layout / JavaScript 共用模組 Prompts

### 18. 更新 _Layout.cshtml 使用 AdminLTE 5

請更新 src/Web/Views/Shared/_Layout.cshtml，使其整合 AdminLTE 5 作為主版面頁，需求如下：

引入 CSS：

Bootstrap 5

Font Awesome 6

AdminLTE 5 主題 CSS

DataTables、SweetAlert2、Toastr CSS（若以 CDN 引入，可直接示例）。

主結構包含：

上方導覽列 (Navbar)：顯示系統名稱與登入使用者資訊。

左側選單 (Sidebar)：選項包含「首頁」、「使用者管理」等，使用 treeview 結構。

內容區塊 (Content Wrapper)：@RenderBody()。

頁腳 (Footer)：顯示版權文字。

右上角使用者下拉選單顯示「帳號名稱」、「角色」、「登出」。

Sidebar 項目依角色顯示（可以先示範如何根據 User.IsInRole("Admin") 顯示或隱藏）。

引入 JS：

jQuery

Bootstrap Bundle

AdminLTE

DataTables、SweetAlert2、Toastr

保留 @RenderSection("Scripts", required: false) 讓各頁面可附加 JS。

請產生 _Layout.cshtml 範例程式碼（可使用 CDN 以簡化資源路徑）。

text

### 19. 建立共用 API 呼叫模組 /wwwroot/js/api.js

請在 src/Web/wwwroot/js/ 建立 api.js，共用 AJAX 呼叫邏輯，需求如下：

以 IIFE 或 ES module 撰寫簡單命名空間，例如 window.apiService。

提供函式：

getAuthToken()：從 localStorage 或 Cookie 取得 JWT Token（可示範兩種方式之一）。

get(url, onSuccess, onError)

post(url, data, onSuccess, onError)

put(url, data, onSuccess, onError)

del(url, onSuccess, onError)

內部使用 $.ajax，beforeSend 加入 Authorization: Bearer {token}。

統一處理 401（導向登入頁）、403（顯示權限不足訊息）、其他錯誤（Toastr 顯示錯誤）。

所有註解使用繁體中文。

請產生 api.js 完整程式碼。

text

### 20. 建立使用者列表互動腳本 user-index.js

請在 src/Web/wwwroot/js/ 建立 user-index.js，用於 User/Index.cshtml，需求如下：

在 DOM 準備完成後初始化 DataTables，從 /api/users 以 AJAX 取得資料。

columns 設定：

Username、Email、Role、IsActive、CreatedAt、Actions。

Role 與 IsActive 欄位需轉成 badge 顯示。

操作欄位：

使用 data-user-id 綁定使用者 Id。

提供檢視、編輯、刪除按鈕（可使用 Font Awesome icon）。

刪除按鈕行為：

使用 SweetAlert2 問是否確認刪除。

確認後呼叫 apiService.del("/api/users/" + id, ...)。

成功後重新載入 DataTables 並顯示 Toastr 成功訊息。

程式碼需有適當繁體中文註解。

請產生 user-index.js 完整程式碼。

text

---

## 七、測試層 Prompts

### 21. 建立 CreateUserCommandHandler 單元測試

請在 tests/Application.Tests/Features/UserManagement/ 建立 CreateUserCommandHandlerTests.cs，需求如下：

使用 xUnit、Moq、FluentAssertions。

測試案例：

Should_CreateUser_When_CommandIsValid

Should_ThrowDuplicateUserException_When_UsernameOrEmailExists

Should_HashPassword_When_CreatingUser

使用 Arrange-Act-Assert 結構。

Mock IUserRepository 與 IPasswordHasher：

確認 ExistsAsync 被正確呼叫。

確認 HashPassword 被呼叫一次。

斷言結果與例外狀況正確。

測試方法名稱使用英文，註解使用繁體中文。

請產生 CreateUserCommandHandlerTests.cs 完整程式碼。

text

### 22. 建立 UsersController Web API 整合測試

請在 tests/Web.Tests/Controllers/ 建立 UsersControllerTests.cs，需求如下：

使用 WebApplicationFactory<Program> 建立測試伺服器。

測試案例：

GetUsers_WithoutToken_ShouldReturnUnauthorized

GetUsers_WithValidToken_ShouldReturnOk

CreateUser_WithNonAdminRole_ShouldReturnForbidden

可以使用測試專用 JWT Token 或簡化為跳過實際驗證（視現有專案設定而定）。

使用 InMemoryDatabase 或測試資料庫初始化一些 User 資料。

使用 FluentAssertions 驗證 HTTP StatusCode 與輸出型別。

請產生 UsersControllerTests.cs 範例程式碼（可以略寫部分設定，但需包含核心測試邏輯）。

text

---

## 使用建議

- 將本檔案分段貼給 Copilot，視目前開發階段選擇對應 Prompt。
- 若一次產生變更超過約 200 行，可先請 Copilot 只產生某一層（例如 Command + Handler）。
- 完成後請自行執行 build 與測試，確保符合專案規範與預期行為。
- 若專案檔名 / namespace 與此模板不同，請你依實際專案調整路徑與命名。

---

## 附錄：快速索引

| 編號 | 類型 | 說明 |
|-----|------|------|
| 1-3 | Domain | User Entity、UserRole Enum、IUserRepository |
| 4-6 | Infrastructure | UserRepository、DbContext 配置、PasswordHasher |
| 7-9 | Application | Commands/Queries/Validators/Handlers |
| 10-12 | Web API | UserDto、UsersController、JWT 設定 |
| 13-17 | MVC Views | UserController、Index/Create/Edit/Details/Login/Register |
| 18-20 | Frontend | _Layout、api.js、user-index.js |
| 21-22 | Tests | 單元測試、整合測試 |
