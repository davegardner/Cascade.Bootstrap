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
        /// <returns></returns>
        string Copy(string bootstrapThemeFolder, string fromSwatch, string toSwatch);

        /// <summary>
        /// Updates the @import statements in site-{swatch}.less
        /// </summary>
        /// <param name="bootstrapThemeFolder">The windows folder in which the theme is located</param>
        /// <param name="fromSwatch">Name of original swatch</param>
        /// <param name="toSwatch">Name of new swatch</param>
        /// <returns></returns>
        bool Replace(string bootstrapThemeFolder, string fromSwatch, string toSwatch);
    }

    public class CascadeBootstrapService : ICascadeBootstrapService
    {

        public ILogger Logger { get; set; }
        readonly string[] fileTypes = { "variables", "bootswatch", "site" };
        readonly string[] fileExtensions = { "less", "css", "min.css" };
        const string imageFolder = "Content\\swatches";
        const string imageSuffix = "_th.png";
        const string stylesFolder = "Styles";

        public CascadeBootstrapService()
        {
            Logger = NullLogger.Instance;
        }

        public string Copy(string bootstrapThemeFolder, string fromSwatch, string toSwatch)
        {
            try
            {
                foreach (var fileType in fileTypes)
                {
                    foreach (var fileExtension in fileExtensions)
                    {
                        var from = BuildPath(bootstrapThemeFolder, fromSwatch, fileType, fileExtension);
                        var to = BuildPath(bootstrapThemeFolder, toSwatch, fileType, fileExtension);
                        
                        // default-variables and default-bootstrap don't exist
                        if (File.Exists(from) && !File.Exists(to)) 
                            File.Copy(from, to);
                    }
                }

                // also copy the image used to show the swatch in the list of swatches
                var fromImg = Path.Combine(bootstrapThemeFolder, imageFolder, fromSwatch + imageSuffix);
                var toImg = Path.Combine(bootstrapThemeFolder, imageFolder, toSwatch + imageSuffix);
                if(File.Exists(fromImg) && !File.Exists(toImg))
                    File.Copy(fromImg, toImg);

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
            catch(Exception ex)
            {
                Logger.Error(ex, "Unable to update @import refs for '{0}'", toSwatch);
                return false;
            }
            return true;

        }

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
    }
}