using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace Zwoo.Database;

/// <summary>
/// a service for executing beta code related database operations
/// </summary>
public interface IBetaCodesService
{
    /// <summary>
    /// check whether a beta code exists in the db
    /// </summary>
    /// <param name="code">the code to check</param>
    public bool ExistsBetaCode(string code);

    /// <summary>
    /// remove a beta code from the db
    /// </summary>
    /// <param name="code">the code to remove</param>
    public bool RemoveBetaCode(string code);
}


public class BetaCodesService : IBetaCodesService
{
    private readonly IDatabase _db;
    private readonly ILogger<BetaCodesService> _logger;

    public BetaCodesService(IDatabase db, ILogger<BetaCodesService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public bool ExistsBetaCode(string betaCode)
    {
        _logger.LogDebug($"checking beta code {betaCode}");
        return _db.BetaCodes.AsQueryable().FirstOrDefault(x => x.Code == betaCode) != null;
    }

    public bool RemoveBetaCode(string betaCode)
    {
        _logger.LogInformation($"removing beta code {betaCode}");
        return _db.BetaCodes.DeleteOne(x => betaCode == x.Code).DeletedCount != 0;
    }
}