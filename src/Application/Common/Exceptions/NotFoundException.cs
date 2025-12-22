namespace AI_DEMO.Application.Common.Exceptions;

/// <summary>
/// 當資源不存在時拋出的例外。
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// 初始化 NotFoundException 的新實例。
    /// </summary>
    /// <param name="message">例外訊息。</param>
    public NotFoundException(string message) : base(message)
    {
    }
}