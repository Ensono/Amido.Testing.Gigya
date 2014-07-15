using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Amido.Testing.Gigya.Tests
{
    [TestFixture]
    public class Class1
    {
        public class GigyaUser
        {
            public string Uid { get; set; }
            public Profile Profile { get; set; }
            public Data Data { get; set; }
        }

        public class Profile
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NickName { get; set; }
        }

        public class Data
        {
            public string Terms { get; set; }
            public string BrandOptIn { get; set; }
            public string ThirdPartyOptIn { get; set; }
            public string NsOptIn { get; set; }
        }

        [Test]
        public void Test()
        {
            var service = new GigyaUserService();
            var user = service.GetUserByEmail<GigyaUser>(
                "3_ErYp0Q7yOZjGn2pteRSeHaXp5T7taqyXPoaoXW64VXNffTA9UkLF1f9I9QMcAot2", 
                "3TOkxN+xEn6Qlr7bkeyxLE5Dg7zoyF42fGViPDQNUsI=",
                "eu1.gigya.com",
                "sean@test.com",
                g => g != null);
        }
    }
}
