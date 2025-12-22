namespace AI_DEMO.Application.Common.Models;

/// <summary>
/// 分頁結果模型，包含項目清單與分頁資訊。
/// </summary>
/// <typeparam name="T">項目型別。</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 項目清單。
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// 總項目數。
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// 目前頁碼。
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// 每頁項目數。
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// 總頁數。
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 初始化 PagedResult 的新實例。
    /// </summary>
    /// <param name="items">項目清單。</param>
    /// <param name="totalCount">總項目數。</param>
    /// <param name="pageNumber">目前頁碼。</param>
    /// <param name="pageSize">每頁項目數。</param>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}