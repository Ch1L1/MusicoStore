using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebMVC.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"--- EMAIL FOR: {email} ---");
        Console.WriteLine($"SUBJECT: {subject}");
        Console.WriteLine($"CONTENT: {htmlMessage}");
        Console.WriteLine("-----------------------------");
        return Task.CompletedTask;
    }
}
