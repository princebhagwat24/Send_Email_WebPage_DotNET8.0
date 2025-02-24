using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace EmailSendingApp.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public async Task SendEmailAsync(string toEmail, string subject, string body)
    //{
    //    // Retrieve email settings from configuration
    //    var emailSettings = _configuration.GetSection("EmailSettings");
    //    var smtpServer = emailSettings.GetValue<string>("SmtpServer");
    //    var smtpPort = emailSettings.GetValue<int>("SmtpPort");
    //    var smtpUsername = emailSettings.GetValue<string>("SmtpUsername");
    //    var smtpPassword = emailSettings.GetValue<string>("SmtpPassword");

    //    var fromEmail = smtpUsername;
    //    var fromName = "Prince";

    //    var mailMessage = new MailMessage
    //    {
    //        From = new MailAddress(fromEmail, fromName),
    //        Subject = subject,
    //        Body = body,
    //        IsBodyHtml = true
    //    };

    //    mailMessage.To.Add(toEmail);

    //    // Use the correct port and SSL settings from your configuration
    //    using (var smtpClient = new SmtpClient(smtpServer, smtpPort)  // Use the port from settings
    //    {
    //        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
    //        EnableSsl = true  // Ensure SSL is enabled


    //    })
    //    {
    //        try
    //        {
    //            await smtpClient.SendMailAsync(mailMessage);
    //        }
    //        catch (SmtpException ex)
    //        {
    //            // Handle SMTP exceptions here (logging, rethrowing, etc.)
    //            throw new Exception("An error occurred while sending the email.", ex);
    //        }
    //    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings.GetValue<string>("SmtpServer");
        var smtpPort = emailSettings.GetValue<int>("SmtpPort");
        var smtpUsername = emailSettings.GetValue<string>("SmtpUsername");
        var smtpPassword = emailSettings.GetValue<string>("SmtpPassword");

        var fromEmail = smtpUsername;
        var fromName = "Prince";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            try
            {
                // Connect to the SMTP server with STARTTLS
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while sending the email.", ex);
            }
        }
    }
}
