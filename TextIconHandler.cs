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
    [COMServerAssociation(AssociationType.FileExtension, ".txt")]
    class TextIconHandler : SharpIconHandler
    {
        private readonly Lazy<Font> thumbnailFont;
        private readonly Lazy<Brush> thumbnailTextBrush;

        public TextIconHandler()
        {
            thumbnailFont = new Lazy<Font>(() => new Font("Consolas", 4f));
            thumbnailTextBrush = new Lazy<Brush>(() => new SolidBrush(Color.Black));
        }

        protected Icon RenderIcon(bool smallIcon, uint iconSize)
        {
            if (smallIcon)
                return null;

            //  Attempt to open the stream with a reader.
            try
            {
                using (var stream = new FileStream(this.SelectedItemPath, FileMode.Open))
                {
                    return IconRenderer.CreateAsciiFileIcon("TXT", stream, iconSize);
                    //return Icon.FromHandle(bitmap.GetHicon());
                }
            }
            catch (Exception exception)
            {
                //  Log the exception and return null for failure.
                LogError("An exception occured opening the text file.", exception);
                return null;
            }
        }

        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            Icon icon = RenderIcon(smallIcon, iconSize);
            
            if (icon == null)
                icon = Properties.Resources.ascii;

            return GetIconSpecificSize(icon, new Size((int)iconSize, (int)iconSize));
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
