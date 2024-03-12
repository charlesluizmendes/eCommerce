namespace Payment.Domain.Core
{
    public class NotificationContext
    {
        private readonly Notification _notification;
        public Notification Notification => _notification;
        public bool HasNotifications => _notification.Errors.Count > 0;

        public NotificationContext()
        {
            _notification = new Notification();
        }

        public void AddNotification(string message)
        {
            _notification.Errors.Add(message);
        }
    }
}
