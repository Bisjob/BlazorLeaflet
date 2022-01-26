using System.Drawing;

namespace BlazorLeaflet.Models
{
    public class Icon
    {

        /// <summary>
        /// (required) The URL to the icon image (absolute or relative to your script path).
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The URL to a retina sized version of the icon image (absolute or relative to your script path). Used for Retina screen devices.
        /// </summary>
        public string RetinalUrl { get; set; }

        /// <summary>
        /// Width of the icon image in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the icon image in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The coordinates of the "tip" of the icon (relative to its top left corner). The icon will be aligned so that this point is at the marker's geographical location. Centered by default if size is specified, also can be set in CSS with negative margins.
        /// </summary>
        //public Point? Anchor { get; set; }
        public int AnchorX { get; set; }
        public int AnchorY { get; set; }

        /// <summary>
        /// The coordinates of the point from which popups will "open", relative to the icon anchor.
        /// </summary>
        public Point PopupAnchor { get; set; } = Point.Empty;

        /// <summary>
        /// The coordinates of the point from which tooltips will "open", relative to the icon anchor.
        /// </summary>
        public Point TooltipAnchor { get; set; } = Point.Empty;

        /// <summary>
        /// The URL to the icon shadow image. If not specified, no shadow image will be created.
        /// </summary>
        public string ShadowUrl { get; set; }

        public string ShadowRetinalUrl { get; set; }

        /// <summary>
        /// Size of the shadow image in pixels.
        /// </summary>
        public Size? ShadowSize { get; set; }

        /// <summary>
        /// The coordinates of the "tip" of the shadow (relative to its top left corner) (the same as iconAnchor if not specified).
        /// </summary>
        public Size? ShadowAnchor { get; set; }

        /// <summary>
        /// A custom class name to assign to both icon and shadow images. Empty by default.
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// True to set An IconDiv (ignore iconUrl)
        /// </summary>
        public bool IsIconDiv { get; set; }

        /// <summary>
        /// Custom HTML code to put inside the div element, empty by default. Alternatively, an instance of HTMLElement.
        /// </summary>
        public string HTML { get; set; }

        /// <summary>
        /// Optional relative position of the background, in pixels
        /// </summary>
        public Point BgPos { get; set; }
    }
}
