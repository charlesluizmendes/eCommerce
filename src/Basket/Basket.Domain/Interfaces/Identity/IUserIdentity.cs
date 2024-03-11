namespace Basket.Domain.Interfaces.Identity
{
    public interface IUserIdentity
    {
        string GetUserIdFromToken();
    }
}
