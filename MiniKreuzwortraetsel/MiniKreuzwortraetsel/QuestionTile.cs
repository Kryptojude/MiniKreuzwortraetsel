using System;
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
        static public readonly List<QuestionTile> questionTileList = new List<QuestionTile>();

        public string Question;
        public string Text = "";
        public int Direction;
        public readonly List<LetterTile> LinkedLetterTiles = new List<LetterTile>();

        public QuestionTile(Point position, string question, int direction) : base(position)
        {
            foregroundColor = Brushes.Red;
            font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Question = question;
            Direction = direction;
            // normal question tile
            string arrow = SubTile.GetArrow(direction);
            if (!string.IsNullOrEmpty(Question))
            {
                questionTileList.Add(this);
                Text = questionTileList.Count + arrow;
            }
            // base word
            else
                Text = arrow;
        }

        public bool HasNumber()
        {
            return !string.IsNullOrEmpty(Question);
        }

        public void ToEmptyTile(Tile[,] grid)
        {
            grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition());
            questionTileList.Remove(this);
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
