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

        static public readonly Font HOVER_ARROW_FONT = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        static readonly Dictionary<string, Point[]> subTilePolygons = new Dictionary<string, Point[]>() {
                                                                 { "horizontal", new Point[3] { new Point(0, 0), new Point(Form1.TS, 0),  new Point(Form1.TS, Form1.TS) } },
                                                                 { "vertical",   new Point[3] { new Point(0, 0), new Point(Form1.TS, Form1.TS), new Point(0, Form1.TS) } },
                                                              };
        static readonly Dictionary<string, string> Arrows = new Dictionary<string, string>() {
                                                                    { "horizontal", "►" },
                                                                    { "vertical", "▼" } };
        static public string GetArrow(string direction)
        {
            return Arrows[direction];
        }
        static public string GetArrow(int direction)
        {
            return Arrows.ElementAt(direction).Value;
        }

        static readonly Dictionary<string, Point> arrowPositions = new Dictionary<string, Point>() {
                                                                       { "horizontal", new Point(Form1.TS / 3, 0) },
                                                                       { "vertical", new Point(-3, 2 * (Form1.TS / 5)) },
                                                                   };

        static public readonly Color MinColor = Color.FromArgb(0x9be8a1);
        static public readonly Color MaxColor = Color.FromArgb(0x00ff14);

        Brush color = Brushes.White;
        public int Direction { get; }
        public EmptyTile ParentTile { get; }

        public SubTile(int direction, EmptyTile parentTile)
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
            if (Direction == 0)
                return subTilePolygons["horizontal"];
            else
                return subTilePolygons["vertical"];
        }

        public string GetArrow()
        {
            if (Direction == 0)
                return Arrows["horizontal"];
            else
                return Arrows["vertical"];
        }

        public Point GetArrowPosition()
        {
            if (Direction == 0)
                return arrowPositions["horizontal"];
            else
                return arrowPositions["vertical"];
        }  
    }
}
