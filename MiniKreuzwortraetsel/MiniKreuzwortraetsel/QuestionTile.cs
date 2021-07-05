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
        public EmptyTile LinkedReservedTile;
        DeleteButton deleteButton = new DeleteButton();

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
            // Turn the letter tiles associated with this questionTile into emptyTiles
            foreach (LetterTile letterTile in LinkedLetterTiles)
            {
                List<QuestionTile> letterTile_QuestionTileList = letterTile.GetQuestionTiles();
                // If the letterTile only belongs to this questionTile, then make into EmptyTile
                if (letterTile_QuestionTileList.Count == 1)
                    letterTile.ToEmptyTile(grid);
                // If the letterTile belongs to multiple QuestionTiles, just remove this QuestionTile from its question tile list
                else
                    letterTile_QuestionTileList.Remove(this);
            }

            // Unreserve the reserved tile of the questionTile
            if (LinkedReservedTile != null)
                LinkedReservedTile.Reserved = false;

            // Insert a new EmptyTile instance into the grid at this tile's position, 
            grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition());
            
            // Remove this instance from the questionTileList,
            questionTileList.Remove(this);
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

                // Draw X
                using (Image deleteButtonImage = deleteButton.GetImage())
                    graphics.DrawImage(deleteButtonImage, DeleteButton.bounds_tile_space);

                return canvas;
            }
        }

        public void MouseMove(MouseEventArgs e, out bool needs_refresh)
        {
            // DeleteButton is visible when hovering over a question tile
            deleteButton.SetVisible(out needs_refresh);

            deleteButton.MouseMove(e, this);
        }

        public void MouseClick(MouseEventArgs e, Tile[,] grid)
        {
            // If the click was on the deleteButton
            if (deleteButton.IsMouseOverMe(e, this))
            {
                // Then delete this question
                ToEmptyTile(grid);
            }
        }
    }
}
