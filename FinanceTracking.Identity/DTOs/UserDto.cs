namespace FinanceTracking.Identity.DTOs
{
    public sealed class UserDto
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
    }
}