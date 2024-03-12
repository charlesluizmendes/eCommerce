namespace Catalog.Domain.Core
{
    public class Notification
    {
        public List<string> Errors { get; set; }

        public Notification()
        {
            Errors = new List<string>();
        }
    }
}
