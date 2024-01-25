using Zwoo.Database.Dao;

namespace Zwoo.Database;


public struct AuditOptions
{

    public string? Actor { get; set; }

    public string? Message { get; set; }

    public AuditOptions(string? actor, string? message)
    {
        Actor = actor;
        Message = message;
    }

    public static AuditOptions WithActor(string name)
    {
        return new AuditOptions(name, null);
    }

    public AuditOptions AddActor(string name)
    {
        return new AuditOptions(name, Message);
    }

    public static AuditOptions WithMessage(string message)
    {
        return new AuditOptions(null, message);
    }

    public AuditOptions AddMessage(string message)
    {
        return new AuditOptions(Actor, message);
    }

    public AuditOptions Merge(AuditOptions? options)
    {
        return new AuditOptions()
        {
            Actor = options?.Actor ?? Actor,
            Message = options?.Message ?? Message,
        };
    }

    internal AuditEventDao ToEvent(AuditEventDao? givenAuditEvent = null)
    {
        var auditEvent = givenAuditEvent ?? new AuditEventDao();

        if (Actor != null)
        {
            auditEvent.Actor = Actor;
        }
        if (Message != null)
        {
            auditEvent.Message = Message;
        }

        return auditEvent;
    }

    internal static AuditEventDao CreateEvent(AuditOptions? options, AuditEventDao data)
    {
        return (options ?? new AuditOptions()).ToEvent(data);
    }
}