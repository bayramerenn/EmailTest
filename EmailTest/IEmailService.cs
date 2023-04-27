namespace EmailTest
{
    public interface IEmailService
    {
        Task<bool> SendChangePassword(string to, string token);

        Task<EmailCodeDto> SendCode(string to);

        Task<bool> SendGeneratePassword(string to, string password);
    }

    public class EmailCodeDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public bool Result { get; set; }
    }
}
