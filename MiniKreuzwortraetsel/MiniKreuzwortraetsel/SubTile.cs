using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class SubTile
    {
        static public SubTile HoverSubTile;

        static public readonly Font ARROW_FONT = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        static readonly Dictionary<string, Point[]> subTilePolygons = new Dictionary<string, Point[]>() {
                                                                 { "horizontal", new Point[3] { new Point(0, 0), new Point(Form1.ts, 0),  new Point(Form1.ts, Form1.ts) } },
                                                                 { "vertical",   new Point[3] { new Point(0, 0), new Point(Form1.ts, Form1.ts), new Point(0, Form1.ts) } },
                                                              };
        static readonly Dictionary<string, string> hoverArrows = new Dictionary<string, string>() {
                                                                    { "horizontal", "►" },
                                                                    { "vertical", "▼" }
                                                                 };
        static readonly Dictionary<string, Point> arrowPositions = new Dictionary<string, Point>() {
                                                                       { "horizontal", new Point(Form1.ts / 3, 0) },
                                                                       { "vertical", new Point(-3, 2 * (Form1.ts / 5)) },
                                                                   };

        static public readonly Color MaxColor = Color.FromArgb(0x9be8a1);
        static public readonly Color MinColor = Color.FromArgb(0x00ff14);

        Brush color = Brushes.White;
        public string Direction { get; }
        public EmptyTile ParentTile { get; }

        public SubTile(string direction, EmptyTile parentTile)
        {
            Direction = direction;
            ParentTile = parentTile;
        }

        public void SetHighlight(float colorLevel)
        {
            color = new SolidBrush(Color.FromArgb((int)(MinColor.R + (MaxColor.R - MinColor.R) * colorLevel), (int)(MinColor.G + (MaxColor.G - MinColor.G) * colorLevel), (int)(MinColor.B + (MaxColor.B - MinColor.B) * colorLevel)));
        }

        public bool IsHighlighted()
        {
            return color != Brushes.White;
        }

        public Brush GetColor()
        {
            return color;
        }

        public Point[] GetSubTilePolygon()
        {
            return subTilePolygons[Direction];
        }

        public string GetHoverArrow()
        {
            return hoverArrows[Direction];
        }

        public Point GetArrowPosition()
        {
            return arrowPositions[Direction];
        }  
    }
}
