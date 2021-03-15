using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
