using DI;
using EndPoint;
using Http.HttpRequest;
using Http.HttpResponce;
using MyController;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
using CustomAutorizationFilters;



namespace CustomWebServer
{
    public class TcpWebServer
    {
        private EndPointHandler _endPointHandler { get; set; }

        private CustomDI _customDI { get; set; }

        public TcpWebServer(EndPointHandler endPointHandler, CustomDI customDI) 
        {
            this._endPointHandler = endPointHandler;
            this._customDI = customDI;
        }

        private Task<MyHttpRequest> RequestHandler(NetworkStream stream)
        {
            return
            Task<string[]>.Run(() =>
            {
                StreamReader reader = new StreamReader(stream);

                char[] buffer = new char[1500];
                int bytesRead = 0;
                StringBuilder builder = new StringBuilder();
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    builder.Append(buffer, 0, bytesRead);
                    if (bytesRead < buffer.Length) 
                    {
                        break;
                    }
                }
                string str_request = builder.ToString();

                Console.WriteLine(str_request);
                string[] http_parts = str_request.Split("\r\n\r\n");

                MyHttpRequest request = new MyHttpRequest();

                request.Body = http_parts[1];
                var header_part = http_parts[0];
                var headers = header_part.Split("\r\n"); 
                var first_inform_row = headers[0].Split(' ');

                request.Method = first_inform_row[0];
                request.Url = first_inform_row[1];

                for (int i = 1; i < headers.Length; i++) 
                {
                    string[] key_value = headers[i].Split(":");
                    request.Headers.Add(key_value[0], string.Join(":",key_value[1..]));
                }

                var cookie_str = request.Headers.Where(x => x.Key.Equals("Cookie")).Select(x => x.Value).FirstOrDefault();
                if (cookie_str is not null) 
                {
                    var cookie_mass = cookie_str.Split(";");

                    foreach (var cookie in cookie_mass)
                    {
                        var name_body = cookie.Split("=");
                        request.Cookies.Cookie.Add(name_body[0], name_body[1]);
                    }
                }


                string[] queryParams = request.Url.Split('?');
                request.Path = queryParams[0];
                if (queryParams.Length > 1)
                {
                    string[] queryParamPairs = queryParams[1].Split('&');
                    foreach (string queryParamPair in queryParamPairs)
                    {
                        string[] queryParamPairParts = queryParamPair.Split('=');
                        if (queryParamPairParts.Length == 2)
                        {
                            request.QueryParams.Add(queryParamPairParts[0], queryParamPairParts[1]);
                        }
                    }
                }

                return request;                                           
            });
        }

        private Task ResponceHandler(TcpClient tcpClient,MyHttpRequest request)
        {

            return 
            Task.Run(() => 
            {
                try
                {
                    object? obj;

                    List<object> object_mass = ReturnParamList(request);

                    var controller = ReturnRouteController(request, tcpClient);

                    CheckFilters(tcpClient, request, controller);

                    _endPointHandler.Add(request.Path, controller);

                    obj = this._endPointHandler.InvokeMethod(request.Path , request.QueryParams);

                    CustomObjectHandler(obj, controller.httpContext.httpResponce);

                    string responce = CreateSTRResponce(controller.httpContext.httpResponce);

                    byte[] bytes = Encoding.UTF8.GetBytes(responce);

                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);


                    _endPointHandler.Delete(request.Path);
                                       
                }
                catch (Exception e)
                {
                    tcpClient.GetStream().Write(Encoding.UTF8.GetBytes("FUCK YOU"), 0, 8);
                }
                finally 
                {
                }
            });
        }

        private List<object> ReturnParamList(MyHttpRequest request) 
        {
            List<object> object_mass = new List<object>();

            foreach (var str in request.QueryParams.Values)
            {
                object_mass.Add(str);
            }

            return object_mass;
        }

        private MyControllerBase ReturnRouteController(MyHttpRequest request , TcpClient client) 
        {
            var type = _endPointHandler.ReturnControllerType(request.Path);

            var controller = _customDI.GetImplementation(type) as MyControllerBase;

            controller.httpContext = new MyHttpContext();

            controller.httpContext.httpRequest = request;

            controller.httpContext.httpResponce = new MyHttpResponce();
            controller._endpointHandler = _endPointHandler;
            controller._customDI = _customDI;
            controller.tcpClient = client;

            return controller;
        }


        private void CheckFilters(TcpClient tcpClient, MyHttpRequest request, MyControllerBase controller) 
        {
            var boolean = _endPointHandler.HasMethodAttribute<IBaseAutorizationFilter>(request.Path, controller);

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
                            controller.httpContext.httpResponce.Headers.Add("HTTP/1.1"," " + controller.httpContext.contentResult.StatusCode + " OK");
                        }
                        
                        string str = CreateSTRResponce(controller.httpContext.httpResponce);
                        tcpClient.GetStream().Write(Encoding.UTF8.GetBytes(str), 0, str.Length);
                        return;
                    }
                }
            }


        }

        internal static string CreateSTRResponce(MyHttpResponce httpResponce) 
        {
            string responce = "";

            foreach (var str in httpResponce.Headers)
            {
                if (str.Key.Equals("HTTP/1.1")) 
                {
                    responce = str.Key + " " + str.Value + "\r\n";
                    continue;
                }
                responce += str.Key + ": " + str.Value + "\r\n";
            }

            if (httpResponce.Cookies is not null) 
            {
                var str = httpResponce.Cookies.GetHeadersContent();
                responce += str;
            }

            responce += "\n";

            if (httpResponce.Body is not null) 
            {
                responce += httpResponce.Body;
            }

            return responce;
        }

        private void CustomObjectHandler(object obj, MyHttpResponce httpResponce) 
        {

            if (obj is byte[])
            {
                httpResponce.Body += Convert.ToBase64String(obj as byte[]);
            }
            else if (obj is string) 
            {
                httpResponce.Body += obj as string;
            }
            else if (obj is ContentResult)
            {

                ContentResult contentResult = obj as ContentResult;

                if (contentResult.StatusCode is not null)
                {
                    httpResponce.Headers.Remove("HTTP/1.1");
                    httpResponce.Headers.Add("HTTP/1.1", contentResult.StatusCode + " OK");
                }

                if (contentResult.ContentType is not null)
                {
                    httpResponce.Headers.Remove("ContentType");
                    httpResponce.Headers.Add("ContentType", contentResult.ContentType);
                }

                if (contentResult.Content is not null)
                {
                    httpResponce.Body = contentResult.Content;
                }
            }
            else if (obj is Stream)
            {
                var reader = new StreamReader(obj as Stream);
                httpResponce.Body = reader.ReadToEnd();
            }
            else
            {
                string json = JsonConvert.SerializeObject(obj);
                httpResponce.Body += json;
            }



            return;
        }

        public async Task StartWebServer() 
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("192.168.211.64"), 80);



            Action<TcpClient> a = (TcpClient _client) =>
            {
                lock (_client)
                {
                    var net_stream = _client.GetStream();
                    var request = RequestHandler(net_stream).Result;                  
                    ResponceHandler(_client, request).Wait();

                    _client.Close();
                }
            };

            try
            {
                listener.Start();

                while (true)
                {
                    var client = listener.AcceptTcpClient();                   
                    Task.Run(() => a(client));
                }
            }
            finally 
            {
                listener.Stop();
            }

        }
    }
}
