namespace Server.BL.Interfaces;

public interface IEncrypt
{
    string HashPassword(string password, string salt);
}