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
        /// <summary>
        /// The question tile(s) that this letter belongs to
        /// </summary>
        readonly List<QuestionTile> questionTiles = new List<QuestionTile>();
        public string Text = "";

        public LetterTile(Point position, QuestionTile questionTile, string text) : base(position)
        {
            questionTile.AddLinkedLetterTile(this);
            Text = text;
        }

        public void ToEmptyTile(Tile[,] grid, QuestionTile questionTile)
        {
            // If the letterTile only belongs to this questionTile, then make into EmptyTile
            if (questionTiles.Count == 1)
                grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition());
            // If the letterTile belongs to multiple QuestionTiles, just remove this QuestionTile from its question tile list
            else
                questionTiles.Remove(questionTile);
        }
        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public override Image GetImage(int ts)
        {
            // Dispose Image and Graphics to prevent memory leak
            Image canvas = new Bitmap(ts, ts);
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                // Draw text
                Size textSize = TextRenderer.MeasureText(Text, font);
                graphics.DrawString(Text, font, foregroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);

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

        public void AddQuestionTile(QuestionTile questionTile)
        {
            questionTiles.Add(questionTile);
        }
    }
}
