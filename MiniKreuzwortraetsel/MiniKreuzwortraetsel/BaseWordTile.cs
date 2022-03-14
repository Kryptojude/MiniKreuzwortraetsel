using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class BaseWordTile : QuestionOrBaseWordTile
    {

        public BaseWordTile(Point position, int direction) : base(position, direction)
        {
            Text = GetArrow(Direction);
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
        }
        public override void MouseLeave(MouseEventArgs e, PictureBox pb)
        {
            throw new NotImplementedException();
        }

        public override void MouseMove(MouseEventArgs e, PictureBox pb, Point[] directions, Tile[,] grid)
        {
            throw new NotImplementedException();
        }

        public override void Paint(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}
