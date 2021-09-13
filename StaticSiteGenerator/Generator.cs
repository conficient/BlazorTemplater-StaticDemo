using BlazorTemplater;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StaticSiteGenerator
{
    /// <summary>
    /// Generates HTML from a Razor Component library into files
    /// </summary>
    public class Generator
    {
        private readonly string _target;
        private readonly Templater _templater;

        /// <summary>
        /// ctor: 
        /// </summary>
        /// <param name="targetFolder">root folder where the files will be created</param>
        public Generator(string targetFolder)
        {
            if (string.IsNullOrWhiteSpace(targetFolder))
                throw new ArgumentNullException(nameof(targetFolder));

            // create the target folder if not found
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            _target = targetFolder;
            _templater = new Templater();
        }

        /// <summary>
        /// tell the generator to use a specific layout
        /// </summary>
        /// <param name="type"></param>
        public void UseLayout(Type type) => _templater.UseLayout(type);

        public void AddService<TService>(TService implementation)
        {
            _templater.AddService<TService>(implementation);
        }
        
        public void AddService<TInterface, TService>(TService implementation) where TService : TInterface
        {
            _templater.AddService<TInterface>(implementation);
        }

        /// <summary>
        /// Render the pages in an assembly to files in the target folder
        /// </summary>
        /// <param name="assembly"></param>
        public async Task RenderAsync(Assembly assembly)
        {
            // get the types in the assembly that implement IComponent
            // and have a RouteAttribute
            Console.WriteLine("Getting all types in assembly");
            var components = GetComponentsWithRoute(assembly);

            int pagesCreated = 0;

            foreach (var component in components)
            {
                Console.WriteLine($"  [{component.Type}]");
                var html = _templater.RenderComponent(component.Type);

                foreach (var route in component.Routes)
                {
                    Console.Write($"   -- {route.Template,-20}  ");

                    var filename = GetFilename(route);
                    if (filename != null)
                    {
                        // mappable route
                        var fullPath = Path.Combine(_target, filename);

                        // routes with / in map to folders: e.g. /folder/PageInSubfolder -> .\Folder\PageInSubFolder.html
                        EnsureFolderExists(fullPath);
                        // write the HTML to the file
                        await File.WriteAllTextAsync(fullPath, html);
                        Console.WriteLine($"-->  {filename}");
                        pagesCreated++;
                    }
                    else
                        Console.WriteLine("skipped");
                }
            }

            Console.WriteLine($"Created {pagesCreated} pages");
        }

        private void EnsureFolderExists(string path)
        {
            // ensure the target folder exists
            var fi = new FileInfo(path);
            var folder = fi.Directory;
            if (!folder.Exists) folder.Create();
        }

        /// <summary>
        /// Get a filename for the route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private string GetFilename(RouteAttribute route)
        {
            // map "/" to "index"
            if (route.Template == "/") return "index.html";

            // check for parameters
            if (IsParameterized(route.Template))
                return null;

            // remove the / prefix, swap remaining / to \ and append .html
            var file = route.Template.TrimStart('/').Replace("/", "\\") + ".html";
            return file;
        }

        /// <summary>
        /// Very simple check for parameterized template: e.g. /customer/{id}
        /// </summary>
        /// <param name="template">route template</param>
        /// <returns></returns>
        private static bool IsParameterized(string template)
        {
            if (string.IsNullOrEmpty(template)) return false;
            return template.Contains('{');
        }

        private static List<TypeAndRoutes> GetComponentsWithRoute(Assembly assembly)
        {
            var icomponent = typeof(IComponent);
            // get all classes that implement IComponent
            return (from t in assembly.GetTypes()
                    where icomponent.IsAssignableFrom(t)
                    && !t.IsAbstract
                    && !t.IsGenericType
                    let routes = GetRouteAttributes(t)
                    where routes.Any()
                    select new TypeAndRoutes(t, routes)).ToList();
        }

        /// <summary>
        /// Get all the RouteAttributes in the component
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <remarks>There can be multiple routes in a page</remarks>
        private static List<RouteAttribute> GetRouteAttributes(Type t)
        {
            return t.GetCustomAttributes<RouteAttribute>(true).ToList();
        }
    }
}
