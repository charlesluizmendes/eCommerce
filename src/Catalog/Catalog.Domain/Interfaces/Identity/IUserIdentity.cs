namespace Catalog.Domain.Interfaces.Identity
{
    public interface IUserIdentity
    {
        string GetUserIdFromToken();
    }
}
