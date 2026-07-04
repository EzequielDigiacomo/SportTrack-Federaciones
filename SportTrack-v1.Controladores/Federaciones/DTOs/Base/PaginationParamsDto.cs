namespace SportTrack_v1.Entidades.DTOs.Base;

public class PaginationParamsDto
{
    private const int MaxPageSize = 100;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string? SearchTerm { get; set; }
}
