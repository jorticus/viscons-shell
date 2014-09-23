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

namespace Viscons.ShellHandler
{
    /// <summary>
    /// Provides dynamic text icons for ASCII files (text and code)
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".txt")]
    class TextIconHandler : SharpIconHandler
    {
        private readonly Lazy<Font> thumbnailFont;
        private readonly Lazy<Brush> thumbnailTextBrush;

        public TextIconHandler()
        {
            thumbnailFont = new Lazy<Font>(() => new Font("Consolas", 4f));
            thumbnailTextBrush = new Lazy<Brush>(() => new SolidBrush(Color.Black));
        }

        protected Icon DefaultIcon(uint iconSize)
        {
            return GetIconSpecificSize(Properties.Resources.ascii, new Size((int)iconSize, (int)iconSize));
        }

        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            Log(String.Format("RenderIcon(size: {0}, file: {1})", iconSize, Path.GetFileName(this.SelectedItemPath)));

            if (smallIcon)
            {
                Log("Ignoring small icon");
                return DefaultIcon(iconSize);
            }

            var previewLines = new List<string>();

            //  Attempt to open the stream with a reader.
            try
            {
                Log(String.Format("Opening text file {0}", this.SelectedItemPath));
                using (var stream = new FileStream(this.SelectedItemPath, FileMode.Open, FileAccess.Read))
                {
                    // Read up to ten lines of text
                    using (var reader = new StreamReader(stream))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var line = reader.ReadLine();
                            if (line == null)
                                break;
                            previewLines.Add(line);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //  Log the exception and return null for failure.
                LogError("An exception occured opening the text file.", exception);
                return DefaultIcon(iconSize);
            }

            Log(String.Format("Rendering Icon (size: {0})", iconSize));
            Icon icon = IconRenderer.CreateAsciiFileIcon("TXT", previewLines, iconSize);

            Log(String.Format("Icon done (size: {0})", iconSize));
            return icon;

        }

        /// <summary>
        /// Creates the thumbnail for text, using the provided preview lines.
        /// </summary>
        /// <param name="previewLines">The preview lines.</param>
        /// <param name="width">The width.</param>
        /// <returns>
        /// A thumbnail for the text.
        /// </returns>
        
    }
}
