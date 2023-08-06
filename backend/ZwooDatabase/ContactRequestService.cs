using MongoDB.Driver;
using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for contact request related operations
/// </summary>
public interface IContactRequestService
{
    /// <summary>
    /// create a new contact request
    /// </summary>
    /// <param name="data">the contact request</param>
    public ContactRequest CreateRequest(ContactRequest data);
}


public class ContactRequestService : IContactRequestService
{
    private readonly IDatabase _db;

    public ContactRequestService(IDatabase db)
    {
        _db = db;
    }

    public ContactRequest CreateRequest(ContactRequest data)
    {
        ContactRequest request = new ContactRequest()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Name = data.Name,
            Email = data.Email,
            Message = data.Message,
            CaptchaScore = data.CaptchaScore,
            Origin = data.Origin
        };
        _db.ContactRequests.InsertOne(request);
        return request;
    }
}
