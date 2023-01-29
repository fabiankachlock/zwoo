using System.Net.Mail;
using System.Net;
using System.Collections.Concurrent;
using log4net;

namespace ZwooBackend.Services;

/// <summary>
/// an service for sending email in zwoo
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// send a password reset email to a user
    /// </summary>
    /// <param name="to">the recipients email</param>
    /// <param name="username">the recipients username</param>
    /// <param name="resetCode">the recipients password reset code</param>
    public void SendPasswordResetMail(string to, string username, string resetCode);

    /// <summary>
    /// send a account verification email to a user
    /// </summary>
    /// <param name="to">the recipients email</param>
    /// <param name="username">the recipients username</param>
    /// <param name="userId">the recipients user id</param>
    /// <param name="code">the recipients verification code</param>
    public void SendVerifyMail(string to, string username, ulong userId, string code);

}


public class EmailService : IHostedService, IEmailService
{
    private SmtpClient _client;
    private ConcurrentQueue<MailMessage> _emailQueue;
    private Thread? _activeEmailThread = null;
    private object _threadLock = new Object();
    private ILog _logger = LogManager.GetLogger("EmailService");

    public EmailService()
    {
        _emailQueue = new ConcurrentQueue<MailMessage>();
        _client = new SmtpClient(Globals.SmtpHostUrl, Globals.SmtpHostPort);
        _client.Credentials = new NetworkCredential(Globals.SmtpUsername, Globals.SmtpPassword);
    }

    public void SendPasswordResetMail(string to, string username, string resetCode)
    {
        throw new NotImplementedException();
    }

    public void SendVerifyMail(string to, string username, ulong userId, string code)
    {
        throw new NotImplementedException();
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
        while (_emailQueue.TryDequeue(out var message))
        {
            try
            {
                _client.Send(message);
            }
            catch (Exception ex)
            {
                _logger.Error("cant send email", ex);
            }
        }
        _logger.Info("email queue finished");
    }
}