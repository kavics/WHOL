using System;
using System.Diagnostics.CodeAnalysis;

namespace Whol.Logic.Tests.Implementations
{
    [ExcludeFromCodeCoverage]
    class TestUserManager : IUserManager
    {
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
