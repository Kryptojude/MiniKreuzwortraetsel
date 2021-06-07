using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniKreuzwortraetsel
{
    class QuestionTile : Tile
    {
        string question;
        List<LetterTile> letterTiles = new List<LetterTile>();

        public bool HasNumber()
        {
            return !string.IsNullOrEmpty(question);
        }

    }
}
