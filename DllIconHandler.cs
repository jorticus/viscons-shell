using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using SharpShell.SharpIconHandler;
using SharpShell.Attributes;
using System.Drawing.IconLib;
using System.IO;

//http://sharpshell.codeplex.com/documentation

namespace Viscons.ShellHandler
{
    /// <summary>
    /// Provides separate icons for Native and Managed (.NET) DLLs
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".dll")]
    class DllIconHandler : SharpIconHandler
    {
        private static IconImage LoadIconResource(string name, Size size)
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
            var icon = iconFile[0];

            // Find the correct size
            foreach (var image in icon)
            {
                if (image.Size == size)
                    return image;
            }

            throw new Exception(String.Format(
                "Could not find matching size ({0}) for icon '{1}'",
                size, name));
        }

        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            Log(String.Format("DLL Icon (size: {0})", iconSize));

            var size = new Size((int)iconSize, (int)iconSize);

            //  The icon we'll return.
            IconImage icon = null;

            //  Check the assembly name. If it's a native dll, this'll throw an exception.
            try
            {
                //  SelectedItemPath is provided by 'SharpIconHandlder' and contains the path of the file.
                AssemblyName.GetAssemblyName(SelectedItemPath);
            }
            catch (BadImageFormatException)
            {
                //  The file is not an assembly.
                icon = LoadIconResource("dll_native", size);
            }
            catch (Exception)
            {
                //  Some other eception occured, so assume we're native.
                icon = LoadIconResource("dll_native", size);
            }

            //  If we haven't determined that the dll is native, use the managed icon.
            if (icon == null)
                icon = LoadIconResource("dll_managed", size);

            //  Return the icon with the correct size. Use the SharpIconHandler 'GetIconSpecificSize'
            //  function to extract the icon of the required size.
            //return new Icon(icon, new Size((int)iconSize, (int)iconSize));
            return icon.Icon;
            //return GetIconSpecificSize(icon, new Size((int)iconSize, (int)iconSize));
        }
    }
}
