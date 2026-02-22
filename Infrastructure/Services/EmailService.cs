using Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task SendVerificationEmailAsync(string toEmail, string verificationToken, CancellationToken cancellationToken)
    {
        var authMethod = new TokenAuthMethodInfo("root");
        var vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod);
        var vaultClient = new VaultClient(vaultClientSettings);

        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "smtp", mountPoint: "secret");

        var smtpHost = secret.Data.Data["Host"].ToString();
        var smtpPort = int.Parse(secret.Data.Data["Port"].ToString()!);
        var smtpUser = secret.Data.Data["User"].ToString();
        var smtpPass = secret.Data.Data["Password"].ToString();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MedCore", smtpUser!));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Activa tu cuenta en MedCore";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"<p>Tu código de activación es: <strong>{verificationToken}</strong></p>"
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(smtpUser, smtpPass, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
