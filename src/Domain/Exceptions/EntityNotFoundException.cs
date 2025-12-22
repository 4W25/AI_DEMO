namespace AI_DEMO.Domain.Exceptions;

/// <summary>
/// 當實體不存在時拋出的例外。
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// 初始化 EntityNotFoundException 的新實例。
    /// </summary>
    /// <param name="message">例外訊息。</param>
    public EntityNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// 初始化 EntityNotFoundException 的新實例。
    /// </summary>
    /// <param name="message">例外訊息。</param>
    /// <param name="innerException">內部例外。</param>
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}