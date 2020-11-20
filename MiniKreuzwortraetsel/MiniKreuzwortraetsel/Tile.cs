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
        Brush BackgroundColor = Brushes.White;
        Brush ForeGroundColor = Brushes.Blue;
        bool Reserved = false;
        public List<Point> HighlightDirections = new List<Point>();

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
        public bool GetBackgroundPolygon(int ts, out Point[] polygon, out Brush color)
        {
            color = BackgroundColor;

            // Triangle
            if (HighlightDirections.Count == 1)
                polygon = new Point[3] { new Point(), new Point(), new Point() };
            // Rectangle
            else
                polygon = new Point[4] { new Point(Position.X, Position.Y), new Point(Position.X + ts, Position.Y), new Point(Position.X + ts, Position.Y + ts), new Point(Position.X, Position.Y + ts) };

        }
        public void SetBackgroundColor(Brush color)
        {
            BackgroundColor = color;
        }
        public bool IsHighlighted()
        {
            if (BackgroundColor != Brushes.White)
                return true;
            else
                return false;
        }
        public bool HasRectangle()
        {
            // Conditions: has background color or text
            if (GetText(out _))
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
