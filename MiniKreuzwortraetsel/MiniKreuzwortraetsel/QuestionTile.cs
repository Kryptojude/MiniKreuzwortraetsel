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
            LinkedReservedTile?.Unreserve();

            // Insert a new EmptyTile instance into the grid at this tile's position, 
            Point position = GetPosition();
            grid[position.Y, position.X] = new EmptyTile(position);

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

        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts, Bitmap screenBuffer)
        {
            /* What can happen when you move the mouse away from a questionTile, onto a questionTile, or within a questionTile?
             * deleteButton could appear, it can't disappear
             * cursor could become hand or arrow (not currently being hashed) but doesnt need refresh anyway
             * 
             * */

            // GetHashCode for before-state
            int beforeHashCode = GetHashCode();

            // Calculate the fields of this instance based on mouse position (this may or may not result in any changes to this object)
            // Is mouse hovering over this tile? (This check will become redundant when MouseLeave is implemented
            if (GetWorldPosition(ts).X <= e.X && GetWorldPosition(ts).X + ts > e.X && GetWorldPosition(ts).Y <= e.Y && GetWorldPosition(ts).Y + ts > e.Y)
            {
                // Set deleteButton to visible
                deleteButton.SetVisible(true);
                // Is mouse hovering over deleteButton?
                if (deleteButton.IsMouseOverMe(e, this, ts))
                {
                    // Hand down event
                    deleteButton.MouseMove(e, this, pb, ts);
                }
            }
            else
            {
                // Mouse is not hovering over this tile, that means that the mouse left this tile
                // What can change based on this?
                // deleteButton is not visible
                deleteButton.SetVisible(false);
                // Cursor is arrow
                pb.Cursor = Cursors.Arrow;

            }


            // GetHashCode for after-state (this may take into account fields that are not relevant to the visual appearance of this tile, may also not take into account changes to objects that are referenced in fields)
            int afterHashCode = GetHashCode();

            // Compare before and after-state, If change occured, call this.Paint();
            if (beforeHashCode != afterHashCode)
                Paint(ts, screenBuffer);


        }

        public override void MouseClick(MouseEventArgs e, Tile[,] grid, int ts)
        {
            // If the click was on the deleteButton
            if (deleteButton.IsMouseOverMe(e, this, ts))
            {
                // Then delete this question
                ToEmptyTile(grid);
            }
        }
    }
}
