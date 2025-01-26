namespace Server.Infastructure.Exceptions;

public class AuthRequiredException : ServiceException
{
    public AuthRequiredException(string title) : base(403, title)
    {
    }
}