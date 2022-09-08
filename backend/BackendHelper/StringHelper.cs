using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BackendHelper;

public static class StringHelper
{
    public static string GenerateNDigitString(int len)
    {
        var rand = new Random();
        var ss = new StringWriter();
        foreach (var _ in Enumerable.Range(0, len))
            ss.Write(Convert.ToString(rand.Next(0, 9)));
        return ss.ToString();
    }

    public static bool IsValidPassword(string pw)
    {
        if ( pw.Length < 8 || pw.Length > 50 )
            return false;

        var m1 = Regex.Match(pw, "[0-9]+");
        var m2 = Regex.Match(pw, "[@!#$%&'*+/=?^_´{|}\\-[\\]]+");
        var m3 = Regex.Match(pw, "[a-zA-Z]+");

        return m1.Success && m2.Success && m3.Success;
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            MailAddress _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool IsValidUsername(string username)
    {
        if ( username.Length < 4 )
            return false;
        if ( username.Length > 20 )
            return false;
        else
            return true;
    }
}