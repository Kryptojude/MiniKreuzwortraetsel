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
        public BaseWordTile(Point position, int ts) : base(position, ts)
        {
        }

        public override void MouseClick(MouseEventArgs e, Tile[,] grid, int ts)
        {
            throw new NotImplementedException();
        }

        public override void MouseLeave(MouseEventArgs e, PictureBox pb, int ts)
        {
            throw new NotImplementedException();
        }

        public override void MouseMove(MouseEventArgs e, PictureBox pb, int ts, Point[] directions, Tile[,] grid)
        {
            throw new NotImplementedException();
        }

        public override void Paint(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}
