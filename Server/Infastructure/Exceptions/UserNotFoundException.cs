namespace Server.Infastructure.Exceptions;

public class UserNotFoundException : ServiceException
{
    public UserNotFoundException(string title)
        : base(404, title)
    {
    }
}