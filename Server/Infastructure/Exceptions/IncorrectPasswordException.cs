namespace Server.Infastructure.Exceptions;

public class IncorrectPasswordException : ServiceException
{
    public IncorrectPasswordException(string title)
        : base(403, title)
    {
    }
}