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
        static SubTile hoverSubTile;
        static Brush hoverBrush = Brushes.Blue;
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
        string direction;
        EmptyTile parentTile;

        public SubTile(string direction, EmptyTile parentTile)
        {
            this.direction = direction;
            this.parentTile = parentTile;
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

        public EmptyTile GetParentTile()
        {
            return parentTile;
        }

        public Point[] GetSubTilePolygon()
        {
            return subTilePolygons[direction];
        }

        public string GetHoverArrow()
        {
            return hoverArrows[direction];
        }

        public Point GetArrowPosition()
        {
            return arrowPositions[direction];
        }

        static public SubTile GetHoverSubTile()
        {
            return hoverSubTile;
        }

        static public Brush GetHoverBrush()
        {
            return hoverBrush;
        }
          
    }
}
