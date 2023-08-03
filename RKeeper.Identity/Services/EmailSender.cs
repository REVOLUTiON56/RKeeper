using Microsoft.AspNetCore.Identity.UI.Services;

namespace RKeeper.Identity.Services;

public class EmailSender : IEmailSender {
    public Task SendEmailAsync(string email, string subject, string htmlMessage) {
        return Task.CompletedTask;
    }
}
