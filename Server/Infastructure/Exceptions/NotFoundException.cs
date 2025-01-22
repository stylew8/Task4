namespace Server.Infastructure.Exceptions;

public class NotFoundException : ServiceException
{
    public NotFoundException(string title) 
        : base(404, title)
    {
    }
}