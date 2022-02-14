using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class EmptyTile : Tile
    {
        public static void RemoveAllHighlights(Tile[,] grid, int ts, Bitmap screenBuffer, PictureBox pb)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] is EmptyTile)
                    {
                        EmptyTile emptyTile = grid[y, x] as EmptyTile;
                        // Reset the Subtiles so highlights disappear
                        emptyTile.MakeSubTiles();
                        // Call paint method of this tile
                        emptyTile.Paint(ts, screenBuffer, pb);
                    }
                }
            }
        }

        bool reserved = false;
        public SubTile[] SubTiles { get; } = new SubTile[2];

        public EmptyTile(Point position) : base(position)
        {
            MakeSubTiles();
        }

        void MakeSubTiles()
        {
            SubTiles[0] = new SubTile(direction: 0, parentTile: this);
            SubTiles[1] = new SubTile(direction: 1, parentTile: this);
        }
        public LetterTile ToLetterTile(Tile[,] grid, QuestionTile questionTile, string text, int ts, Bitmap screenBuffer, PictureBox pb)
        {
            grid[GetPosition().Y, GetPosition().X] = new LetterTile(GetPosition(), questionTile, text, ts, screenBuffer, pb);
            return grid[GetPosition().Y, GetPosition().X] as LetterTile;
        }
        public QuestionTile ToQuestionTile(Tile[,] grid, string question, int direction)
        {
            grid[GetPosition().Y, GetPosition().X] = new QuestionTile(GetPosition(), question, direction);
            return grid[GetPosition().Y, GetPosition().X] as QuestionTile;
        }
        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public override void Paint(int ts, Bitmap screenBuffer, PictureBox pb)
        {
            // Dispose Image and Graphics to prevent memory leak
            Bitmap tileBitmap = new Bitmap(ts, ts);
            using (Graphics graphics = Graphics.FromImage(tileBitmap))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                // Draw highlights
                for (int i = 0; i < SubTiles.Length; i++)
                {
                    graphics.FillPolygon(SubTiles[i].GetColor(), SubTiles[i].GetSubTilePolygon());
                }

                SubTile hoverSubTile = SubTile.BlueHoverSubTile;
                // Draw hover effect
                if (SubTile.BlueHoverSubTile?.ParentTile == this)
                {
                    graphics.FillPolygon(Brushes.Blue, hoverSubTile.GetSubTilePolygon());
                    graphics.DrawString(hoverSubTile.GetArrow(), SubTile.HOVER_ARROW_FONT, Brushes.Red, hoverSubTile.GetArrowPosition());
                }

                // Draw Rectangle
                // Condition: At least one subtile is highlighted
                if (SubTiles[0].IsHighlighted() || SubTiles[1].IsHighlighted())
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

                PaintToScreenBuffer(ts, screenBuffer, tileBitmap, pb);  
            }

        }

        public void Reserve()
        {
            reserved = true;
        }

        public void Unreserve()
        {
            reserved = false;
        }

        public bool IsReservedForQuestionTile()
        {
            return reserved;
        }
        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts, Bitmap screenBuffer)
        {

        }
        public override void MouseClick(MouseEventArgs e, Tile[,] grid, int ts)
        {

        }
        public override void MouseLeave(MouseEventArgs e, PictureBox pb, int ts, Bitmap screenBuffer)
        {
            
        }
        private bool CheckVisualChange()
        {
            if (oldHashCode != GetHashCode())
                return true;
            else
                return false;
            // Hash all visual properties in before state

            // Hash all visual properties in after state

            // Compare before to after state

            // In case of change, add this tile to refreshList
        }

    }
}
