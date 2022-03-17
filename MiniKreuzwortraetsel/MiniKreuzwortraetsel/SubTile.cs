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
        static public readonly Font HOVER_ARROW_FONT = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        static readonly Point[][] subTilePolygons = new Point[][] {
            new Point[3] { new Point(0, 0), new Point(Form1.TS, 0),  new Point(Form1.TS, Form1.TS) },
            new Point[3] { new Point(0, 0), new Point(Form1.TS, Form1.TS), new Point(0, Form1.TS) }
        };

        static readonly Point[] arrowPositions = new Point[] { new Point(Form1.TS / 3, 0), new Point(-3, 2 * (Form1.TS / 5)) };

        static public readonly Color MinColor = Color.FromArgb(0x9be8a1);
        static public readonly Color MaxColor = Color.FromArgb(0x00ff14);

        private bool hover_flag;
        Brush color;
        public int Direction { get; }
        public EmptyTile ParentTile { get; }

        public SubTile(int direction, EmptyTile parentTile)
        {
            color = Brushes.White;
            Direction = direction;
            ParentTile = parentTile;
        }
        public void RemoveHighlight()
        {
            color = Brushes.White;
            ParentTile.SetRepaintFlag(true);
        }
        public void SetHighlight(float colorLevel)
        {
            color = new SolidBrush(Color.FromArgb((int)(MinColor.R + (MaxColor.R - MinColor.R) * colorLevel), (int)(MinColor.G + (MaxColor.G - MinColor.G) * colorLevel), (int)(MinColor.B + (MaxColor.B - MinColor.B) * colorLevel)));
        }
        public bool IsHighlighted()
        {
            return color != Brushes.White;
        }
        public void SetHoverFlag(bool flag)
        {
            hover_flag = flag;
        }
        public bool GetHoverFlag()
        {
            return hover_flag;
        }
        public Brush GetColor()
        {
            return color;
        }

        public void Paint (Graphics g)
        {
            // Hover flag set to true?
            if (GetHoverFlag())
            {
                // Draw hover effect
                g.FillPolygon(Brushes.Blue, subTilePolygons[Direction]);
                g.DrawString(Tile.GetArrow(Direction), HOVER_ARROW_FONT, Brushes.Red, arrowPositions[Direction]);
            }
            // No hover flag, so draw highlight
            else
                // Draw highlight
                g.FillPolygon(GetColor(), subTilePolygons[Direction]);

        }
    }
}
