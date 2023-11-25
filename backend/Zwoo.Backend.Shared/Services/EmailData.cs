using System.Reflection;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Zwoo.Backend.Shared.Configuration;

namespace Zwoo.Backend.Shared.Services;

public class EmailData
{
    private static string _TemplatesPrefix = "Zwoo.Backend.Templates";

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

    private static string evaluateTemplate(string tmpl, Dictionary<string, string> args)
    {
        foreach (var key in args.Keys)
        {
            tmpl = tmpl.Replace(key, args[key]);
        }
        return tmpl;
    }

    public static MailMessage PasswordChangeRequestEmail(string language, string username, string resetCode, ZwooOptions options)
    {
        string link = $"{(options.Server.UseSsl ? "https://" : "http://")}{options.Server.Domain}/reset-password?code={resetCode}";
        string domain = $"{(options.Server.UseSsl ? "https://" : "http://")}{options.Server.Domain}";
        Dictionary<string, string> args = new Dictionary<string, string>() {
            {"{{username}}", username},
            {"{{link}}", link},
            {"{{year}}", DateTime.Now.Year.ToString()},
            {"{{domain}}", domain},
        };

        string txtBody = _readTemplate($"{language}.resetPassword.txt");
        txtBody = evaluateTemplate(txtBody, args);
        string htmlBody = _readTemplate($"{language}.resetPassword.html");
        htmlBody = evaluateTemplate(htmlBody, args);

        var mail = new MailMessage();
        mail.Subject = _subjectsMap[$"{language}.resetPassword"];
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(txtBody, null, "text/plain"));
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html"));
        return mail;
    }

    public static MailMessage VerifyEmail(string language, string username, string puid, string code, ZwooOptions options)
    {
        string link = $"{(options.Server.UseSsl ? "https://" : "http://")}{options.Server.Domain}/verify-account?id={puid}&code={code}";
        string domain = $"{(options.Server.UseSsl ? "https://" : "http://")}{options.Server.Domain}";
        Dictionary<string, string> args = new Dictionary<string, string>() {
            {"{{username}}", username},
            {"{{link}}", link},
            {"{{year}}", DateTime.Now.Year.ToString()},
            {"{{domain}}", domain},
        };

        string txtBody = _readTemplate($"{language}.verifyAccount.txt");
        txtBody = evaluateTemplate(txtBody, args);
        string htmlBody = _readTemplate($"{language}.verifyAccount.html");
        htmlBody = evaluateTemplate(htmlBody, args);

        var mail = new MailMessage();
        mail.Subject = _subjectsMap[$"{language}.verify"];
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(txtBody, null, "text/plain"));
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html"));
        return mail;
    }
}