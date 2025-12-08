public class BookQueryDto
{
    public string? Title { get; set; }
    public int? AuthorId { get; set; }
    public string? ISBN { get; set; }
    public string? SortBy { get; set; } = "Title";
    public bool Desc { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
