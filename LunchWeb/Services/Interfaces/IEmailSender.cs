namespace LunchWeb.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
