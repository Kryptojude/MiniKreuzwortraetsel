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
        static public readonly List<QuestionTile> QuestionTileList = new List<QuestionTile>();

        public string Question;
        public string Text = "";
        public int Direction;
        readonly List<LetterTile> LinkedLetterTiles = new List<LetterTile>();
        public EmptyTile LinkedReservedTile;
        DeleteButton deleteButton;

        public QuestionTile(Point position, string question, int direction, int ts) : base(position, ts)
        {
            deleteButton = new DeleteButton(GetBounds().Location);
            foregroundColor = Brushes.Red;
            font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Question = question;
            Direction = direction;
            // normal question tile
            string arrow = SubTile.GetArrow(direction);
            if (!string.IsNullOrEmpty(Question))
                QuestionTileList.Add(this);

            GenerateText();
        }

        private void GenerateText()
        {
            // normal question tile
            string arrow = SubTile.GetArrow(Direction);
            if (!string.IsNullOrEmpty(Question))
            {
                Text = QuestionTileList.IndexOf(this) + 1 + arrow;
            }
            // base word
            else
                Text = arrow;
        }

        public bool IsBaseWord()
        {
            return string.IsNullOrEmpty(Question);
        }

        public void ToEmptyTile(Tile[,] grid, int ts)
        {
            // Turn the letter tiles associated with this questionTile into emptyTiles
            foreach (LetterTile letterTile in LinkedLetterTiles)
                letterTile.ToEmptyTile(grid, this, ts);

            // Unreserve the reserved tile of the questionTile
            LinkedReservedTile?.Unreserve();

            // Insert a new EmptyTile instance into the grid at this tile's position, 
            Point position = GetPosition();
            grid[position.Y, position.X] = new EmptyTile(position, ts);

            // Save this index
            int indexOfThisQuestionTile = QuestionTileList.IndexOf(this);
            // Remove this instance from the questionTileList,
            QuestionTileList.Remove(this);
            // Now indexOfThisQuestionTile points to the next questionTile
            // Lower the number of every questionTile that comes after this one
            for (int i = indexOfThisQuestionTile; i < QuestionTileList.Count; i++)
                QuestionTileList[i].GenerateText();
        }

        public override void Paint(Graphics g)
        {
            TranslateTransformGraphics(g, GetBounds().Location);
            int ts = Form1.TS;

            // Draw background
            g.FillRectangle(Brushes.White, 0, 0, ts, ts);
            // Draw text
            Size textSize = TextRenderer.MeasureText(Text, font);
            //g.DrawString(Text, font, foregroundColor, GetBounds().Location.X + (ts / 2 - textSize.Width / 2), GetBounds().Location.Y + (ts / 2 - textSize.Height / 2));
            g.DrawString(Text, font, foregroundColor, ts / 2 - textSize.Width / 2, ts / 2 - textSize.Height / 2);

            // Draw Rectangle
            g.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);

            TranslateTransformGraphics(g, new Point(-GetBounds().Location.X, -GetBounds().Location.Y));

            // Draw X
            deleteButton.Paint(g);
        }

        public void AddLinkedLetterTile(LetterTile letterTile)
        {
            LinkedLetterTiles.Add(letterTile);
            letterTile.AddParentQuestionTile(this);
        }
        // What I did last time: Painting logic is pretty much done now, now fix why tiles aren't showing up properly after adding them
        // CheckVisualChange() method probably instead override GetHashCode() and in that go through all the fields that are relevant for visuals (even child elements)
        // and hash it before and after
        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts, Point[] directions, Tile[,] grid)
        {
            /* What can happen when you move the mouse onto a questionTile, or within a questionTile?
             * deleteButton could appear, it can't disappear
             * cursor could become hand or arrow (not currently being hashed) but doesnt need refresh anyway
             * 
             * */

            // Calculate the fields of this instance based on mouse position (this may or may not result in any changes to this object)
            // Is mouse hovering over this tile? (This check will become redundant when MouseLeave is implemented
            // Set deleteButton to visible
            deleteButton.SetVisible(true);
            // Is mouse hovering over deleteButton?
            if (deleteButton.IsMouseOverMe(e, this, ts))
                // Call delete button hover logic
                deleteButton.SetHover(true, pb);
            else
                // Mouse is not over deleteButton, 
                // so undo deleteButton hover
                deleteButton.SetHover(false, pb);
        }

        public override void MouseLeave(MouseEventArgs e, PictureBox pb, int ts)
        {
            // deleteButton is not visible
            deleteButton.SetVisible(false);
            deleteButton.SetHover(false, pb);
        }
        public override void MouseClick(MouseEventArgs e, Tile[,] grid, int ts)
        {
            // If the click was on the deleteButton
            if (deleteButton.IsMouseOverMe(e, this, ts))
            {
                // Then delete this question
                ToEmptyTile(grid, ts);
            }
        }

    }
}
