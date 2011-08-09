using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace EyePatch.Core.Util
{
    public static class MD5
    {
        public static string Encode(string text)
        {
            //convert to char array
            char[] chars = text.ToCharArray();

            //get bytes from char array
            var bytes = Encoding.Unicode.GetBytes(text.ToCharArray());

            //get MD5 hash value
            var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);

            //convert byte array into hashed string
            return hash.Aggregate(new StringBuilder(32), (sb, b) => sb.Append(b.ToString("X2"))).ToString();
        }
    }
}
