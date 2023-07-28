using Http.HttpRequest;
using Http.HttpResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Model
{
    public class MyHttpContext
    {
        public ContentResult contentResult { get; set; }

        public MyHttpRequest httpRequest { get; set; }

        public MyHttpResponce httpResponce { get; set; }
    }
}
