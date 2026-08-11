namespace BiegusowoApi.Shared.Helpers;

public class CursorPaginatedList<T>(List<T> items, bool hasNextPage)
{
    public List<T> Items { get; } = items;
    public bool HasNextPage { get; } = hasNextPage;
}

