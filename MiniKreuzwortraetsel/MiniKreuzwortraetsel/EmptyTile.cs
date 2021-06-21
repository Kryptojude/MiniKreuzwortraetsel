using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class EmptyTile : Tile
    {
        bool reserved = false;
        SubTile[] subTiles = new SubTile[2];

        public EmptyTile(Point position) : base(position)
        {
            subTiles[0] = new SubTile("horizontal", this);
            subTiles[1] = new SubTile("vertical", this);
        }

        public void ToLetterTile(Tile[,] grid)
        {
            grid[GetPosition().Y, GetPosition().X] = new LetterTile(GetPosition());
        }

        public void ToQuestionTile(Tile[,] grid)
        {
            grid[GetPosition().Y, GetPosition().X] = new QuestionTile(GetPosition());
        }

        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public override Image GetGraphics(int ts)
        {
            // Dispose Image and Graphics to prevent memory leak
            Image canvas = new Bitmap(ts, ts);
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                // Draw highlights
                for (int i = 0; i < subTiles.Length; i++)
                {
                    graphics.FillPolygon(subTiles[i].GetColor(), subTiles[i].GetSubTilePolygon());
                }

                SubTile hoverSubTile = SubTile.GetHoverSubTile();
                // Draw hover effect
                if (SubTile.GetHoverSubTile().GetParentTile() == this)
                {
                    graphics.FillPolygon(SubTile.GetHoverBrush(), hoverSubTile.GetSubTilePolygon());
                    graphics.DrawString(hoverSubTile.GetHoverArrow(), new Font(,), Brushes.Red, hoverSubTile.GetArrowPosition(),);
                }

                // Draw Rectangle
                // Condition: At least one subtile is highlighted
                if (subTiles[0].IsHighlighted() || subTiles[1].IsHighlighted())
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
    }
}
