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
        public static Tile currentHoveringTile;
        static List<Tile> questionTileList = new List<Tile>();

        Point position;
        string text = "";
        public Font font;
        Brush ForegroundColor = Brushes.Blue;
        bool Reserved = false;
        public Brush[] SubtileHighlightColors = new Brush[2];
        /// <summary>
        /// -1 = no hover effect
        /// 0 = hover on horizontal subtile
        /// 1 = hover on vertical subtile
        /// </summary>
        public int hoverSubtile = -1;
        /// <summary>
        /// Determines if this tile should have red outline based on question tile hover pointing to it, 
        /// -1 = off, 0 = 2 outlines horizontal, 1 = 3 outlines horizontal, 2 = 2 outlines vertical, 3 = 3 outlines vertical
        /// </summary>
        int extendedHover = -1;
        Pen extendedHoverPen = new Pen(Brushes.Red, 5);
        /// <summary>
        /// Contains the tile-space coordinates for the two subtiles
        /// </summary>
        Point[][] subtileTriangles;
        string[] hoverArrows = new string[2] { "►", "▼" };
        static Font arrowFont = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        Point[] arrowPositions;
        /// <summary>
        /// Keeps track of how many question tiles point over this tile, important when deleting
        /// </summary>
        int setTextCounter = 0;

        public Tile(int x, int y)
        {
            position = new Point(x, y);
        }

        public void ToEmptyTile(Tile[,] grid)
        {
            grid[position.Y, position.X] = new EmptyTile();
        }

        public Tile(int x, int y, int ts, Font font)
        {
            position = new Point(x, y);
            this.font = font;
            // Generate tile-space coordinates for the two subtiles and the arrowPositions
            subtileTriangles = new Point[2][] { new Point[3] { new Point(0, 0), new Point(ts, 0),  new Point(ts, ts) },
                                                new Point[3] { new Point(0, 0), new Point(ts, ts), new Point(0, ts) } };

            arrowPositions = new Point[] { new Point(ts / 3, 0), new Point(-3, 2 * (ts / 5)) };
        }

        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public Image GetGraphics(int ts)
        {
            // Dispose Image and Graphics to prevent memory leak
            Image canvas = new Bitmap(ts, ts);
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
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
                    graphics.DrawString(hoverArrows[hoverSubtile], arrowFont, Brushes.Red, arrowPositions[hoverSubtile]);
                }

                // Draw text
                if (GetText(out _))
                {
                    Size textSize = TextRenderer.MeasureText(text, font);
                    graphics.DrawString(text, font, ForegroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);
                }

                // Draw Rectangle
                // Conditions: has background color or text
                if (GetText(out _) || IsHighlighted())
                    graphics.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);

                // Draw extendedHover
                switch (extendedHover)
                {
                    case 0:
                        graphics.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                        graphics.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                        break;
                    case 1:
                        graphics.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                        graphics.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                        graphics.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                        break;
                    case 2:
                        graphics.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                        graphics.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                        break;
                    case 3:
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
            ForegroundColor = Brushes.Red;
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
