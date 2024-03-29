using MongoDB.Driver;
using Zwoo.Database.Dao;
using Microsoft.Extensions.Logging;

namespace Zwoo.Database;

/// <summary>
/// a service for executing audit trails related database operations
/// </summary>
public interface IAuditTrailService
{
    /// <summary>
    /// create a new audit trail event
    /// </summary>
    /// <param name="id">the trails id</param>
    /// <param name="actor">the actor which executed the event</param>
    /// <param name="message">a messsage reasoning the change</param>
    /// <param name="newValue">the new value with changes</param>
    /// <param name="oldValue">the old value</param>
    public void Protocol(string id, string actor, string message, object newValue, object? oldValue);


    /// <summary>
    /// create a new audit trail event
    /// </summary>
    /// <param name="id">the trails id</param>
    /// <param name="event">the audit event</param>
    public void Protocol(string id, AuditEventDao @event);

    /// <summary>
    /// return the audit trail for an entity
    /// </summary>
    /// <param name="id">the trail id</param>
    /// <returns>the trail if present or null</returns>
    public AuditTrailDao? GetProtocol(string id);

    /// <summary>
    /// create a trail id for an user
    /// </summary>
    /// <param name="user">the user for which the id should get created</param>
    public string GetAuditId(UserDao user);
    public string GetAuditId(DeletedUserDao user);
    public string GetAuditId(ulong userId);
}

public class AuditTrailService : IAuditTrailService
{
    private readonly IDatabase _db;
    private readonly ILogger<AuditTrailService> _logger;

    public AuditTrailService(IDatabase db, ILogger<AuditTrailService> logger)
    {
        _db = db;
        _logger = logger;
    }

    private void ensureDao(string id)
    {
        if (_db.AuditTrails.AsQueryable().FirstOrDefault(dao => dao.Id == id) == null)
        {
            _logger.LogInformation($"creating audit trail for {id}");
            _db.AuditTrails.InsertOne(new AuditTrailDao()
            {
                Id = id,
                Events = new(),
            });
        }
    }

    public void Protocol(string id, string actor, string message, object newValue, object? oldValue)
    {
        ensureDao(id);
        _logger.LogDebug($"creating audit trail event for {id}");
        var newEvent = new AuditEventDao(actor, message, DateTimeOffset.Now.ToUnixTimeMilliseconds(), newValue, oldValue);
        _db.AuditTrails.UpdateOne(trail => trail.Id == id, Builders<AuditTrailDao>.Update.Push(trail => trail.Events, newEvent));
    }

    public void Protocol(string id, AuditEventDao data)
    {
        ensureDao(id);
        _logger.LogDebug($"creating audit trail event for {id}");
        _db.AuditTrails.UpdateOne(trail => trail.Id == id, Builders<AuditTrailDao>.Update.Push(trail => trail.Events, data));
    }

    public AuditTrailDao? GetProtocol(string id)
    {
        return _db.AuditTrails.AsQueryable().FirstOrDefault(trail => trail.Id == id);
    }

    public string GetAuditId(UserDao user)
    {
        return user.Id.ToString();
    }

    public string GetAuditId(DeletedUserDao user)
    {
        return user.Id.ToString();
    }

    public string GetAuditId(ulong userId)
    {
        return userId.ToString();
    }
}