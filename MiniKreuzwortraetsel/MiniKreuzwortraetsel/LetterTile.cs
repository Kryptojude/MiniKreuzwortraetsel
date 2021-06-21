﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class LetterTile : Tile
    {
        List<QuestionTile> questionTiles = new List<QuestionTile>();

        public LetterTile(Point position) : base(position)
        {

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

                // Draw text
                if (GetText(out _))
                {
                    Size textSize = TextRenderer.MeasureText(text, font);
                    graphics.DrawString(text, font, foregroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);
                }

                // Draw Rectangle
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