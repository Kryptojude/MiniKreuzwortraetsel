using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class Tile
    {
        public static (string Question, string Answer) tupleToBeFilled;

        Point Position;
        string Text = "";
        Brush ForeGroundColor = Brushes.Blue;
        bool Reserved = false;
        public List<(Point Direction, Brush Color)> HighlightDirectionsAndColors = new List<(Point, Brush)>();

        public Tile(int x, int y)
        {
            Position = new Point(x, y);
        }
        public bool IsQuestionTile()
        {
            if (Text.Contains("►") || Text.Contains("▼"))
                return true;
            else
                return false;
        }
        public bool GetQuestionDirection(out Point direction)
        {
            direction = new Point();
            if (Text.Contains("►"))
                direction = new Point(1, 0);
            else if (Text.Contains("▼"))
                direction = new Point(0, 1);

            return IsQuestionTile();
        }
        /// <summary>
        /// Shit code
        /// </summary>
        public List<(Point[] Polygon, Brush Color)> GetBackgroundPolygon(int ts)
        {
            List<(Point[] Polygon, Brush Color)> polygonsAndColors = new List<(Point[] Polygon, Brush Color)>();
            for (int i = 0; i < HighlightDirectionsAndColors.Count; i++)
            {
                polygonsAndColors.Add((new Point[3] { new Point(Position.X, Position.Y), new Point(Position.X + 1, Position.Y + 1), new Point() }, HighlightDirectionsAndColors[i].Color));
                // Vertical
                if (HighlightDirectionsAndColors[i].Direction.X == 0)
                    polygonsAndColors.Last().Polygon[2] = new Point(Position.X, Position.Y + 1);
                // Horizontal
                else if (HighlightDirectionsAndColors[i].Direction.X == 1)
                    polygonsAndColors.Last().Polygon[2] = new Point(Position.X + 1, Position.Y);
            }

            // Scale from grid space to world space
            for (int polygon = 0; polygon < polygonsAndColors.Count; polygon++)
            {
                for (int point = 0; point < polygonsAndColors[polygon].Polygon.Length; point++)
                {
                    polygonsAndColors[polygon].Polygon[point].X *= ts;
                    polygonsAndColors[polygon].Polygon[point].Y *= ts;
                }
            }

            return polygonsAndColors;
        }
        public bool IsHighlighted()
        {
            if (HighlightDirectionsAndColors.Count > 0)
                return true;
            else
                return false;
        }
        public bool HasRectangle()
        {
            // Conditions: has background color or text
            if (GetText(out _) || HighlightDirectionsAndColors.Count > 0)
                return true;
            else
                return false;
        }
        public bool GetText(out string text)
        {
            text = Text;
            if (Text != "")
                return true;
            else
                return false;
        }
        public void SetText(string text)
        {
            Text = text;
        }
        public Point GetPosition()
        {
            return Position;
        }
        public Brush GetForegroundColor()
        {
            return ForeGroundColor;
        }
        public bool IsReserved()
        {
            return Reserved;
        }
        public void SetReserved(bool reserved)
        {
            Reserved = reserved;
        }
    }
}
