namespace BiegusowoApi.Helpers;

public class PaginatedList<T>(List<T> items, int pageIndex, int totalPages)
{
    public List<T> Items { get; } = items;
    public int PageIndex { get; } = pageIndex;
    public int TotalPages { get; } = totalPages;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}

public static class PaginationHelper
{
    public static int CalculateSkipNumber(int pageSize, int pageNumber)
    {
        return (pageNumber - 1) * pageSize;
    }
}
