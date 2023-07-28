using MyController;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace WebExternalResourceApplication.Controllers
{
    public class LoginController : MyControllerBase
    {
        private string responce_str = "HTTP/1.1 200 OK\n"
                + "Date: Sun, 10 Oct 2010 23:26:07 GM\n"
                + "Server: Apache/2.2.8 (Ubuntu) mod_ssl/2.2.8 OpenSSL/0.9.8g\n"
                //+ "Content-Type: application/octet-stream\n"
                + "Content-Length: " + 10 + "\n"
                //+ "Content-Disposition: attachment; filename=\"config.json\"\n"
                + "Connection: close\n"
                + "Set-Cookie: " + /*httpContext.httpResponce.Cookies*/  "\n"
                + "\n"
                + "" + "0123456789"
                ;

        [Route("/LogIn")]
        public ContentResult LogIn(string user, string password) 
        {
            if (CheckPassword(user, password))
            {
                SetCookie(user, password);            
            }
            else 
            {
                return new ContentResult() { StatusCode = 400, Content = "fUCK YOU", ContentType = "text/html" };
            }

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("user", user);

            var g = Redirect("/load_fact_data", dictionary);

            return g as ContentResult;
        }

        private bool CheckPassword(string user, string password) 
        {
            var list = new List<string>();
            list.AddRange(File.ReadAllLines("password.txt"));

            foreach (var l in list) 
            {
                var mass = l.Split(";");
                if (mass.Length > 0) 
                {
                    if (user.Equals(mass[0][6..]) && password.Equals(mass[1][10..]))
                    {
                        return true;
                    }                  
                }
            }
            return false;
        }


        private void SetCookie(string user, string password)
        {
            httpContext.httpResponce.Cookies.SetCookie("user_name",user);
            httpContext.httpResponce.Cookies.SetCookie("pass", password);
        }
    }
}
