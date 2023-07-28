
using Microsoft.AspNetCore.Mvc;
using MyController;
using WebExternalResourceApplication.Filters;

namespace WebExternalResourceApplication.Controllers
{
    public class HomeController : MyControllerBase
    {
        [AutorizationWebFilter]
        [Route("/load_fact_data")]
        public ContentResult Index(string user)
        {
            string path = "Jopa.html";
            var file = new StreamReader(path);
            

            var content = file.ReadToEnd();

            file.Close();

            return new ContentResult() { StatusCode = 200, Content = content, ContentType = "text/html" };
        }

        [AutorizationWebFilter]
        [Route("/load_fuck_data")]
        public string load_fuck_data(string file_name)
        {
            string responce_srt = "HTTP/1.1 200 OK\n"
            + "Date: Sun, 10 Oct 2010 23:26:07 GM\n"
            + "Server: Apache/2.2.8 (Ubuntu) mod_ssl/2.2.8 OpenSSL/0.9.8g\n"
            //+ "Content-Type: application/octet-stream\n"
            + "Content-Length: " + 10 + "\n"
            //+ "Content-Disposition: attachment; filename=\"config.json\"\n"
            + "Connection: close\n"
            + "\n"
            + "" + "0123456789"
            ;

            return responce_srt;
        }

        [AutorizationWebFilter]
        [Route("/load_selected_file")]
        public ContentResult load_file(string filename) 
        {
            string path = "risamos.txt";
            var file = new StreamReader(filename);

            var content = file.ReadToEnd();
            httpContext.httpResponce.Headers.Add("Content-Disposition", "attachment; filename=file.txt");
            return new ContentResult() {StatusCode = 200, Content = content, ContentType = "application/octet-stream" };
        }


    }
}
