# BlazorTemplater-StaticDemo
Demonstrates creating a static site using BlazorTemplater

### Background

The [BlazorTemplater](https://github.com/conficient/BlazorTemplater) package enables the rendering of Razor Components to HTML. The main objective in writing the package was to enable the use of `.razor` components to generate content for HTML email text.

However it was suggested that BlazorTemplater could also be used to generate a static website from a Blazor website, using the `@page` routing attribute to determine the page name and structure.

This is an experiment to see if this is possible and what it might look like, and possible issues.

**Important**: this is a **demo** and is **unsupported**. I won't be creating a finished product out of it. Feel free to adapt for your own use if you wish.

### Approach

The app will find Razor Components in a given assembly, that also have a `@page` directive, which results in `[RouteAttribute]` attributes being added to the page component classes. We use reflection to find these, convert to a file reference and use [BlazorTemplater](https://github.com/conficient/BlazorTemplater) to render the component to the file.

A pages with a routes of `@page "/counter"` would write to `\counter.html`.


#### SampleWebsite
I've created a standard Blazor Server web app called **SampleWebsite**. This is to be the source of the pages we are going to render.

#### StaticSiteGenerator
Next is a console app called **StaticSiteGenerator**.

This creates an instance of the `Generator` class, passing a root folder in the constructor. The program calls `.RenderAsync()` passing in the Assembly it wants to search for pages.

### Issues

#### Layout

The default layout is normally set in the `App.razor` file, which we never reference or use in the generator, so we need to tell it which layout to use:
```c#
generator.UseLayout(typeof(SampleWebsite.Shared.StaticLayout));
```

The `MainLayout` in the default app isn't a full HTML document - that exists in `_Host.cshtml`, but we can't render that.

Instead I created `StaticLayout.razor` which combines the content of `_Host.cshtml` and `MainLayout.razor`.

#### CSS and Static Files

The standard `_Host.cshtml` references a lot of static files found in `wwwroot`. These are not copied into the target folder so these all fail.

To improve formatting I replaced the Bootstrap CSS reference with an external file in `StaticLayout.razor`:
```html
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css" ...  >
```

#### CSS Isolation / generated CSS

The layout also has two other CSS references:
```html
 <link href="css/site.css" rel="stylesheet" />
 <link href="SampleWebsite.styles.css" rel="stylesheet" />
 ```
 I haven't tried to fix these. CSS isolation (generated CSS tags) isn't supported.

 #### Services

 The `FetchData.razor` page injects a `WeatherForecastService`, so the app has to provide this before trying to render:
 ```c#
generator.AddService(new SampleWebsite.Data.WeatherForecastService());
```

The `NavLink` components in the `NavBar` inject a `NavigationManger` service. This is an abstract class so I created a `DummyNavigationManager` to inject:
```c#
// required for NavBar
generator.AddService<Microsoft.AspNetCore.Components.NavigationManager>(new DummyNavigationManager());
```

#### Links

The links created by `NavLink` are invalid since they don't have the `.html` suffix, but this could probably be fixed in some way. This is left as an exercise for the reader.

## Finally

It's an experiment - use at your own risk!