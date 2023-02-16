using System.Security.Cryptography;
using System.Text;

namespace Glb.Common.Cryptography;

public static class Encryption
{
    public static string CalculateSHA256(string strData)
    {
        var message = Encoding.UTF8.GetBytes(strData);
        using (var alg = SHA256.Create())
        {
            string hex = "";

            var hashValue = alg.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += string.Format("{0:x2}", x);
            }
            return hex;
        }
    }
    public static string CalculateMD5(string strData)
    {
                var message = Encoding.UTF8.GetBytes(strData);
        using (var alg = MD5.Create())
        {
            string hex = "";

            var hashValue = alg.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += string.Format("{0:x2}", x);
            }
            return hex;
        }
    }
}