# 專案規則檔（.NET 10 C# Web）

這是本專案的開發規範，請 Copilot 在產生程式碼、重構或解釋時遵循以下規則。

## 基本指示

- 回應一律使用**繁體中文**。
- 程式碼範例使用 C# 語法，並符合 .NET 10 的慣例。
- 若建議的變更超過 200 行，請先提出變更計畫，並詢問「我打算這樣改，您覺得如何？」。
- 不要引入專案中未使用的 NuGet 套件，除非明確要求。

## 專案概述

本專案是一個 ASP.NET Core 10 Web API + MVC，提供使用者管理功能（註冊、登入、權限）。已實作 Clean Architecture 全層：Domain、Application、Infrastructure、Web。

### 主要功能

- 使用者 CRUD（建立、查詢、更新、刪除）
- JWT 認證與角色授權（Admin/User）
- 分頁查詢與資料驗證
- 密碼雜湊（BCrypt）

### 技術堆疊

- **語言**：C# 13+（.NET 10）
- **框架**：ASP.NET Core 10 Web
- **資料存取**：Entity Framework Core 10，使用 SQLite
- **架構**：Clean Architecture（CQRS + MediatR）
- **驗證**：FluentValidation
- **認證**：JWT Bearer
- **格式化**：EditorConfig + .NET Formatting

## 專案結構與角色

專案採用 Clean Architecture 分層，目錄結構如下：

```
src/
├── Application/          # 應用邏輯（CQRS Commands/Queries/Handlers/Validators）
├── Domain/               # 領域模型（Entities/Enums/Interfaces）
├── Infrastructure/       # 基礎設施（EF Core Repositories/Services）
└── Web/                  # API 層（Controllers/DTOs/Views）
```

- 新增功能時，建立 `src/Application/Features/{FeatureName}/` 子目錄。
- 共用元件放在 `src/Shared/` 或各層 Common。

## 命名與程式碼風格

- 類別、介面、方法、屬性：PascalCase。
- 私有欄位：_camelCase。
- 介面：I 前綴（如 `IUserRepository`）。
- DTO：{Entity}Dto（如 `UserDto`）。
- MediatR：Command `CreateUserCommand`，Query `GetUsersQuery`，Handler `{Command}Handler`。
- 非同步方法：以 `Async` 結尾。
- 每個 public 成員有 XML 註解（繁體中文）。
- 使用 `var` 當型別明確，否則明確型別。
- `using` 放在 namespace 內，按字母排序。
- Guard Clauses 避免巢狀。

## 關鍵模式與實作指引

### Domain 層

- Entity：私有建構子 + 工廠方法 `User.Create(...)`，狀態變更透過方法（如 `UpdateProfile`）。
- 範例：[src/Domain/Entities/User.cs](src/Domain/Entities/User.cs)

### Application 層

- Commands/Queries：record 類型，實作 `IRequest<T>`。
- Handlers：注入 Repository/Service，處理業務邏輯。
- Validators：繼承 `AbstractValidator<T>`，錯誤訊息繁體中文。
- 範例：[src/Application/Features/UserManagement/Commands/CreateUserCommandHandler.cs](src/Application/Features/UserManagement/Commands/CreateUserCommandHandler.cs)

### Infrastructure 層

- Repository：實作 Domain Interface，使用 EF Core AsNoTracking() 查詢。
- DbContext：Fluent API 設定唯一索引、主鍵、預設值。
- 範例：[src/Infrastructure/Data/ApplicationDbContext.cs](src/Infrastructure/Data/ApplicationDbContext.cs)

### Web 層

- Controllers：`[ApiController]` + `[Authorize]`，Admin 操作需 `[Authorize(Roles = "Admin")]`。
- 使用 `IMediator.Send()` 呼叫 Application 層。
- 範例：[src/Web/Controllers/UsersController.cs](src/Web/Controllers/UsersController.cs)

## 安全與品質

- API 輸入驗證，避免 SQL Injection/XSS。
- 敏感資訊從 `IConfiguration` 讀取，不寫死程式碼。
- 密碼僅儲存雜湊，不明文。
- 單元測試覆蓋率目標 80%（尚未實作）。

## 開發工作流程

- **建置**：`dotnet build`
- **執行**：`dotnet run`，預設 http://localhost:5001
- **測試**：尚未實作
- **格式化**：EditorConfig 自動格式化

## 禁止事項

- 禁止 `dynamic`、`object` 作為參數/傳回型別。
- Controller 不可直接存取 `DbContext`，一律透過 Repository。
- Application/Domain 層不可使用 `HttpContext`。

此文件隨專案發展更新。
