using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace StaticSiteGenerator
{
    /// <summary>
    /// Store a Type and its routes
    /// </summary>
    internal class TypeAndRoutes
    {

        public TypeAndRoutes(Type type, List<RouteAttribute> routes)
        {
            Type = type;
            _routes = routes;
        }

        public Type Type { get; }

        public IReadOnlyList<RouteAttribute> Routes => _routes;
        
        private readonly List<RouteAttribute> _routes;

    }
}