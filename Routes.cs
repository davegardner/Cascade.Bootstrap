using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cascade.Bootstrap
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {   
                    Priority = 5,
                    Route = new Route("Admin/Settings/CascadeBootstrapTheme/DuplicateSwatch",
                        new RouteValueDictionary {
                            {"area", "Cascade.Bootstrap"},
                            {"controller", "BootstrapSettings"},
                            {"action", "DuplicateSwatch"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Bootstrap"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {   
                    Priority = 5,
                    Route = new Route("Admin/Settings/CascadeBootstrapTheme/DuplicateTheme",
                        new RouteValueDictionary {
                            {"area", "Cascade.Bootstrap"},
                            {"controller", "BootstrapSettings"},
                            {"action", "DuplicateTheme"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Bootstrap"}
                        },
                        new MvcRouteHandler())
                }

            };
        }
    }
}