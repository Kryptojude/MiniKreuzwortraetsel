using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class LetterTile : Tile
    {
        List<QuestionTile> questionTiles = new List<QuestionTile>();

        public LetterTile(Point position) : base(position)
        {

        }
    }
}
