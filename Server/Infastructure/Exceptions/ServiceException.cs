namespace Server.Infastructure.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(int statusCode, string title)
    {
        StatusCode = statusCode;
        Title = title;
    }

    public int StatusCode { get; set; }
    public string Title { get; set; }
}