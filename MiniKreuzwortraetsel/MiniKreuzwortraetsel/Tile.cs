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
        Brush ForegroundColor = Brushes.Blue;
        bool Reserved = false;
        public bool IsBaseWordTile = false;
        public Brush[] SubtileHighlightColors = new Brush[2];
        /// <summary>
        /// -1 = no hover effect
        /// 0 = hover on horizontal subtile
        /// 1 = hover on vertical subtile
        /// </summary>
        public int hoverSubtile = -1;
        /// <summary>
        /// Contains the world coordinates for the two subtiles
        /// </summary>
        Point[][] subtileTriangles;
        string[] hoverArrows = new string[2] { "►", "▼" };
        Point[] arrowPositions;
        public static Tile currentHoveringTile;

        public Tile(int x, int y, int ts)
        {
            Position = new Point(x, y);
            // Generate world coordinates for the two subtiles
            subtileTriangles = new Point[2][] { new Point[3] { new Point(x * ts, y * ts), new Point((x + 1) * ts, y * ts),     new Point((x + 1) * ts, (y + 1) * ts) },
                                              new Point[3] { new Point(x * ts, y * ts), new Point((x + 1) * ts, (y + 1) * ts), new Point(x * ts, (y + 1) * ts) } };
            arrowPositions = new Point[] { new Point((x * ts) + ts / 3, y * ts), new Point(x * ts - 3, x * ts + 2 * (ts / 5)) };
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
        public Image GetGraphics(int ts, Font font)
        {
            Image canvas = new Bitmap(ts, ts);
            Graphics graphics = Graphics.FromImage(canvas);

            // Draw highlights
            for (int i = 0; i < SubtileHighlightColors.Length; i++)
            {
                if (SubtileHighlightColors[i] != null)
                    graphics.FillPolygon(SubtileHighlightColors[i], subtileTriangles[i]);
            }

            // Draw hover effect
            if (hoverSubtile != -1)
            {
                graphics.FillPolygon(Brushes.Blue, subtileTriangles[hoverSubtile]);
                graphics.DrawString(hoverArrows[hoverSubtile], font, Brushes.Red, arrowPositions[hoverSubtile]);
            }

            // Draw text
            Size textSize = System.Windows.Forms.TextRenderer.MeasureText(Text, font);
            graphics.DrawString(Text, font, ForegroundColor, Position.X * ts + ts / 2 - textSize.Width / 2, Position.Y * ts + ts / 2 - textSize.Height / 2);

            // Draw Rectangle
            if (HasRectangle())
                graphics.DrawRectangle(Pens.Black, Position.X * ts, Position.Y * ts, ts - 1, ts - 1);

            return canvas;
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
            bool highlighted = false;
            foreach (Brush brush in SubtileHighlightColors)
            {
                if (brush != null)
                    highlighted = true;
            }
            if (highlighted)
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
                    grid[y, x].SubtileHighlightColors = new Brush[2];
                }
            }
        }
        public bool HasRectangle()
        {
            bool highlighted = false;
            foreach (Brush brush in SubtileHighlightColors)
            {
                if (brush != null)
                    highlighted = true;
            }
            // Conditions: has background color or text
            if (GetText(out _) || highlighted)
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
            return ForegroundColor;
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
