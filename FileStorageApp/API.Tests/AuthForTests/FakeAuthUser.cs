using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace API.Tests.AuthForTests
{
    public class FakeAuthUser
    {
        public List<Claim> Claims { get; }

        public FakeAuthUser(params Claim[] claims) => Claims = claims.ToList();
    }
}