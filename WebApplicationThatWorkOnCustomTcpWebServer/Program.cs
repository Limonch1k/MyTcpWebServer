using CustomWebServer;
using DI;
using EndPoint;
using WebExternalResourceApplication.Controllers;
using WebExternalResourceApplication.Filters;

class Program
{
    public static void Main(string[] args)
    {
        var endPoint = new EndPointHandler();
        var customDI = new CustomDI();
        customDI.Add<HomeController>();
        customDI.Add<LoginController>();
        
        endPoint.Add<HomeController>("/load_fact_data");
        endPoint.Add<HomeController>("/load_fuck_data");
        endPoint.Add<HomeController>("/load_selected_file");
        endPoint.Add<LoginController>("/LogIn");
        


        var tcpWebServer = new TcpWebServer(endPoint, customDI);

        tcpWebServer.StartWebServer();
    }
}
