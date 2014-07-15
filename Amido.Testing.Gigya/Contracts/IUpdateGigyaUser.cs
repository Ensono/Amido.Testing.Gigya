using System.Collections.Generic;

namespace Amido.Testing.Gigya.Contracts
{
    public interface IUpdateGigyaUser
    {
        IFinalizeRegistration SetAccountInfo(Dictionary<string, object> gigyaUserParts);
    }
}