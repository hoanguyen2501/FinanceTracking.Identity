namespace FinanceTracking.Identity.Helpers
{
    public sealed class JwtSettings
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string SigningKey { get; set; } = default!;
        public int Expiration { get; set; } = 60;
    }
}