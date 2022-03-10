using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class BaseWordTile : Tile
    {
        string Text;
        public int Direction;
        readonly List<LetterTile> LinkedLetterTiles = new List<LetterTile>();
        public EmptyTile LinkedReservedTile;
        DeleteButton deleteButton;

        public BaseWordTile(Point position, int direction) : base(position)
        {
            deleteButton = new DeleteButton(GetBounds().Location);
            foregroundColor = Brushes.Red;
            font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Direction = direction;
            Text = GetArrow(Direction);
        }

        public string GetText()
        {
            return Text;
        }

        public override void MouseClick(MouseEventArgs e, Tile[,] grid)
        {
            throw new NotImplementedException();
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
