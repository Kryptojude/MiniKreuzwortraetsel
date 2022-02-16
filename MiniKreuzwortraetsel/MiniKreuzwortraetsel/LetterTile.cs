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

        public LetterTile(Point position, QuestionTile questionTile, string text, int ts, PictureBox pb) : base(position, ts)
        {
            questionTile.AddLinkedLetterTile(this);
            Text = text;
            Paint(ts, pb);
        }

        public void ToEmptyTile(Tile[,] grid, QuestionTile questionTile, int ts)
        {
            // If the letterTile only belongs to this questionTile, then make into EmptyTile
            if (questionTiles.Count == 1)
                grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition(), ts);
            // If the letterTile belongs to multiple QuestionTiles, just remove this QuestionTile from its question tile list
            else
                questionTiles.Remove(questionTile);
        }
        public override void Paint(int ts, PictureBox pb)
        {
            // Dispose Image and Graphics to prevent memory leak
            Bitmap tileBitmap = new Bitmap(ts, ts);
            using (Graphics graphics = Graphics.FromImage(tileBitmap))
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

                NextPaintInstruction.Set(GetBounds(), tileBitmap, pb);
            }
        }

        public void AddQuestionTile(QuestionTile questionTile)
        {
            questionTiles.Add(questionTile);
        }

        private void CheckVisualChange(int ts, Bitmap screenBuffer, PictureBox pb)
        {
            int newHashCode = GetHashCode();
            if (oldHashCode != newHashCode)
            {
                // Save old Hash code
                oldHashCode = newHashCode;
                // Call my paint function
                Paint(ts, pb);
            }

            // Hash all visual properties in before state

            // Hash all visual properties in after state

            // Compare before to after state

            // In case of change, add this tile to refreshList
        }

        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts)
        {
        }

        public override void MouseLeave(MouseEventArgs e, PictureBox pb, int ts)
        {

        }        
        public override void MouseClick(MouseEventArgs e, Tile[,] grid, int ts)
        {

        }

    }
}
