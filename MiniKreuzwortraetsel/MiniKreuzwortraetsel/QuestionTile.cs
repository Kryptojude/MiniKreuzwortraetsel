using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class QuestionTile : QuestionOrBaseWordTile
    {
        static public readonly List<QuestionTile> QuestionTileList = new List<QuestionTile>();

        string Question;

        public QuestionTile(Point position, string question, int direction) : base(position, direction)
        {
            Question = question;
            QuestionTileList.Add(this);
            GenerateText();
        }

        public void GenerateText()
        {
            string arrow = GetArrow(Direction);
            Text = QuestionTileList.IndexOf(this) + 1 + arrow;
        }

        public string GetQuestion()
        {
            return Question;
        }

        public override void ToEmptyTile(Tile[,] grid)
        {
            base.ToEmptyTile(grid);

            // Save this index
            int indexOfThisQuestionTile = QuestionTileList.IndexOf(this);
            // Remove this instance from the questionTileList,
            QuestionTileList.Remove(this);
            // Now indexOfThisQuestionTile points to the next questionTile
            // Lower the number of every questionTile that comes after this one
            for (int i = indexOfThisQuestionTile; i < QuestionTileList.Count; i++)
            {
                QuestionTileList[i].GenerateText();
            }
        }
    }
}
