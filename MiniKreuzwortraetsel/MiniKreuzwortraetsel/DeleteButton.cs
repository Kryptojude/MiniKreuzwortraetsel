using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class DeleteButton
    {
        bool visible = false;
        public Point Location = new Point(0,0);
        bool hover = false;
        /// <summary>
        /// Relative to ts (tilesize)
        /// </summary>
        public const float buttonSizeFactor = 0.3f;

        public bool IsVisible()
        {
            if (visible)
                return true;
            else
                return false;
        }

        public void SetVisible()
        {
            visible = true;
        }

        public void SetInvisible()
        {
            visible = false;
        }

        public void SetHover(bool hover)
        {
            this.hover = hover;
            if (hover)
                Cursor.Current = Cursors.Hand;
        }

        public bool GetHover()
        {
            return hover;
        }
    }
}
