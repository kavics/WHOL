namespace Whol.Logic;

public interface IUserManager
{
    string SignUp(string email, string password); // returns token
    void SignIn(string token); // throws CannotConnectException
    void SignOut(string email);
    void Forget(string email);
}