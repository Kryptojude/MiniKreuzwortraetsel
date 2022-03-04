﻿using System;
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
        public static void RemoveAllHighlights(Tile[,] grid, int ts, PictureBox pb)
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
                        emptyTile.Paint(Form1.myBuffer.Graphics);
                    }
                }
            }
        }

        bool reserved = false;
        public SubTile[] SubTiles { get; } = new SubTile[2];

        public EmptyTile(Point position, int ts) : base(position, ts)
        {
            MakeSubTiles();
        }

        void MakeSubTiles()
        {
            SubTiles[0] = new SubTile(direction: 0, parentTile: this);
            SubTiles[1] = new SubTile(direction: 1, parentTile: this);
        }
        public LetterTile ToLetterTile(Tile[,] grid, QuestionTile questionTile, string text, int ts, PictureBox pb)
        {
            grid[GetPosition().Y, GetPosition().X] = new LetterTile(GetPosition(), questionTile, text, ts);
            return grid[GetPosition().Y, GetPosition().X] as LetterTile;
        }
        public QuestionTile ToQuestionTile(Tile[,] grid, string question, int direction, int ts)
        {
            grid[GetPosition().Y, GetPosition().X] = new QuestionTile(GetPosition(), question, direction, ts);
            return grid[GetPosition().Y, GetPosition().X] as QuestionTile;
        }
        public override void Paint(Graphics g)
        {
            int ts = Form1.TS;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            // Draw highlights
            for (int i = 0; i < SubTiles.Length; i++)
            {
                g.FillPolygon(SubTiles[i].GetColor(), SubTiles[i].GetSubTilePolygon());
            }

            SubTile hoverSubTile = SubTile.BlueHoverSubTile;
            // Draw hover effect
            if (SubTile.BlueHoverSubTile?.ParentTile == this)
            {
                g.FillPolygon(Brushes.Blue, hoverSubTile.GetSubTilePolygon());
                g.DrawString(hoverSubTile.GetArrow(), SubTile.HOVER_ARROW_FONT, Brushes.Red, hoverSubTile.GetArrowPosition());
            }

            // Draw Rectangle
            // Condition: At least one subtile is highlighted
            if (SubTiles[0].IsHighlighted() || SubTiles[1].IsHighlighted())
                g.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);

            // Draw extendedHover
            switch (extendedHover)
            {
                case ExtendedHover.Two_Outlines_Horizontal:
                    g.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                    g.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                    break;
                case ExtendedHover.Three_Outlines_Horizontal:
                    g.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                    g.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                    g.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                    break;
                case ExtendedHover.Two_Outlines_Vertical:
                    g.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                    g.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                    break;
                case ExtendedHover.Three_Outlines_Vertical:
                    g.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                    g.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                    g.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                    break;
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
        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts) { }
        public override void MouseClick(MouseEventArgs e, Tile[,] grid, int ts) { }
        public override void MouseLeave(MouseEventArgs e, PictureBox pb, int ts) { }
    }
}
