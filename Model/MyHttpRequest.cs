using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http.HttpRequest
{
    public class MyHttpRequest
    {
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();

        public MyCookies Cookies { get; set; } = new MyCookies();

        public string Url { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }

       public string Path { get; set; }
        
    }
}
