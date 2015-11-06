using Cascade.Bootstrap.Helpers;
using Orchard.Logging;
using System;
using System.IO;
using System.Linq;

namespace Cascade.Bootstrap.Services
{
    public interface ICascadeBootstrapService : Orchard.IDependency
    {
        /// <summary>
        /// Copies a complete set of files for a bootswatch
        /// </summary>
        /// <param name="bootstrapThemeFolder">The windows folder in which the theme is located</param>
        /// <param name="fromSwatch">Name of swatch to copy</param>
        /// <param name="toSwatch">Name of new swatch</param>
        /// <returns>null on success. Otherwise, an error message</returns>
        string Copy(string bootstrapThemeFolder, string fromSwatch, string toSwatch);

        /// <summary>
        /// Updates the @import statements in site-{swatch}.less
        /// </summary>
        /// <param name="bootstrapThemeFolder">The windows folder in which the theme is located</param>
        /// <param name="fromSwatch">Name of original swatch</param>
        /// <param name="toSwatch">Name of new swatch</param>
        /// <returns>true on success</returns>
        bool Replace(string bootstrapThemeFolder, string fromSwatch, string toSwatch);

        /// <summary>
        /// Creates a new theme based on Cascade.Bootstrap
        /// </summary>
        /// <param name="themesFolder">Windows folder for Themes</param>
        /// <param name="fromTheme">Name of the theme to copy (can be null) </param>
        /// <param name="toTheme">Name of the theme to be created</param>
        /// <returns>null on success. Otherwise, an error message</returns>
        string CreateTheme(string themesFolder, string fromTheme, string toTheme);

        /// <summary>
        /// Retrieve an attribute value from a css file
        /// </summary>
        /// <param name="bootstrapThemeFolder">Windows folder for themes</param>
        /// <param name="swatch">Name of bootswatch</param>
        /// <param name="styleName">Style name (eg: .navbar-nav)</param>
        /// <param name="attribute">Attribute whose value is to be returned (eg: background-color)</param>
        /// <returns>The value of the attribute, or null if not found</returns>
        string GetCssValue(string bootstrapThemeFolder, string swatch, string styleName, string attribute);
    }

    public class CascadeBootstrapService : ICascadeBootstrapService
    {

        public ILogger Logger { get; set; }
        readonly string[] fileTypes = { "variables", "bootswatch", "site" };
        readonly string[] fileExtensions = { "less", "css", "min.css" };
        const string imageFolder = "Content\\swatches";
        const string imageSuffix = "_th.png";
        const string stylesFolder = "Styles";
        const string cascade = "Cascade.Bootstrap";

        public CascadeBootstrapService()
        {
            Logger = NullLogger.Instance;
        }

        public string Copy(string bootstrapThemeFolder, string fromSwatch, string toSwatch)
        {
            Logger.Error("Starting Copy()");
            try
            {
                foreach (var fileType in fileTypes)
                {
                    foreach (var fileExtension in fileExtensions)
                    {
                        var from = BuildPath(bootstrapThemeFolder, fromSwatch, fileType, fileExtension);
                        var to = BuildPath(bootstrapThemeFolder, toSwatch, fileType, fileExtension);

                        //Logger.Error("About to copy from '" + from + "' to '" + to + "'");

                        // default-variables and default-bootstrap don't exist
                        if (File.Exists(from) && !File.Exists(to))
                            File.Copy(from, to);

                        //Logger.Error("success!");
                    }
                }

                // also copy the image used to show the swatch in the list of swatches
                var fromImg = Path.Combine(bootstrapThemeFolder, imageFolder, fromSwatch + imageSuffix);
                var toImg = Path.Combine(bootstrapThemeFolder, imageFolder, toSwatch + imageSuffix);

                //Logger.Error("About to copy image from '" + fromImg + "' to '" + toImg + "'");
                
                if (File.Exists(fromImg) && !File.Exists(toImg))
                    File.Copy(fromImg, toImg);

                //Logger.Error("success!");

            }
            catch (Exception ex)
            {
                string message = String.Format("Unable to copy from swatch '{0}' to swatch '{1}'", fromSwatch, toSwatch);
                Logger.Error(ex, message);
                return ex.Message ?? message;
            }
            return null; // success
        }

        public bool Replace(string bootstrapThemeFolder, string fromSwatch, string toSwatch)
        {
            var fromPath = BuildPath(bootstrapThemeFolder, fromSwatch, "site", "less");
            var toPath = BuildPath(bootstrapThemeFolder, toSwatch, "site", "less");

            try
            {
                var lines = File.ReadAllLines(fromPath);
                for (var i = 0; i < lines.Count(); i++)
                {
                    var line = lines[i];
                    if (line.Trim().ToLower().StartsWith("@import"))
                    {
                        lines[i] = line.ToLower().Replace("/" + fromSwatch + "-", "/" + toSwatch + "-");
                    }
                }

                if (File.Exists(toPath))
                    File.Delete(toPath);
                File.WriteAllLines(toPath, lines);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to update @import refs for '{0}'", toSwatch);
                return false;
            }
            return true;

        }


