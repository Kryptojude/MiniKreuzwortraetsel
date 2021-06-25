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
        static Dictionary<string, Point[]> subTilePolygons = new Dictionary<string, Point[]>() {
                                                                { "horizontal", new Point[3] { new Point(0, 0), new Point(Form1.ts, 0),  new Point(Form1.ts, Form1.ts) } },
                                                                { "vertical",   new Point[3] { new Point(0, 0), new Point(Form1.ts, Form1.ts), new Point(0, Form1.ts) } },
                                                              };
        static Dictionary<string, string> hoverArrows = new Dictionary<string, string>() {
                                                            { "horizontal", "►" },
                                                            { "vertical", "▼" }
                                                        };
        static Dictionary<string, Point> arrowPositions = new Dictionary<string, Point>() {
                                                                { "horizontal", new Point(Form1.ts / 3, 0) },
                                                                { "vertical", new Point(-3, 2 * (Form1.ts / 5)) },
                                                            };

        Brush color = Brushes.White;
        public string Direction { get; }
        public EmptyTile ParentTile { get; }

        public SubTile(string direction, EmptyTile parentTile)
        {
            Direction = direction;
            ParentTile = parentTile;
        }

        public void SetHighlight(int level)
        {

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
