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
        public bool IsBaseWordTile = false;
        public List<(Point Direction, Brush Color)> HighlightDirectionsAndColors = new List<(Point, Brush)>();
        /// <summary>
        /// -1 = no hover effect
        /// 0 = hover on horizontal subtile
        /// 1 = hover on vertical subtile
        /// </summary>
        public int hoverSubtile = -1;
        /// <summary>
        /// Contains the grid coordinates for the two subtiles
        /// </summary>
        Point[][] HoverTriangles;
        public static Tile currentHoveringTile;

        public Tile(int x, int y)
        {
            Position = new Point(x, y);
            // Generate grid coordinates for the two subtiles
            HoverTriangles = new Point[2][] { new Point[3] { new Point(x, y), new Point(x + 1, y),     new Point(x + 1, y + 1) },
                                              new Point[3] { new Point(x, y), new Point(x + 1, y + 1), new Point(x, y + 1) } };
        }

        //MakeHoverTriangles()

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
        public List<(Point[] Polygon, Brush Color)> GetVisuals(int ts, out string arrow, out Point arrowPos)
        {
            arrow = "";
            arrowPos = new Point();
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

            // Check for Hover effect
            if (hoverSubtile != -1)
            {
                // Hover effect is active on this tile
                polygonsAndColors.Add(((Point[])HoverTriangles[hoverSubtile].Clone(), Brushes.Blue));
                if (hoverSubtile == 0)
                {
                    arrow = "►";
                    arrowPos = new Point((Position.X * ts) + ts / 3, Position.Y * ts);
                }
                else
                {
                    arrow = "▼";
                    arrowPos = new Point(Position.X * ts - 3, Position.Y * ts + 2 * (ts / 5));
                }
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
        /// <summary>
        /// Checks if subtile should activate hover effect and does so if necessary
        /// </summary>
        public void ActivateHover(int mouseX, int mouseY, int ts)
        {
            // Remove old hover effect
            if (currentHoveringTile != null)
            {
                currentHoveringTile.hoverSubtile = -1;
                currentHoveringTile = null;
            }

            // Determine the subtile the mouse is over
            int subtile = 0;
            if (mouseX - Position.X * ts < mouseY - Position.Y * ts)
                subtile = 1;

            // Check if that subtile is highlighted
            bool subtileIsHighlighted = false;
            foreach (var item in HighlightDirectionsAndColors)
            {
                if (item.Direction.X == 1 && subtile == 0)
                    subtileIsHighlighted = true;
                else if (item.Direction.X == 0 && subtile == 1)
                    subtileIsHighlighted = true;
            }

            if (subtileIsHighlighted)
            {
                //Activate Hover for this subtile
                hoverSubtile = subtile;
                currentHoveringTile = this;
            }
        }
        public bool IsHighlighted()
        {
            if (HighlightDirectionsAndColors.Count > 0)
                return true;
            else
                return false;
        }
        public static void RemoveAllHighlights(Tile[,] grid)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x].HighlightDirectionsAndColors.Clear();
                }
            }
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