        public string CreateTheme(string themesFolder, string fromTheme, string toTheme)
        {
            // if fromTheme is specified and it exists, use it as a template (ie copy and rename)
            // Otherwise, create a new theme by copying mandatory files such as web.config
            // from Cascade.Bootstrap.

            string fromFolder = Path.Combine(themesFolder, fromTheme);

            var themetxt = "Name: " + toTheme + Environment.NewLine +
                            "Author: Cascade Pixels" + Environment.NewLine +
                            "Website: http://cascadepixels.com.au" + Environment.NewLine +
                            "Description: Extends Cascade.Bootstrap theme" + Environment.NewLine +
                            "Version: 1.0" + Environment.NewLine +
                            "BaseTheme: Cascade.Bootstrap" + Environment.NewLine;

            try
            {
                string toFolder = Path.Combine(themesFolder, toTheme);
                Directory.CreateDirectory(toFolder);

                if (!String.IsNullOrWhiteSpace(fromTheme) && Directory.Exists(fromFolder))
                {
                    // -- copy existing theme --

                    // create directories
                    foreach (string dirPath in Directory.GetDirectories(fromFolder, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(fromFolder, toFolder));

                    // copy files
                    foreach (string newPath in Directory.GetFiles(fromFolder, "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(fromFolder, toFolder), true);

                    // update or create theme.txt
                    var themeTxtPath = Path.Combine(themesFolder, "theme.txt");
                    if (File.Exists(themeTxtPath))
                    {
                        themetxt = File.ReadAllText(themeTxtPath);
                        themetxt.Replace(fromTheme, toTheme);
                    }
                    File.WriteAllText(themeTxtPath, themetxt);
                }
                else
                {
                    // -- create a brand new theme --

                    // create directories
                    var contentFolder = Path.Combine(toFolder, "Content");
                    var scriptsFolder = Path.Combine(toFolder, "Scripts");
                    var stylesFolder = Path.Combine(toFolder, "Styles");
                    var viewsFolder = Path.Combine(toFolder, "Views");
                    Directory.CreateDirectory(contentFolder);
                    Directory.CreateDirectory(scriptsFolder);
                    Directory.CreateDirectory(stylesFolder);
                    Directory.CreateDirectory(viewsFolder);

                    // copy files from base theme
                    File.Copy(Path.Combine(themesFolder, cascade, "web.config"), Path.Combine(toFolder, "web.config"));
                    File.Copy(Path.Combine(themesFolder, cascade, "theme.png"), Path.Combine(toFolder, "theme.png"));
                    File.WriteAllText(Path.Combine(toFolder, "theme.txt"), themetxt);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null; //success
        }


        public string GetCssValue(string bootstrapThemeFolder, string swatch, string styleName, string attribute)
        {
            var path = BuildPath(Path.Combine(bootstrapThemeFolder, cascade), swatch, "site", "css");
            return FindProperty(bootstrapThemeFolder, swatch, styleName, attribute, path);
        }
        // HELPERS /////////////////////////////////

        private string BuildPath(string bootstrapThemeFolder, string swatch, string fileType, string fileExtension)
        {
            var bootswatchFolder = Path.Combine(stylesFolder, "bootswatch");
            var filename = String.Empty;
            var path = String.Empty;

            if (fileType == "site")
            {
                filename = fileType + "-" + swatch + "." + fileExtension;
                path = Path.Combine(bootstrapThemeFolder, stylesFolder, filename);
            }
            else
            {
                filename = swatch + "-" + fileType + "." + fileExtension;
                path = Path.Combine(bootstrapThemeFolder, bootswatchFolder, filename);
            }
            return path;
        }

        private static CssParser _parser = null;
        private static string _swatch = null;
        private CssParser GetParser(string bootstrapThemeFolder, string swatch)
        {
            // cache CssParser
            if (_swatch != swatch)
                _parser = null;

            if (_parser == null)
            {
                _parser = new CssParser();
                _parser.AddStyleSheet(BuildPath(Path.Combine(bootstrapThemeFolder, cascade), swatch, "site", "css"));
            }
            return _parser;
        }


        private string FindProperty(string bootstrapThemeFolder, string swatch, string styleName, string attribute, string cssFileName)
        {
            try
            {
                var sc = GetParser(bootstrapThemeFolder, swatch).Styles[styleName];
                return sc.Attributes[attribute];
            }
            catch
            {
                return null;
            }
        }
    }

}