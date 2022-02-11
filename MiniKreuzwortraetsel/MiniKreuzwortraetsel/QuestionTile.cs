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
        readonly List<LetterTile> LinkedLetterTiles = new List<LetterTile>();
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
                questionTileList.Add(this);

            GenerateText();
        }

        private void GenerateText()
        {
            // normal question tile
            string arrow = SubTile.GetArrow(Direction);
            if (!string.IsNullOrEmpty(Question))
            {
                Text = questionTileList.IndexOf(this) + 1 + arrow;
            }
            // base word
            else
                Text = arrow;
        }

        public bool IsBaseWord()
        {
            return string.IsNullOrEmpty(Question);
        }

        public void ToEmptyTile(Tile[,] grid)
        {
            // Turn the letter tiles associated with this questionTile into emptyTiles
            foreach (LetterTile letterTile in LinkedLetterTiles)
                letterTile.ToEmptyTile(grid, this);

            // Unreserve the reserved tile of the questionTile
            if (LinkedReservedTile != null)
                LinkedReservedTile.Unreserve();

            // Insert a new EmptyTile instance into the grid at this tile's position, 
            grid[GetPosition().Y, GetPosition().X] = new EmptyTile(GetPosition());

            // Save this index
            int indexOfThisQuestionTile = questionTileList.IndexOf(this);
            // Remove this instance from the questionTileList,
            questionTileList.Remove(this);
            // Now indexOfThisQuestionTile points to the next questionTile
            // Lower the number of every questionTile that comes after this one
            for (int i = indexOfThisQuestionTile; i < questionTileList.Count; i++)
                questionTileList[i].GenerateText();
        }

        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public override void Paint(int ts, Bitmap screenBuffer)
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

                // Draw X
                using (Image deleteButtonImage = deleteButton.GetImage())
                    graphics.DrawImage(deleteButtonImage, DeleteButton.bounds_tile_space);

                PaintToScreenBuffer(ts, screenBuffer, tileBitmap);
            }
        }

        public void AddLinkedLetterTile(LetterTile letterTile)
        {
            LinkedLetterTiles.Add(letterTile);
            letterTile.AddQuestionTile(this);
        }

        public void MouseMove(MouseEventArgs e, out bool needs_refresh, PictureBox pb)
        {
            // DeleteButton is visible when hovering over a question tile
            deleteButton.SetVisible(out needs_refresh);

            deleteButton.MouseMove(e, this, pb);
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

        private void CheckVisualChange(int ts, Bitmap screenBuffer)
        {
            int newHashCode = GetHashCode();
            if (oldHashCode != newHashCode)
            {
                // Save old Hash code
                oldHashCode = newHashCode;
                // Call my paint function
                Paint(ts, screenBuffer);
            }

            // Hash all visual properties in before state

            // Hash all visual properties in after state

            // Compare before to after state

            // In case of change, add this tile to refreshList
        }
    }
}
