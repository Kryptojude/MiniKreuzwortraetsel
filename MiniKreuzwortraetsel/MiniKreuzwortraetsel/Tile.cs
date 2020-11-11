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
        public Point Position;
        public string Text = "";
        public Brush BackgroundColor = Brushes.White;
        public Brush ForeGroundColor = Brushes.Blue;

        public bool Reserved = false;

        static public (string Question, string Answer) tupleToBeFilled;
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

        public void RemoveAllHighlights()
        {
            throw new NotImplementedException();
        }
    }
}
