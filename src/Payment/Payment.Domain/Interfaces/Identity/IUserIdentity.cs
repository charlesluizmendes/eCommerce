namespace Payment.Domain.Interfaces.Identity
{
    public interface IUserIdentity
    {
        string GetUserIdFromToken();
    }
}
