using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class Popup
    {
        bool visible;
        string text;
        Point position;

        public Popup()
        {
            visible = false;
            text = "";
            position = new Point();
        }

        public bool IsVisible()
        {
            return visible;
        }

        public void Show()
        {
            visible = true;
        }

        public void Hide()
        {
            visible = false;
        }

        public string GetText()
        {
            return text;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public Point GetPosition()
        {
            return position;
        }

        public void SetPosition(Point position)
        {
            this.position = position;
        }
    }
}
