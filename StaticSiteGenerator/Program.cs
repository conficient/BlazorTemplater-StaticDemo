using System;
using System.Threading.Tasks;

namespace StaticSiteGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string targetFolder = @"C:\Temp\SampleWebsite";

            Console.WriteLine("Creating Static Site");
            Console.WriteLine($"Target folder: {targetFolder}");

            // create a generator
            var generator = new Generator(targetFolder);

            // register services required
            generator.AddService(new SampleWebsite.Data.WeatherForecastService());
            // required for NavBar
            generator.AddService<Microsoft.AspNetCore.Components.NavigationManager>(new DummyNavigationManager());

            // set the Layout to use "StaticLayout" which includes the full HTML markup
            generator.UseLayout(typeof(SampleWebsite.Shared.StaticLayout));

            var assembly = typeof(SampleWebsite.App).Assembly;

            await generator.RenderAsync(assembly);
        }

    }
}
