using System;
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
        readonly List<QuestionTile> parent_question_tiles = new List<QuestionTile>();
        public string Text = "";

        public LetterTile(Point position, QuestionTile questionTile, string text, int ts) : base(position, ts)
        {
            questionTile.AddLinkedLetterTile(this);
            Text = text;        }

        public void ToEmptyTile(Tile[,] grid, QuestionTile questionTile, int ts)
        {
            // If the letterTile only belongs to this questionTile, then make into EmptyTile
            if (parent_question_tiles.Count == 1)
                grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition(), ts);
            // If the letterTile belongs to multiple QuestionTiles, just remove this QuestionTile from its question tile list
            else
                parent_question_tiles.Remove(questionTile);
        }
        public override void Paint(Graphics g)
        {
            TranslateTransformGraphics(g, GetBounds().Location);
            int ts = Form1.TS;

            // Draw background
            g.FillRectangle(Brushes.White, GetBounds());
            // Draw text
            Size textSize = TextRenderer.MeasureText(Text, font);
            g.DrawString(Text, font, foregroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);

            // Draw Rectangle
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

            TranslateTransformGraphics(g, new Point(-GetBounds().Location.X, -GetBounds().Location.Y));
        }

        public void AddParentQuestionTile(QuestionTile questionTile)
        {
            parent_question_tiles.Add(questionTile);
        }

        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts, Point[] directions, Tile[,] grid)
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
