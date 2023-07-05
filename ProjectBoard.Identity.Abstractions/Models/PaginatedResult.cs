namespace ProjectBoard.Identity.Abstractions.Models;

public class PaginatedResult<T>
{
    public List<T> Result { get; set; } = new List<T>();
    public string? NextPageKey { get; set; } = string.Empty;
}