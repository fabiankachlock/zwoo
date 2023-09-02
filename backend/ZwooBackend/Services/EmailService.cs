using System;
using System.Net.Mail;
using System.Net;
using System.Collections.Concurrent;
using ZwooDatabase.Dao;
using log4net;

namespace ZwooBackend.Services;

public interface IRecipient
{
    public string Email { get; }
    public string Username { get; }
    public LanguageCode PreferredLanguage { get; }
}

/// <summary>
/// an service for sending email in zwoo
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// send a password reset email to a user
    /// </summary>
    /// <param name="recipient">the recipient</param>
    /// <param name="resetCode">the recipients password reset code</param>
    public void SendPasswordResetMail(IRecipient recipient, string resetCode);

    /// <summary>
    /// send a account verification email to a user
    /// </summary>
    /// <param name="recipient">the recipient</param>
    /// <param name="userId">the recipients user id</param>
    /// <param name="code">the recipients verification code</param>
    public void SendVerifyMail(IRecipient recipient, ulong userId, string code);

    /// <summary>
    /// send an email trigger by a contact form submit
    /// </summary>
    /// <param name="recipient">the recipient</param>
    /// <param name="contactRequest">the form submission data</param>
    public void SendContactFormEmail(IRecipient recipient, ContactRequest contactRequest);

    /// <summary>
    /// create a recipient object
    /// </summary>
    /// <param name="email">the recipients email address</param>
    /// <param name="username">the recipients user name</param>
    /// <param name="language">the recipients preferred language</param>
    /// <returns></returns>
    IRecipient CreateRecipient(string email, string username, LanguageCode language);
}


public class EmailService : IHostedService, IEmailService
{
    private ConcurrentQueue<MailMessage> _emailQueue;
    private Thread? _activeEmailThread = null;
    private object _threadLock = new Object();
    private ILog _logger = LogManager.GetLogger("EmailService");
    private ILanguageService _languageService;

    public EmailService(ILanguageService languageService)
    {
        _emailQueue = new ConcurrentQueue<MailMessage>();
        _languageService = languageService;
    }

    private struct Recipient : IRecipient
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public LanguageCode PreferredLanguage { get; set; }
    }

    public IRecipient CreateRecipient(string email, string username, LanguageCode language)
    {
        return new Recipient()
        {
            Email = email,
            Username = username,
            PreferredLanguage = language,
        };
    }

    public void SendPasswordResetMail(IRecipient recipient, string resetCode)
    {
        MailMessage mail = EmailData.PasswordChangeRequestEmail(_languageService.CodeToString(recipient.PreferredLanguage), recipient.Username, resetCode);
        mail.From = new MailAddress(Globals.SmtpHostEmail);
        mail.To.Add(new MailAddress(recipient.Email));
        _emailQueue.Enqueue(mail);
        TryStartWork();
    }

    public void SendVerifyMail(IRecipient recipient, ulong userId, string code)
    {
        MailMessage mail = EmailData.VerifyEmail(_languageService.CodeToString(recipient.PreferredLanguage), recipient.Username, $"{userId}", code);
        mail.From = new MailAddress(Globals.SmtpHostEmail);
        mail.To.Add(new MailAddress(recipient.Email));
        _emailQueue.Enqueue(mail);
        TryStartWork();
    }

    public void SendContactFormEmail(IRecipient recipient, ContactRequest request)
    {
        MailMessage mail = new MailMessage();
        mail.Subject = $"ContactForm Submit from: {request.Name} ({request.Origin})";
        mail.Body = $"From: '{request.Name}' ({request.Email})\n";
        mail.Body += $"Submitted: {request.Origin} at {DateTimeOffset.FromUnixTimeMilliseconds(request.Timestamp).ToString("dd.MM.yy HH:mm:ss")}\n";
        mail.Body += $"Captcha Score: {request.CaptchaScore} / 1\n";
        mail.Body += $"Message:\n{request.Message}\n\n";
        mail.From = new MailAddress(Globals.SmtpHostEmail);
        mail.To.Add(new MailAddress(recipient.Email));
        _emailQueue.Enqueue(mail);
        TryStartWork();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        TryStartWork();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            lock (_threadLock)
            {
                if (_activeEmailThread != null)
                {
                    _activeEmailThread.Join();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warn("an error happened while stopping the service", ex);
        }
        return Task.CompletedTask;
    }

    private void TryStartWork()
    {
        try
        {
            lock (_threadLock)
            {
                if (_activeEmailThread?.ThreadState == ThreadState.Stopped)
                {
                    _activeEmailThread?.Join();
                    _activeEmailThread = null;
                }
                if (_activeEmailThread == null)
                {
                    _activeEmailThread = new Thread(() => SendMails());
                    _activeEmailThread.Start();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("cant start email thread", ex);
        }
    }

    private void SendMails()
    {
        _logger.Info("start sending emails");
        var client = new SmtpClient(Globals.SmtpHostUrl, Globals.SmtpHostPort);
        client.EnableSsl = false;
        client.Credentials = new NetworkCredential(Globals.SmtpUsername, Globals.SmtpPassword);

        while (_emailQueue.TryDequeue(out var message))
        {
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                _logger.Error("cant send email", ex);
            }
        }
        _logger.Info("email queue finished");
    }
}