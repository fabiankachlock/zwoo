namespace ZwooBackend.Controllers.DTO;

public class CreateAccount
{
    public string username { get; set; } = "";
    public string email { get; set; } = "";
    public string password { get; set; } = "";
    public string? code { get; set; } = "";
    
}