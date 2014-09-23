using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpShell.SharpIconHandler;
using System.Drawing.IconLib;
using System.Collections;

namespace Viscons.ShellHandler
{


    public static class IconRenderer
    {
        private const string fontName = "Consolas";

        private struct IconEntry
        {
            public string name;
            public uint size;
            public Rectangle contentBounds;
            public Font textFont;
        }

        /// <summary>
        /// This defines a preset list of icon templates used for creating dynamic icons.
        /// The contentBounds defines a region where text can be written to,
        /// any text that lies outside these bounds is clipped.
        /// </summary>
        static readonly List<IconEntry> IconEntries = new List<IconEntry> {
            new IconEntry { name = "white32px", size = 32, contentBounds = new Rectangle(7, 10, 19, 19), textFont = new Font(fontName, 4.0f) },
            new IconEntry { name = "white48px", size = 48, contentBounds = new Rectangle(9, 14, 31, 31), textFont = new Font(fontName, 32.0f) },
            new IconEntry { name = "white256px", size = 256, contentBounds = new Rectangle(46, 77, 164, 157), textFont = new Font(fontName, 32.0f) },
        };

        private static IconEntry GetIconEntry(uint size)
        {
            try
            {
                return IconRenderer.IconEntries.Single((item) => item.size == size);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception(String.Format("Could not find icon entry for size {0}", size), innerException: e);
            }
        }


        /// <summary>
        /// Load an IconImage from the specified resource name,
        /// matching the given size (eg. 32x32)
        /// </summary>
        /*private static IconImage LoadIconResource(string name, Size size)
        {
            // Load the icon resource
            // Note: Icons must be stored as 'System.Byte[], mscorlib'
            MultiIcon iconFile = new MultiIcon();
            var resource = (byte[])Properties.Resources.ResourceManager.GetObject(name);
            using (var stream = new MemoryStream(resource))
            {
                iconFile.Load(stream);
            }

            // MultiIcon can contain multiple icons (eg. ICL/DLL files), 
            // but in this case it will only ever contain one icon.
            var icon = iconFile.First();

            // Find the correct size
            foreach (var image in icon)
            {
                if (image.Size == size)
                    return image;
            }

            throw new Exception(String.Format(
                "Could not find matching size ({0}) for icon '{1}'",
                size, name));
        }*/

        private static Bitmap LoadIconResource(string name, Size size)
        {
            Icon icon = (Icon)Properties.Resources.ResourceManager.GetObject(name);
            return new Icon(icon, size).ToBitmap();
        }

        private static Bitmap LoadBitmapResource(string name)
        {
            return (Bitmap)Properties.Resources.ResourceManager.GetObject(name);
        }


        /// <summary>
        /// Create a dynamic icon with text content loaded from a Stream
        /// </summary>
        /// <param name="label">The label at the top of the icon (eg. "TXT")</param>
        /// <param name="fileStream">The stream to load the text content from</param>
        /// <param name="size">The size of the icon to create (eg. 32x32)</param>
        /// <returns>An Icon containing a single size RGBA image</returns>
        public static Icon CreateAsciiFileIcon(string label, Stream fileStream, uint size)
        {
            IconEntry iconEntry = GetIconEntry(size);
            Bitmap bitmap = LoadBitmapResource(iconEntry.name);

            // Read up to ten lines of text
            var previewLines = new List<string>();
            using (var reader = new StreamReader(fileStream))
            {
                for (int i = 0; i < 10; i++)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;
                    previewLines.Add(line);
                }
            }

            // Draw text overlay
            using (var graphics = Graphics.FromImage(bitmap))
            {
                RenderTextOverlay(graphics, previewLines, iconEntry.contentBounds);
            }

            return BitmapToIcon(bitmap);
        }

        /// <summary>
        /// Convert a Bitmap to an Icon
        /// (.NET doesn't have any built-in methods that do this properly)
        /// </summary>
        private static Icon BitmapToIcon(Bitmap bitmap)
        {
            SingleIcon si = new SingleIcon("icon");
            si.Add(bitmap);
            return si.Icon;
        }

        /// <summary>
        /// Render dynamic text content onto a graphics canvas
        /// </summary>
        private static void RenderTextOverlay(Graphics graphics, IEnumerable<string> previewLines, Rectangle contentBounds)
        {
            var font = new Font("Consolas", 4.0f);
            var brush = new SolidBrush(Color.Black);

            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            graphics.Clip = new Region(contentBounds);

            float y = contentBounds.Top;
            foreach (var line in previewLines)
            {
                graphics.DrawString(line, font, brush, contentBounds.Left, y);
                y += 5.0f;
            }
        }
    }
}
