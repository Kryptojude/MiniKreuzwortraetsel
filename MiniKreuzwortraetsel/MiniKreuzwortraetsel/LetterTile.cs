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
        readonly List<QuestionOrBaseWordTile> parent_QuestionOrBaseWordTiles = new List<QuestionOrBaseWordTile>();
        public string Text = "";

        public LetterTile(Point position, QuestionOrBaseWordTile questionOrBaseWordTile, string text) : base(position)
        {
            questionOrBaseWordTile.AddLinkedLetterTile(this);
            Text = text;
        }

        public void ToEmptyTile(Tile[,] grid, QuestionOrBaseWordTile questionTileInterface)
        {
            // If the letterTile only belongs to this questionTile, then make into EmptyTile
            if (parent_QuestionOrBaseWordTiles.Count == 1)
                grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition());
            // If the letterTile belongs to multiple QuestionTiles, just remove this QuestionTile from its question tile list
            else
                parent_QuestionOrBaseWordTiles.Remove(questionTileInterface);
        }
        public override void Paint(Graphics g)
        {
            TranslateTransformGraphics(g, GetBounds().Location);
            int ts = Form1.TS;

            // Draw background
            g.FillRectangle(Brushes.White, 0, 0, ts, ts);
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

        public void AddParentQuestionOrBaseWordTile(QuestionOrBaseWordTile questionOrBaseWordTile)
        {
            parent_QuestionOrBaseWordTiles.Add(questionOrBaseWordTile);
        }

        public override void MouseMove(MouseEventArgs e, PictureBox pb, Point[] directions, Tile[,] grid)
        {
        }

        public override void MouseLeave(MouseEventArgs e, PictureBox pb)
        {

        }        
    }
}
