using System.Reflection;
using System.Net.Mail;

namespace ZwooBackend.Services;

public class EmailData
{
    private static string _TemplatesPrefix = "ZwooBackend.Templates";

    private static string _readTemplate(string templateName)
    {
        Assembly asm = Assembly.GetExecutingAssembly()!;
        Stream stream = asm.GetManifestResourceStream($"{_TemplatesPrefix}.{templateName}")!;
        StreamReader source = new StreamReader(stream);
        string fileContent = source.ReadToEnd();
        source.Dispose();
        stream.Dispose();
        return fileContent;
    }

    private static readonly Dictionary<string, string> _subjectsMap = new Dictionary<string, string>() {
        { "de.verify", "ZWOO Account Verifizierung" },
        { "en.verify", "ZWOO Account Verification" },
        { "de.resetPassword", "ZWOO Passwort Änderung" },
        { "en.resetPassword", "Change your ZWOO password" },
    };

    public static MailMessage PasswordChangeRequestEmail(string language, string username, string resetCode)
    {
        string link = $"{(Globals.UseSsl ? "https://" : "http://")}{Globals.ZwooDomain}/reset-password?code={resetCode}";

        string txtBody = _readTemplate($"{language}.resetPassword.txt");
        txtBody = txtBody.Replace("{{username}}", username);
        txtBody = txtBody.Replace("{{link}}", link);
        txtBody = txtBody.Replace("{{year}}", DateTime.Now.Year.ToString());
        string htmlBody = _readTemplate($"{language}.resetPassword.html");
        htmlBody = htmlBody.Replace("{{username}}", username);
        htmlBody = htmlBody.Replace("{{link}}", link);
        htmlBody = htmlBody.Replace("{{year}}", DateTime.Now.Year.ToString());

        var mail = new MailMessage();
        mail.Subject = _subjectsMap[$"{language}.resetPassword"];
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(txtBody, null, "text/plain"));
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html"));
        return mail;
    }

    public static MailMessage VerifyEmail(string language, string username, string puid, string code)
    {
        string link = $"{(Globals.UseSsl ? "https://" : "http://")}{Globals.ZwooDomain}/verify-account?id={puid}&code={code}";

        string txtBody = _readTemplate($"{language}.verifyAccount.txt");
        txtBody = txtBody.Replace("{{username}}", username);
        txtBody = txtBody.Replace("{{link}}", link);
        txtBody = txtBody.Replace("{{year}}", DateTime.Now.Year.ToString());
        string htmlBody = _readTemplate($"{language}.verifyAccount.html");
        htmlBody = htmlBody.Replace("{{username}}", username);
        htmlBody = htmlBody.Replace("{{link}}", link);
        htmlBody = htmlBody.Replace("{{year}}", DateTime.Now.Year.ToString());

        var mail = new MailMessage();
        mail.Subject = _subjectsMap[$"{language}.verify"];
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(txtBody, null, "text/plain"));
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html"));
        return mail;
    }
}