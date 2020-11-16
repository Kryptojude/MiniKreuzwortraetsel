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
        public Brush GetBackgroundColor()
        {
            Brush color = BackgroundColor;
            if (BackgroundColor != null)
                return color;
            else
                return color;
        }
        public void SetBackgroundColor(Brush color)
        {
            BackgroundColor = color;
        }
        // Need to differentiate between the question tile highlight and the mouse over highlight later
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
