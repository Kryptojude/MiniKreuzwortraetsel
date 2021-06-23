﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class QuestionTile : Tile
    {
        static List<Tile> questionTileList = new List<Tile>();
        static public List<Tile> GetQuestionTileList()
        {
            return questionTileList;
        }

        string question;
        public string Text { get; set; } = "";
        public List<LetterTile> linkedLetterTiles { get; } = new List<LetterTile>();

        public QuestionTile(Point position) : base(position)
        {
            foregroundColor = Brushes.Red;
            font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        }

        public bool HasNumber()
        {
            return !string.IsNullOrEmpty(question);
        }

        public void ToEmptyTile(Tile[,] grid)
        {
            grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition());
            questionTileList.Remove(this);
        }

        public void SetQuestion(string question)
        {
            this.question = question;
        }
        public string GetQuestion()
        {
            return question;
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
                Size textSize = TextRenderer.MeasureText(Text, font);
                graphics.DrawString(Text, font, foregroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);

                // Draw Rectangle
                graphics.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);
                
                return canvas;
            }
        }
    }
}
