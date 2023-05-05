using MongoDB.Driver;


namespace ZwooDatabase;

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
    private IDatabase _db;

    public BetaCodesService(IDatabase db)
    {
        _db = db;
    }

    public bool ExistsBetaCode(string betaCode)
    {
        return _db.BetaCodes.AsQueryable().FirstOrDefault(x => x.Code == betaCode) != null;
    }

    public bool RemoveBetaCode(string betaCode)
    {
        return _db.BetaCodes.DeleteOne(x => betaCode == x.Code).DeletedCount != 0;
    }
}