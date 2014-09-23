using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using SharpShell.SharpIconHandler;
using SharpShell.Attributes;

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
        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            //  The icon we'll return.
            Icon icon = null;

            //  Check the assembly name. If it's a native dll, this'll throw an exception.
            try
            {
                //  SelectedItemPath is provided by 'SharpIconHandlder' and contains the path of the file.
                AssemblyName.GetAssemblyName(SelectedItemPath);
            }
            catch (BadImageFormatException)
            {
                //  The file is not an assembly.
                icon = Properties.Resources.dll_native;
            }
            catch (Exception)
            {
                //  Some other eception occured, so assume we're native.
                icon = Properties.Resources.dll_native;
            }

            //  If we haven't determined that the dll is native, use the managed icon.
            if (icon == null)
                icon = Properties.Resources.dll_managed;

            //  Return the icon with the correct size. Use the SharpIconHandler 'GetIconSpecificSize'
            //  function to extract the icon of the required size.
            return GetIconSpecificSize(icon, new Size((int)iconSize, (int)iconSize));
        }
    }
}
