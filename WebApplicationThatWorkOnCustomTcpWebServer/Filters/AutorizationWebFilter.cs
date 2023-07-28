using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using CustomAutorizationFilters;
using Http.HttpRequest;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace WebExternalResourceApplication.Filters
{
    public class AutorizationWebFilter : Attribute, IBaseAutorizationFilter
    {
        private MyHttpContext _context;

        public AutorizationWebFilter() 
        {

        }

        public void OnAutorization(MyHttpContext context)
        {
            _context = context;
            var user = context.httpRequest.Cookies.TryParseUserCookie("user_name");
            var password = context.httpRequest.Cookies.TryParseUserCookie("pass");
            if (CheckPassword(user, password)) 
            {
                //return new ContentResult() { Content = "You succesfull pass", StatusCode = 200, ContentType="text/html"};
                return;
            }
            //return new ContentResult() { Content = "Fuck you", StatusCode = 400, ContentType = "text/html" };
            context.contentResult = new ContentResult() { Content = "Fuck you", StatusCode = 400, ContentType = "text/html" }; 
        }


        private bool CheckPassword(string user, string password)
        {
            string line;
            StreamReader file = new("password.txt");
            while ((line = file.ReadLine()) != null)
            {
                var str = line.Split(";");
                List<string> s = new List<string>(); 
                foreach (var st in str) 
                {
                    var c = st.Split(": ");
                    s.Add(c[1]);
                }

                if (s.Contains(user.Replace("\0","")) && s.Contains(password.Replace("\0",""))) 
                {
                    file.Close();
                    return true;
                }
            }
            file.Close();
            return false;
        }
    }
}
