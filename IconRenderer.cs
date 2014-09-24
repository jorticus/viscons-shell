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
using System.Drawing.Drawing2D;

namespace Viscons.ShellHandler
{
    /// <summary>
    /// Preset theme colours
    /// </summary>
    static public class ThemeColors {
        static public Color Accent = Color.FromArgb(0x99, 0xAC, 0xF3);
        static public Color Background = Color.White;
        static public Color Foreground = Color.Black;
    }

    /// <summary>
    /// Internal struct used for configuring various parameters
    /// used in rendering the icons
    /// </summary>
    public struct IconConfig
    {
        public uint IconSize;
        public string IconName;
        
        public Font LabelFont;
        public PointF LabelPos;
        public Color LabelStrokeColor;
        public Color LabelOutlineColor;

        public Region ClipRegion;

        // TextIconRenderer-specific configs
        public PointF ContentPos;
        public Font ContentFont;
        public Color ContentColor;
    }

    /// <summary>
    /// Static class containing various icon theme configs
    /// </summary>
    static public class IconThemeConfigs {
        // Static configuration for dynamically constructed icons
        static public readonly Dictionary<uint, IconConfig> WhiteThemeConfigs = new Dictionary<uint, IconConfig> {
            {32, new IconConfig { 
                IconName = "white32px", 
                IconSize = 32,

                LabelFont = new Font("ProFontWindows", 11.0f),
                LabelPos = new PointF(7f, 1f),
                LabelStrokeColor = ThemeColors.Accent,
                LabelOutlineColor = ThemeColors.Background,

                ClipRegion = new Region(new GraphicsPath(new Point[] {
                    new Point(7, 3), 
                    new Point(19, 3),
                    new Point(19, 10),
                    new Point(26, 10),
                    new Point(26, 29),
                    new Point(7, 29),
                }, new byte[] { 
                    (byte)PathPointType.Start,
                    (byte)PathPointType.Line,
                    (byte)PathPointType.Line,
                    (byte)PathPointType.Line,
                    (byte)PathPointType.Line,
                    (byte)PathPointType.Line,
                })),

                ContentPos = new PointF(6f, 11f),
                ContentFont = new Font("Consolas", 4.0f),
                ContentColor = Color.Black,
            }},
            /*{48, new TextIconRenderer { 
                IconFilename = "white48px", 
                IconSize = 48,
                TextFont = new Font("Consolas", 6.0f),
            }},
            {256, new TextIconRenderer { 
                IconFilename = "white256px", 
                IconSize = 48,
                TextFont = new Font("Consolas", 16.0f),
            }},*/
        };

        static public IconConfig? GetConfig(uint iconSize, Dictionary<uint, IconConfig> configTheme)
        {
            // Retrieve the config for the specified size
            IconConfig config;
            if (!configTheme.TryGetValue(iconSize, out config))
                return null;

            return config;
        }
    }

    /// <summary>
    /// Icon renderer class, allows any kind of icon to be rendered from
    /// generic components. Subclass this to change the type of overlay that is drawn
    /// </summary>
    public class IconRenderer
    {
        // The config contains parameters specific to a certain icon size
        protected IconConfig config;
        protected string label;

        public IconRenderer(IconConfig config)
        {
            this.config = config;
        }

        public virtual Icon Render(string label)
        {
            this.label = label; // Store for later use

            // Load the base icon
            Bitmap bitmap = IconUtils.LoadBitmapResource(config.IconName);

            // Render the overlay (specific to the type of icon renderer)
            using (var graphics = Graphics.FromImage(bitmap))
            {
                RenderOverlay(graphics);
            }

            // Compose the Icon
            return IconUtils.BitmapToIcon(bitmap);
        }

        protected virtual void RenderOverlay(Graphics graphics)
        {
            // Draw overlay stuff here
            graphics.ResetClip();

            if (this.label != null && this.label != "")
            {
                // Create a path from the label string using the config fon
                StringFormat fmt = StringFormat.GenericTypographic;
                GraphicsPath labelPath = new GraphicsPath();
                labelPath.AddString(this.label, config.LabelFont.FontFamily, (int)FontStyle.Bold, config.LabelFont.Size, config.LabelPos, fmt);

                // Enable smoothing
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Draw the path with outline
                graphics.DrawPath(new Pen(config.LabelOutlineColor, 2.0f), labelPath);
                graphics.FillPath(new SolidBrush(config.LabelStrokeColor), labelPath);
            }
        }
    }

    /// <summary>
    /// Subclass of IconRenderer, renders dynamic text content for the overlay layer
    /// </summary>
    public class TextIconRenderer : IconRenderer
    {
        protected List<string> content;

        public TextIconRenderer(IconConfig config) : base(config) { }

        public Icon Render(string label, List<string> content)
        {
            // Intercept the original Render call so we can pass it the content
            this.content = content;

            // Render normally (will call RenderOverlay when required)
            return base.Render(label);
        }

        protected override void RenderOverlay(Graphics graphics)
        {
            // Draw text overlay here (text is stored in this.content)

            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clip = config.ClipRegion;
            
            // Uncomment to show the ContentBounds (For debugging)
            //graphics.FillRegion(Brushes.Red, config.ContentBounds);

            if (content != null && content.Count() > 0)
            {
                var brush = new SolidBrush(config.ContentColor);
                var font = config.ContentFont;
                var loc = config.ContentPos;

                float y = loc.Y;
                foreach (var line in content)
                {
                    graphics.DrawString(line, font, brush, loc.X, y);
                    y += font.Size;
                }
            }

            // Draw original icon overlay
            base.RenderOverlay(graphics);
        }

    }
}
