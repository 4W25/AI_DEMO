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
