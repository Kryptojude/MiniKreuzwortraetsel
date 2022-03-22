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
            BeginPaint(g);

            Rectangle LocalBounds = GetLocalBounds();
            // Draw text
            Size textSize = TextRenderer.MeasureText(Text, font);
            g.DrawString(Text, font, foregroundColor, Form1.TS / 2 - textSize.Width / 2, Form1.TS / 2 - textSize.Height / 2);
            g.DrawRectangle(Pens.Black, 0, 0, LocalBounds.Width - 1, LocalBounds.Height - 1);

            DrawExtendedHover(g);

            EndPaint(g);
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
