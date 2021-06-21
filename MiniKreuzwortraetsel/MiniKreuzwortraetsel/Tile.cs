using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace MiniKreuzwortraetsel
{
    abstract class Tile
    {
        public static (string Question, string Answer) tupleToBeFilled;
        protected enum ExtendedHover
        {
            Off = -1,
            Two_Outlines_Horizontal = 0,
            Three_Outlines_Horizontal = 1,
            Two_Outlines_Vertical = 2,
            Three_Outlines_Vertical = 3,
        }

        Point position;
        string text = "";
        Font font = new Font("Verdana", 9.75f, FontStyle.Bold);
        Brush foregroundColor = Brushes.Blue;
        /// <summary>
        /// Determines if this tile should have red outline based on question tile hover pointing to it, 
        /// -1 = off, 0 = 2 outlines horizontal, 1 = 3 outlines horizontal, 2 = 2 outlines vertical, 3 = 3 outlines vertical
        /// </summary>
        protected ExtendedHover extendedHover = ExtendedHover.Off;
        protected Pen extendedHoverPen = new Pen(Brushes.Red, 5);

        public Tile(Point position)
        {
            this.position = position;
        }

        public void ToEmptyTile(Tile[,] grid)
        {
            grid[position.Y, position.X] = new EmptyTile(position);
        }

        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public abstract Image GetGraphics(int ts)
        {
            // Dispose Image and Graphics to prevent memory leak
            Image canvas = new Bitmap(ts, ts);
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                // Draw highlights
                for (int i = 0; i < subTiles.Length; i++)
                {
                    if (SubtileHighlightColors[i] != null)
                        graphics.FillPolygon(SubtileHighlightColors[i], subtileTriangles[i]);
                }

                // Draw hover effect
                if (hoverSubtile != -1)
                {
                    graphics.FillPolygon(Brushes.Blue, subtileTriangles[hoverSubtile]);
                    graphics.DrawString(hoverArrows[hoverSubtile], arrowFont, Brushes.Red, arrowPositions[hoverSubtile]);
                }

                // Draw text
                if (GetText(out _))
                {
                    Size textSize = TextRenderer.MeasureText(text, font);
                    graphics.DrawString(text, font, foregroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);
                }

                // Draw Rectangle
                // Conditions: has background color or text
                if (GetText(out _) || IsHighlighted())
                    graphics.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);

                // Draw extendedHover
                switch (extendedHover)
                {
                    case ExtendedHover.Two_Outlines_Horizontal:
                        graphics.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                        graphics.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                        break;
                    case ExtendedHover.Three_Outlines_Horizontal:
                        graphics.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                        graphics.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                        graphics.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                        break;
                    case ExtendedHover.Two_Outlines_Vertical:
                        graphics.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                        graphics.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                        break;
                    case ExtendedHover.Three_Outlines_Vertical:
                        graphics.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                        graphics.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                        graphics.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                        break;
                }

                return canvas;
            }
        }
        /// <summary>
        /// Checks if subtile should activate hover effect and does so if necessary, 
        /// </summary>
        public void ActivateHover(int mouseX, int mouseY, int ts, Tile[,] grid, PictureBox gridPB, Point[] directions)
        {
            // Get old state of hover effect
            string hoverStateOld = "";
            if (currentHoveringTile != null)
                hoverStateOld += "active-" + currentHoveringTile.position.X + "-" + currentHoveringTile.position.Y + "-" + currentHoveringTile.hoverSubtile + "-";
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
            if (mouseX - position.X * ts < mouseY - position.Y * ts)
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
                hoverStateNew += "active-" + currentHoveringTile.position.X + "-" + currentHoveringTile.position.Y + "-" + currentHoveringTile.hoverSubtile + "-";
            else
                hoverStateNew += "inactive-";

            // Did hover effect change?
            if (hoverStateNew != hoverStateOld)
            {
                RemoveAllExtendedHover(grid);
                if (newTile != null)
                {
                    // Activate extendedHover for adjacent tiles
                    Point directionPoint = directions[hoverSubtile];
                    for (int i = 0; i < tupleToBeFilled.Answer.Length; i++)
                    {
                        int letterX = position.X + directionPoint.X * (1 + i);
                        int letterY = position.Y + directionPoint.Y * (1 + i);
                        // out of bounds check
                        if (letterX <= grid.GetUpperBound(1) && letterY <= grid.GetUpperBound(0))
                        {
                            // End or middle outline
                            if (i < tupleToBeFilled.Answer.Length - 1)
                                grid[letterY, letterX].extendedHover = 0;
                            else
                                grid[letterY, letterX].extendedHover = 1;

                            // Vertical mode
                            if (directionPoint.Y == 1) 
                                grid[letterY, letterX].extendedHover += 2;
                        }
                    }
                }

                gridPB.Refresh();
            }
        }
        public bool IsQuestionTile()
        {
            return isQuestionTile;
        }
        public void SetQuestionTile()
        {
            isQuestionTile = true;
            foregroundColor = Brushes.Red;
            font = arrowFont;
            questionTileList.Add(this);
            Reserved = false;
            hoverSubtile = -1;
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
        public static void RemoveAllExtendedHover(Tile[,] grid)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x].extendedHover = -1;
                }
            }
        }
        public bool GetText(out string text)
        {
            text = this.text;
            if (this.text != "")
                return true;
            else
                return false;
        }
        public void SetText(string text)
        {
            this.text = text;
            setTextCounter++;
        }
        public int GetSetTextCounter()
        {
            return setTextCounter;
        }
        public void DecreaseSetTextCounter()
        {
            setTextCounter--;
        }
        public Point GetPosition()
        {
            return position;
        }
        public Point GetWorldPosition(int ts)
        {
            return new Point(position.X * ts, position.Y * ts);
        }
        public bool IsReserved()
        {
            return Reserved;
        }
        public void SetReserved(bool reserved)
        {
            Reserved = reserved;
        }
        /// <summary>
        /// Links a letter tile to this question Tile, important for deleting
        /// </summary>
        public void AddLinkedLetterTile(Tile letterTile)
        {
            linkedLetterTiles.Add(letterTile);
        }
        public List<Tile> GetLinkedLetterTiles()
        {
            return linkedLetterTiles;
        }
        public void SetQuestion(string question)
        {
            this.question = question;
        }
        public string GetQuestion()
        {
            return question;
        }
        static public List<Tile> GetQuestionTileList()
        {
            return questionTileList;
        }
        public void RemoveFromQuestionTileList()
        {
            questionTileList.Remove(this);
        }
    }
}
