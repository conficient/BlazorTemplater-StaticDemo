using Microsoft.AspNetCore.Components;

namespace StaticSiteGenerator
{
    /// <summary>
    /// Fake NavigationManager service
    /// </summary>
    /// <remarks>
    /// Needed an implementation of NavigationManager for the `NavLink` entries in `NavMenu.razor`
    /// If the NavLinks are removed this isn't needed.
    /// </remarks>
    public class DummyNavigationManager : NavigationManager
    {
        public DummyNavigationManager()
        {
            const string uri = "http://example.com/";
            Initialize(uri, uri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            return;
        }
    }
}
