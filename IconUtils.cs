using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.IconLib;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viscons.ShellHandler;

namespace Viscons.IconRenderer
{
    public static class IconUtils
    {
        /// <summary>
        /// Select a single icon format of the given size
        /// </summary>
        /// <param name="icon">An icon containing one or more formats</param>
        /// <param name="size">The exact size to locate (eg. 32x32)</param>
        /// <returns>An icon containing exactly one format</returns>
        public static Icon GetIconSpecificSize(Icon icon, Size size)
        {
            MultiIcon iconFile = new MultiIcon();

            // Create a memory stream.
            using (var memoryStream = new MemoryStream())
            {
                // Save the icon to the stream, then seek to the origin.
                icon.Save(memoryStream);
                memoryStream.Position = 0;

                iconFile.Load(memoryStream);
            }

            // MultiIcon can contain multiple icons (eg. ICL/DLL files), 
            // but in this case it will only ever contain one icon.
            var singleIcon = iconFile[0];

            // Find the correct size
            foreach (var image in singleIcon)
            {
                if (image.Size == size)
                    return image.Icon;
            }

            throw new Exception(String.Format("Could not find matching size ({0}) for icon", size));
        }

        /// <summary>
        /// Convert a Bitmap to an Icon
        /// (.NET doesn't have any built-in methods that do this properly)
        /// </summary>
        public static Icon BitmapToIcon(Bitmap bitmap)
        {
            SingleIcon si = new SingleIcon("icon");
            si.Add(bitmap);
            return si.Icon;
        }

        /// <summary>
        /// Load an Icon as a bitmap of the specified size
        /// </summary>
        public static Bitmap LoadIconResource(string name, Size size)
        {
            Icon icon = (Icon)Viscons.ShellHandler.Properties.Resources.ResourceManager.GetObject(name);
            return new Icon(icon, size).ToBitmap();
        }

        /// <summary>
        /// Load a Bitmap from Resources by name
        /// </summary>
        public static Bitmap LoadBitmapResource(string name)
        {
            return (Bitmap)Viscons.ShellHandler.Properties.Resources.ResourceManager.GetObject(name);
        }
    }
}
