using System;
using XDemo.Core.ApiDefinitions;
namespace XDemo.Core.Shared
{
    public static class UrlHelper
    {
        /// <summary>
        /// Combine the specified relativeResource with mainhost config in <see cref="ApiHosts.MainHost"/>.
        /// </summary>
        /// <returns>The combine.</returns>
        /// <param name="relativeResource">Relative resource.</param>
        public static string Combine(string relativeResource)
        {
            if (relativeResource == null)
                throw new ArgumentNullException(nameof(relativeResource), $"Your {nameof(relativeResource)} can not be null!");
            return $"{TrimSlash(ApiHosts.MainHost)}/{TrimSlash(relativeResource)}";
        }

        static string TrimSlash(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url), $"Your {nameof(url)} can not be null!");
            url = url.Trim();
            if (url.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                url = url.Substring(1);
            if (url.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                url = url.Substring(0, url.Length - 1);
            return url;
        }
    }
}
