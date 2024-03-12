namespace Identity.Infraestructure.Options
{
    public class AccessTokenConfiguration
    {
        public string Secret { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
