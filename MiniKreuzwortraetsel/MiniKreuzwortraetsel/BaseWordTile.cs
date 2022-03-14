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
    }
}
