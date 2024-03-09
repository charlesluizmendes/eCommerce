namespace Order.Domain.Interfaces.Proxys
{
    public interface IEmailProxy
    {
        Task SendAsync(string from, string to, string subject, string html);
    }
}
