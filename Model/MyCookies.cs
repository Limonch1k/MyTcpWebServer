using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossPlatformProtectedData;
using System.Security.Cryptography;
using ProtectedData = CrossPlatformProtectedData.ProtectedData;
using CustomWebServer;

namespace Model
{
    public class MyCookies
    {
        internal Dictionary<string, string> Cookie { get; set; } = new();

        public void SetCookie(string id, string name) 
        {
            byte[] entropy = Encoding.Unicode.GetBytes("Salt Is Not A Password");
            byte[] cipherText = ProtectedData.Protect(Encoding.Unicode.GetBytes(name), entropy, 0);
            var str = Convert.ToBase64String(cipherText);
            Console.WriteLine(str.Length);

            Cookie.Add(id, id + "=" + str);
            //str = Unprotect(cipherText);
            return ;
        }
        public string TryParseUserCookie(string id) 
        {
            string str = Cookie[" " + id.Trim()];
            return Unprotect(Convert.FromBase64String(str));
        }

        private string Unprotect(byte[] data) 
        {
            byte[] entropy = Encoding.Unicode.GetBytes("Salt Is Not A Password");
            byte[] plaintext = ProtectedData.Unprotect(data, entropy,0);
            return Encoding.UTF8.GetString(plaintext);
        }

        internal string GetHeadersContent() 
        {
            string str = "";
            foreach (var a in Cookie) 
            {
                var b = a.Value.Split("=");
                str += "Set-Cookie: " + b[0] + "=" + b[1] +  ";\r\n"; 
            }

            return str;
        }
    }
}
