namespace AI_DEMO.Application.Common.Exceptions;

/// <summary>
/// 當嘗試建立重複使用者時拋出的例外。
/// </summary>
public class DuplicateUserException : Exception
{
    /// <summary>
    /// 初始化 DuplicateUserException 的新實例。
    /// </summary>
    /// <param name="message">例外訊息。</param>
    public DuplicateUserException(string message) : base(message)
    {
    }
}