namespace Amido.Testing.Gigya.Contracts
{
    public interface IRegisterUser
    {
        IUpdateGigyaUser Register(string email, string password);
    }
}