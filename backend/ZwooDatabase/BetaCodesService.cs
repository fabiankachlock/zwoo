using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing beta code related database operations
/// </summary>
public interface IBetaCodesService
{
    public bool ExistsBetaCode(string code);

    public bool RemoveBetaCode(string code);

}