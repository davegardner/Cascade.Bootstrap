using Cascade.Bootstrap.Services;
using System;
using System.Web.Mvc;

namespace Cascade.Bootstrap.Controllers
{
    public class BootstrapSettingsController : Controller
    {
        private readonly ICascadeBootstrapService _cascadeBootstrapService;

        public BootstrapSettingsController(ICascadeBootstrapService cascadeBootstrapService)
        {
            _cascadeBootstrapService = cascadeBootstrapService;
        }

        [HttpPost]
        public string DuplicateSwatch(string fromSwatch, string toSwatch)
        {
            // normalize swatch names
            fromSwatch = fromSwatch.Trim().ToLower();
            toSwatch = toSwatch.Trim().ToLower();

            // duplicate the swatch
            var bootstrapThemeFolder = Server.MapPath("~/Themes/Cascade.Bootstrap");
            var message = _cascadeBootstrapService.Copy(bootstrapThemeFolder, fromSwatch, toSwatch);
            if (!String.IsNullOrEmpty(message))
                return message;

            // update the site-swatch.less file
            if (!_cascadeBootstrapService.Replace(bootstrapThemeFolder, fromSwatch, toSwatch))
                return "Unable to update swatch @import statements";

            // Success
            return null;

        }

        [HttpPost]
        public string DuplicateTheme(string fromTheme, string toTheme)
        {
            // normalize
            fromTheme = fromTheme.Trim().ToLower();
            toTheme = toTheme.Trim().ToLower();
            var bootstrapThemeFolder = Server.MapPath("~/Themes");

            return _cascadeBootstrapService.CreateTheme(bootstrapThemeFolder, fromTheme, toTheme);
        }

        [HttpGet]
        public string GetCssValue(string Swatch, string Style, string Attribute)
        {
            return _cascadeBootstrapService.GetCssValue(Server.MapPath("~/Themes"), Swatch.Trim().ToLower(), Style.Trim(), Attribute.Trim());
        }

    }
}