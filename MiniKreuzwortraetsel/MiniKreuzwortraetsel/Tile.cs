﻿using System;
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
        public static Tile currentHoveringTile;
        /// <summary>
        /// If this list has children, then only they will be repainted in the Form1.Paint method, 
        /// this saves RAM and CPU, needs no additional Garbage Collection
        /// </summary>
        public static List<Tile> ExclusiveRedraw = new List<Tile>();

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

        public Tile(int x, int y, int ts)
        {
            Position = new Point(x, y);
            // Generate tile-space coordinates for the two subtiles and the arrowPositions
            subtileTriangles = new Point[2][] { new Point[3] { new Point(0, 0), new Point(ts, 0),  new Point(ts, ts) },
                                                new Point[3] { new Point(0, 0), new Point(ts, ts), new Point(0, ts) } };

            arrowPositions = new Point[] { new Point(ts / 3, 0), new Point(-3, 2 * (ts / 5)) };
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
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
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
            graphics.DrawString(Text, font, ForegroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);

            // Draw Rectangle
            // Conditions: has background color or text
            if (GetText(out _) || IsHighlighted())
                graphics.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);

            return canvas;
        }
        /// <summary>
        /// Checks if subtile should activate hover effect and does so if necessary, 
        /// also returns whether hover effect has changed, and saves the affected tiles in Tile.ExclusiveRedraw
        /// </summary>
        public void ActivateHover(int mouseX, int mouseY, int ts, out bool hasHoverChanged)
        {
            // Get old state of hover effect
            Tile oldTile = currentHoveringTile;
            string hoverStateOld = "";
            if (currentHoveringTile != null)
                hoverStateOld += "active-" + currentHoveringTile.Position.X + "-" + currentHoveringTile.Position.Y + "-" + currentHoveringTile.hoverSubtile + "-";
            else
                hoverStateOld += "inactive-";

            // Remove old hover effect
            if (currentHoveringTile != null)
            {
                currentHoveringTile.hoverSubtile = -1;
                currentHoveringTile = null;
            }

            // Determine the subtile the mouse is hovering over
            int mouseSubtile = 0;
            if (mouseX - Position.X * ts < mouseY - Position.Y * ts)
                mouseSubtile = 1;

            // Is that subtile highlighted?
            if (SubtileHighlightColors[mouseSubtile] != null)
            {
                //Activate Hover for this subtile
                hoverSubtile = mouseSubtile;
                currentHoveringTile = this;
            }

            // Get new state of hover effect
            Tile newTile = currentHoveringTile;
            string hoverStateNew = "";
            if (currentHoveringTile != null)
                hoverStateNew += "active-" + currentHoveringTile.Position.X + "-" + currentHoveringTile.Position.Y + "-" + currentHoveringTile.hoverSubtile + "-";
            else
                hoverStateNew += "inactive-";

            // Did hover effect change?
            if (hoverStateNew != hoverStateOld)
            {
                hasHoverChanged = true;
                // Tell paint method to only redraw the two affected tiles,
                // (one lost hover effect, one gained hover effect)
                if (oldTile != newTile)
                {
                    if (oldTile != null)
                        ExclusiveRedraw.Add(oldTile);
                    if (newTile != null)
                        ExclusiveRedraw.Add(newTile);
                }
                // If both tiles are the same, then only the hoverSubtile within the same tile was changed, 
                // so only redraw that one tile, instead of drawing the same one twice
                else
                    if (oldTile != null)
                    ExclusiveRedraw.Add(oldTile);
            }
            else
                hasHoverChanged = false;
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
        public void SetForegroundColor(Brush color)
        {
            ForegroundColor = color;
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
