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

namespace shellhandler
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".txt")]
    class TextIconHandler : SharpIconHandler
    {
        private readonly Lazy<Font> thumbnailFont;
        private readonly Lazy<Brush> thumbnailTextBrush;

        public TextIconHandler()
        {
            thumbnailFont = new Lazy<Font>(() => new Font("Courier New", 12f));
            thumbnailTextBrush = new Lazy<Brush>(() => new SolidBrush(Color.Black));
        }

        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            //  Attempt to open the stream with a reader.
            try
            {
                using (var reader = new StreamReader(this.SelectedItemPath))
                {
                    //  Read up to ten lines of text.
                    var previewLines = new List<string>();
                    for (int i = 0; i < 10; i++)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            break;
                        previewLines.Add(line);
                    }

                    //  Now return a preview of the lines.
                    var img = CreateThumbnailForText(previewLines, iconSize);

                    Icon icon = Icon.FromHandle(img.GetHicon());
                    return icon;
                    //return GetIconSpecificSize(icon, new Size((int)iconSize, (int)iconSize));
                }
            }
            catch (Exception exception)
            {
                //  Log the exception and return null for failure.
                LogError("An exception occured opening the text file.", exception);
                return GetIconSpecificSize(Properties.Resources.blank, new Size((int)iconSize, (int)iconSize));
            }
        }
        /*protected override System.Drawing.Bitmap GetThumbnailImage(uint width)
        {

        }*/

        /// <summary>
        /// Creates the thumbnail for text, using the provided preview lines.
        /// </summary>
        /// <param name="previewLines">The preview lines.</param>
        /// <param name="width">The width.</param>
        /// <returns>
        /// A thumbnail for the text.
        /// </returns>
        private Bitmap CreateThumbnailForText(IEnumerable<string> previewLines, uint width)
        {
            //  Create the bitmap dimensions.
            var thumbnailSize = new Size((int)width, (int)width);

            //  Create the bitmap.
            var bitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Height, PixelFormat.Format32bppArgb);

            //  Create a graphics object to render to the bitmap.
            using (var graphics = Graphics.FromImage(bitmap))
            {
                //  Set the rendering up for anti-aliasing.
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                //  Draw the page background.
                graphics.DrawImage(Properties.Resources.blank.ToBitmap(), 0, 0, thumbnailSize.Width, thumbnailSize.Height);

                //  Create offsets for the text.
                var xOffset = width * 0.2f;
                var yOffset = width * 0.3f;
                var yLimit = width - yOffset;

                graphics.Clip = new Region(new RectangleF(xOffset, yOffset, thumbnailSize.Width - (xOffset * 2), thumbnailSize.Height - width * .1f));

                //  Render each line of text.
                foreach (var line in previewLines)
                {
                    graphics.DrawString(line, thumbnailFont.Value, thumbnailTextBrush.Value, xOffset, yOffset);
                    yOffset += 14f;
                    if (yOffset + 14f > yLimit)
                        break;
                }
            }

            //  Return the bitmap.
            return bitmap;
        }
    }
}
