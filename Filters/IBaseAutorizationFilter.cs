using Http.HttpRequest;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAutorizationFilters
{
    public interface IBaseAutorizationFilter
    {
        public void OnAutorization(MyHttpContext context);
    }
}
