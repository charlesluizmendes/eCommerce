namespace Identity.Infraestructure.Options
{
    public class AccessTokenConfiguration
    {
        public string Secret { get; set; }

        public string Iss { get; set; }

        public string Aud { get; set; }
    }
}
