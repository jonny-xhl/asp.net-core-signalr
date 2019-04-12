using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.SignalRdemo.Constraint
{
    public class EmailCustomConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }
            if (string.IsNullOrWhiteSpace(routeKey))
            {
                throw new ArgumentNullException(nameof(routeKey));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            object o;

            return false;
        }
    }
}
