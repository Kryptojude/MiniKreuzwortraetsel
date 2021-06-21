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
        static Dictionary<string, Point[]> subTileTriangles = new Dictionary<string, Point[]>() {
                                                                { "horizontal", new Point[3] { new Point(0, 0), new Point(Form1.ts, 0),  new Point(Form1.ts, Form1.ts) } },
                                                                { "vertical",   new Point[3] { new Point(0, 0), new Point(Form1.ts, Form1.ts), new Point(0, Form1.ts) } },
                                                              };
        static string[] hoverArrows = new string[2] { "►", "▼" };
        static Point[] arrowPositions = new Point[] { new Point(Form1.ts / 3, 0), new Point(-3, 2 * (Form1.ts / 5)) };

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

        public Point[] GetSubTileTriangle()
        {
            return subTileTriangles[direction];
        }

        public EmptyTile GetParentTile()
        {
            return parentTile;
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
