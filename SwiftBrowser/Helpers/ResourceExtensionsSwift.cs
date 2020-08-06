using Windows.ApplicationModel.Resources;

namespace SwiftBrowser.Helpers
{
    internal static class ResourceExtensionsSwift
    {
        private static readonly ResourceLoader _resLoader = new ResourceLoader();

        public static string GetLocalizedSwift(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }
    }
}
