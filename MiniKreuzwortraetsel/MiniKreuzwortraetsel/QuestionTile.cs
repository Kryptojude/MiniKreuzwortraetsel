using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class QuestionTile : Tile
    {
        static List<Tile> questionTileList = new List<Tile>();

        string question;
        List<LetterTile> letterTiles = new List<LetterTile>();
        Font font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        Brush foregroundColor = Brushes.Red;

        public QuestionTile(Point position) : base(position)
        {

        }

        public bool HasNumber()
        {
            return !string.IsNullOrEmpty(question);
        }

    }
}
