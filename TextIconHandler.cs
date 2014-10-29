using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpShell.SharpIconHandler;
using SharpShell.Attributes;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.IO;
using Viscons.IconRenderer;

namespace Viscons.ShellHandler
{
    /// <summary>
    /// Provides dynamic text icons for ASCII files (text and code)
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".txt", ".md", ".c", ".cpp", ".h")]
    class TextIconHandler : SharpIconHandler
    {
        public TextIconHandler()
        {

        }

        /// <summary>
        /// Read the first 10 lines in the given file
        /// Does not check for binary/ascii formats, so use with care.
        /// </summary>
        /// <param name="filename">The file to read from</param>
        /// <returns>Up to 10 lines of text content</returns>
        protected List<string> ReadFileContents(string filename)
        {
            var previewLines = new List<string>();

            
            using (var stream = new FileStream(this.SelectedItemPath, FileMode.Open, FileAccess.Read))
            {
                // Read up to 10 lines of text
                using (var reader = new StreamReader(stream))
                {
                    for (int i = 0; i < 20; i++)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            break;
                        previewLines.Add(line);
                    }
                }
            }
            return previewLines;
        }

        /// <summary>
        /// Renders a dynamic icon that contains the text of the current file
        /// </summary>
        protected Icon RenderIcon(bool smallIcon, uint iconSize)
        {
            string name = Path.GetFileName(this.SelectedItemPath);
            string label = Path.GetExtension(this.SelectedItemPath).Substring(1).ToUpperInvariant();

            /*if (iconSize < 32 || smallIcon)
                return DefaultIcon(iconSize);*/

            List<string> previewLines = null;

            //  Attempt to open the stream with a reader.
            try
            {
                Log(String.Format("{0}: Opening text file {1}", name, this.SelectedItemPath));
                previewLines = ReadFileContents(this.SelectedItemPath);
            }
            catch (Exception exception)
            {
                LogError(String.Format("{0}: An exception occured opening the text file.", name), exception);
                //return null;
            }


            Log(String.Format("{0}: Rendering Icon", name));

            IconConfig? config = IconThemeConfigs.GetConfig(iconSize, IconThemeConfigs.WhiteThemeConfigs);
            if (config.HasValue)
            {
                TextIconRenderer renderer = new TextIconRenderer(config.Value);
                Bitmap bitmap = renderer.Render(label, previewLines);
                return IconUtils.BitmapToIcon(bitmap);
            }
  
            return null;
        }

        /// <summary>
        /// Retrieve the correct icon for the current file (SharpShell)
        /// </summary>
        /// <returns>An icon containing a single format of the correct size</returns>
        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            Icon icon = RenderIcon(smallIcon, iconSize);

            if (icon == null)
            {
                return IconUtils.GetIconSpecificSize(Properties.Resources.ascii, new Size((int)iconSize, (int)iconSize));
            }

            return icon;

        }
        
    }
}
