namespace Server.Infastructure.Exceptions;

public class UserBlockedException : ServiceException
{
    public UserBlockedException(string title) : base(403, title)
    {
    }
}