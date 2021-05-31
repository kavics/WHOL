using System;

namespace Whol.Logic
{
    public class BuiltInUserManager : IUserManager
    {
        public User LoggedInUser { get; private set; }


        public string SignUp(string email, string password)
        {
            throw new NotImplementedException();
        }

        public void SignIn(string token)
        {
            throw new NotImplementedException();
        }

        public void SignOut(string email)
        {
            throw new NotImplementedException();
        }

        public void Forget(string email)
        {
            throw new NotImplementedException();
        }
    }
}
