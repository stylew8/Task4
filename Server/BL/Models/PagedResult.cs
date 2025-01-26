namespace Server.BL.Models;

public class PagedResult<T>
{
    public List<T> Users { get; set; }
    public int TotalCount { get; set; }

    public PagedResult(List<T> users, int totalCount)
    {
        Users = users;
        TotalCount = totalCount;
    }
}