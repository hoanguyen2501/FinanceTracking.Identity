namespace FinanceTracking.Identity.DTOs
{
    public sealed class LoginDto
    {
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}