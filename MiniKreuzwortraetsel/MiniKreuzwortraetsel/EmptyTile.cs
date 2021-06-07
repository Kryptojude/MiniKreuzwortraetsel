using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniKreuzwortraetsel
{
    class EmptyTile : Tile
    {
        bool reserved = false;
        SubTile[] subTiles = new SubTile[2];

        public EmptyTile(int x, int y) : base(x, y)
        {
            subTiles[0] = new SubTile();
            subTiles[1] = new SubTile();
        }
    }
}
