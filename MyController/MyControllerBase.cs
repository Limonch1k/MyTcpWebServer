using CustomAutorizationFilters;
using CustomWebServer;
using DI;
using EndPoint;
using Http.HttpRequest;
using Http.HttpResponce;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyController
{
    public class MyControllerBase
    {
        internal EndPointHandler _endpointHandler { get; set; }

        internal TcpClient tcpClient { get; set; }

        internal CustomDI _customDI { get; set; }

        public MyHttpContext httpContext { get; set; }

        protected object Redirect(string route, Dictionary<string,string> dictionary) 
        {
            try
            {
                Type t = _endpointHandler.ReturnControllerType(route);
                var controller = _customDI.GetImplementation(t) as MyControllerBase;
                if (CheckFilters(httpContext.httpRequest.Cookies, controller, route) ||
                CheckFilters(httpContext.httpResponce.Cookies, controller, route))
                {
                    _endpointHandler.Add(route, controller);
                    var b = _endpointHandler.InvokeMethod(route, dictionary);
                    _endpointHandler.Delete(route);
                    return b;
                }
                else 
                {
                    var str = TcpWebServer.CreateSTRResponce(controller.httpContext.httpResponce);
                    tcpClient.GetStream().Write(Encoding.UTF8.GetBytes(str), 0, str.Length);
                }
                return null;
            }
            catch (Exception e)
            {
            }
            finally 
            {
            }

            return null;
        }

        private bool CheckFilters(MyCookies cookies, MyControllerBase controller, string route) 
        {
            var boolean = _endpointHandler.HasMethodAttribute<IBaseAutorizationFilter>(route, controller);

            List<IBaseAutorizationFilter> filters = null;

            if (boolean)
            {
                filters = _customDI.GetImplementations<IBaseAutorizationFilter>(typeof(IBaseAutorizationFilter));
            }

            if (filters is not null)
            {
                foreach (var filter in filters)
                {
                    filter.OnAutorization(controller.httpContext);
                    if (controller.httpContext.contentResult is null)
                    {
                        continue;
                    }
                    else
                    {
                        if (controller.httpContext.httpResponce.Headers["Content"] is not null)
                        {
                            controller.httpContext.httpResponce.Headers.Remove("Content");
                            controller.httpContext.httpResponce.Headers.Add("Content", controller.httpContext.contentResult.Content);
                        }

                        if (controller.httpContext.httpResponce.Headers["Content-Type"] is not null)
                        {
                            controller.httpContext.httpResponce.Headers.Remove("Content-Type");
                            controller.httpContext.httpResponce.Headers.Add("Content-Type", controller.httpContext.contentResult.ContentType);
                        }

                        if (controller.httpContext.httpResponce.Headers["HTTP/1.1"] is not null)
                        {
                            controller.httpContext.httpResponce.Headers.Remove("HTTP/1.1");
                            controller.httpContext.httpResponce.Headers.Add("HTTP/1.1", " " + controller.httpContext.contentResult.StatusCode + " OK");
                        }

                        
                        return false;
                    }
                }
            }

            return true;

        }

    }
}
