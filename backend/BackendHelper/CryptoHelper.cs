using System.Security.Cryptography;
using System.Text;

namespace BackendHelper;

public class CryptoHelper
{
    private static byte[] key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ZWOO_ENCRYPTION_KEY"));
    
    public static byte[] Encrypt(byte[] str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));
        byte[] arr = new byte[str.Length];
        for (var i = 0; i < str.Length; i++)
            arr[i] = (byte)((uint)str[i] ^ key[i % key.Length]);
        return arr;
    }

    public static byte[] Decrypt(byte[] str)
    {
        return Encrypt(str);
    }
}