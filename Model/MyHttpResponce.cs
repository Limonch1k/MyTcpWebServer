using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http.HttpResponce
{
    public class MyHttpResponce
    {
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        //public string Url { get; set; }

        public Dictionary<string, string> MethodList { get; set;}

        //public string Method { get; set; }

        public string Body { get; set; }

        public MyCookies Cookies { get; set; } = new MyCookies();

        public MyHttpResponce() 
        {
            Headers = new Dictionary<string, string>();
            Headers.Add("HTTP/1.1", "200 OK");
            Headers.Add("Date", "Sun, 10 Oct 2010 23:26:07 GM");
            Headers.Add("Server", "Apache/2.2.8 (Ubuntu) mod_ssl/2.2.8 OpenSSL/0.9.8g");
            Headers.Add("Connection", "close");
        }

        public string CreateHtmlString() 
        {
            string responce_srt = "HTTP/1.1 200 OK\n"
                + "Date: Sun, 10 Oct 2010 23:26:07 GM\n" 
                + "Server: Apache/2.2.8 (Ubuntu) mod_ssl/2.2.8 OpenSSL/0.9.8g\n"
                //+ "Content-Type: application/octet-stream\n"
                //+ "Content-Length: " + fileInfo.Length + "\n"
                //+ "Content-Disposition: attachment; filename=\"config.json\"\n"
                + "Connection: close\n"
                + "\n"
                ;

            return responce_srt;
        }


    }
}
