using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    abstract class QuestionOrBaseWordTile : Tile
    {
        protected string Text;
        protected int Direction;
        protected readonly List<LetterTile> LinkedLetterTiles;
        protected EmptyTile LinkedReservedTile;
        protected DeleteButton deleteButton;

        public QuestionOrBaseWordTile(Point position, int direction) : base(position)
        {
            Direction = direction;
            LinkedLetterTiles = new List<LetterTile>();
            deleteButton = new DeleteButton(GetBounds().Location);

            foregroundColor = Brushes.Red;
            font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
        }

        public string GetText()
        {
            return Text;
        }
        public void SetLinkedReservedTile(EmptyTile linkedReservedTile)
        {
            LinkedReservedTile = linkedReservedTile;
        }
        public int GetDirection()
        {
            return Direction;
        }

        public void AddLinkedLetterTile(LetterTile letterTile)
        {
            LinkedLetterTiles.Add(letterTile);
            letterTile.AddParentQuestionOrBaseWordTile(this);
        }
    }
}
