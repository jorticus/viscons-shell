using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpShell.Attributes;
using SharpShell.SharpIconHandler;

namespace Viscons.ShellHandler
{
    /// <summary>
    /// Provides content-aware icons for "unknown" file types
    /// </summary>
    /*[ComVisible(true)]
    [COMServerAssociation(AssociationType.UnknownFiles)]
    class UnknownIconHandler : SharpIconHandler
    {
        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            Icon icon = Properties.Resources.blank;
            //TODO: Automatically detect empty, ASCII, and binary files.
            // Perhaps also detect files by MIME type??

            //  Return the icon with the correct size. Use the SharpIconHandler 'GetIconSpecificSize'
            //  function to extract the icon of the required size.
            return null;// GetIconSpecificSize(icon, new Size((int)iconSize, (int)iconSize));
        }
    }*/
}
