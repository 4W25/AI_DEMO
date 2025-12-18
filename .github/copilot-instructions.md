# 專案規則檔（.NET 9 C# Web API）

這是本專案的開發規範，請 Copilot 在產生程式碼、重構或解釋時遵循以下規則。

## 基本指示

- 回應一律使用**繁體中文**。
- 程式碼範例使用 C# 語法，並符合 .NET 9 的慣例。
- 若建議的變更超過 200 行，請先提出變更計畫，並詢問「我打算這樣改，您覺得如何？」。
- 不要引入專案中未使用的 NuGet 套件，除非明確要求。

## 專案概述

本專案是一個 ASP.NET Core 9 Web API，提供後端服務給前端應用程式。

### 主要功能

- 使用者管理（註冊、登入、權限）
- 資料 CRUD 操作
- 透過 Swagger 提供 API 文件
- 整合 JWT 認證與基本授權

### 技術堆疊

- **語言**：C# 13+（.NET 9）
- **框架**：ASP.NET Core 9（Web API）
- **資料存取**：Entity Framework Core 9，使用 SQL Server
- **架構**：Clean Architecture（分層：Application、Domain、Infrastructure、Web）
- **依賴注入**：使用內建 DI 容器
- **驗證**：FluentValidation 12+
- **中介層**：MediatR 12+（Commands/Queries）
- **測試**：xUnit 2.5+、Moq、FluentAssertions
- **格式化**：EditorConfig + .NET Formatting

## 專案結構與角色

專案採用 Clean Architecture 分層，目錄結構如下：

```
src/
├── Application/          # 應用邏輯（Commands、Queries、Handlers、Validators）
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   └── Validators/
├── Domain/               # 領域模型（Entities、Value Objects、Interfaces）
│   ├── Entities/
│   ├── ValueObjects/
│   └── Interfaces/
├── Infrastructure/       # 基礎設施（EF Core、Authentication、External Services）
│   ├── Data/
│   ├── Identity/
│   └── Services/
└── Web/                  # API 層（Controllers、DTOs、Filters、Startup）
    ├── Controllers/
    ├── DTOs/
    ├── Filters/
    └── Program.cs
tests/
├── Application.Tests/    # 應用層測試
├── Domain.Tests/         # 領域層測試
└── Web.Tests/            # API 層整合測試
```

- 新增功能時，請依功能建立子目錄（如 `src/Application/Features/UserManagement/`）。
- 共用元件（如通用工具、擴充方法）放在 `src/Shared/` 目錄。

## 命名與程式碼風格

### 命名規則

- 類別、介面、方法、屬性：PascalCase。
- 私有欄位、區域變數：camelCase，私有欄位可加 `_` 前綴（如 `_logger`）。
- 介面名稱以 `I` 開頭（如 `IUserRepository`）。
- DTO 命名：`{Entity}Dto`（如 `UserDto`、`CreateUserDto`）。
- MediatR 命名：
  - Command：`CreateUserCommand`、`UpdateUserCommand`
  - Query：`GetUserQuery`、`GetUsersQuery`
  - Handler：`CreateUserCommandHandler`、`GetUserQueryHandler`

### 程式碼風格

- 使用 `var` 當型別明確時，否則寫明確型別。
- `using` 指令放在 `namespace` 內部，並按字母排序。
- `async` 方法名稱以 `Async` 結尾（如 `GetUserAsync`）。
- 每個 `public` 方法與屬性都要有 XML 註解，說明用途與參數。
- 避免過度巢狀，善用 Guard Clauses 或 `return` 早期退出。

## 常見模式與實作指引

### Controller

- 使用 `[ApiController]` 與 `[Route("[controller]")]`。
- 動作方法使用 MediatR 的 `Send` 或 `SendAsync`。
- 錯誤處理統一使用 `ProblemDetails` 與自訂 exception filter。

### Application 層

- Command/Query 與 Handler 分開，Handler 命名為 `{Command/Query}Handler`。
- Handler 內不直接寫複雜商業邏輯，呼叫 Domain Service 或 Application Service。
- 驗證使用 FluentValidation，驗證規則寫在 `Validators/` 目錄。

### Domain 層

- Entity 使用 `public` 屬性，但建構子或方法控制狀態變更。
- Value Object 實作 `IEquatable<T>` 與 `==`/`!=` 運算子。
- 領域事件（Domain Events）放在 `Domain/Events/` 目錄。

### Infrastructure 層

- EF Core：
  - 使用 `IQueryable` 在 Repository 層，不在此時執行 `ToList`。
  - 複雜查詢使用 Specification Pattern 或 Query Object。
- 依賴注入：使用 constructor injection，避免 `IServiceProvider.GetService`。

## 安全與品質

- 所有 API 輸入都要驗證，避免 SQL Injection 與 XSS。
- 敏感資訊（如 API Key、連接字串）不得寫死在程式碼中，使用 `IConfiguration` 或環境變數。
- 產生的程式碼需符合 SonarQube / CodeQL 的基本規則。
- 單元測試覆蓋率目標 80% 以上，整合測試覆蓋主要流程。

## 測試撰寫方針

- 測試專案命名：`{Layer}.Tests`（如 `Application.Tests`）。
- 測試類別命名：`{ClassToTest}Tests`（如 `CreateUserCommandHandlerTests`）。
- 使用 `Arrange-Act-Assert` 結構。
- Mock 外部服務（如 `IUserRepository`、`IEmailService`）。
- 測試方法命名：`Should_{ExpectedBehavior}_When_{Condition}`（如 `Should_ReturnSuccess_When_UserIsValid`）。

## 禁止事項

- 禁止使用 `dynamic`，除非有明確需求。
- 原則上不使用 `object` 作為參數或傳回型別。
- 禁止在 Controller 中直接存取 `DbContext`，應透過 Repository 或 Service。
- 禁止在 Application/Domain 層使用 `HttpContext` 或 `IHttpContextAccessor`，應透過參數傳遞必要資訊。
