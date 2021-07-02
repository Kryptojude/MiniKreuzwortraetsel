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
        static readonly Pen pen;
        const float sizeFactor = 0.3f;
        static readonly int absoluteSize;
        public static readonly Rectangle bounds_tile_space;
        static DeleteButton lastVisibleDeleteButton;
        public static void SetInvisible()
        {
            lastVisibleDeleteButton.visible = false;
            lastVisibleDeleteButton = null;
        }

        bool visible = false;
        bool hover = false;

        static DeleteButton()
        {
            pen = new Pen(Brushes.Red, 1.7f);

            absoluteSize = (int)(sizeFactor * Form1.TS);
            bounds_tile_space = new Rectangle(Form1.TS - absoluteSize, 0, absoluteSize, absoluteSize);
        }

        public bool IsVisible()
        {
            return visible;
        }

        public void SetVisible(out bool needs_refresh)
        {
            // Check if there has been a change
            if (this == lastVisibleDeleteButton)
                needs_refresh = true;
            else
            {
                visible = true;
                lastVisibleDeleteButton = this;
                needs_refresh = false;
            }
        }

        void SetHover(bool hover)
        {
            this.hover = hover;
            if (hover)
                Cursor.Current = Cursors.Hand;
            else
                Cursor.Current = Cursors.Default;
        }

        public bool IsMouseOverMe(MouseEventArgs e, QuestionTile parentTile)
        {
            // Calculate mouse position in tile space
            Point mousePosition_tile_space = new Point(e.X - parentTile.GetWorldPosition(Form1.TS).X, e.Y - parentTile.GetWorldPosition(Form1.TS).Y);
            // Check if mouse is over deleteButton
            if (bounds_tile_space.X <= mousePosition_tile_space.Y && bounds_tile_space.Height >= mousePosition_tile_space.Y)
                return true;
            else
                return false;
        }

        // Checks if mouse is hovering over deleteButton and makes hover visible
        public void MouseMove(MouseEventArgs e, QuestionTile parentTile)
        {
            if (IsMouseOverMe(e, parentTile))
                SetHover(true);
            else
                SetHover(false);
        }

        public Image GetImage()
        {
            // Dispose Image and Graphics to prevent memory leak
            Image canvas = new Bitmap(absoluteSize, absoluteSize);
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                if (IsVisible())
                {
                    graphics.DrawRectangle(pen, Form1.TS - absoluteSize, 0, absoluteSize, absoluteSize);
                    graphics.DrawLine(pen, Form1.TS - absoluteSize, 0, Form1.TS, absoluteSize);
                    graphics.DrawLine(pen, Form1.TS - absoluteSize, absoluteSize, Form1.TS, 0);
                }

                return canvas;
            }
        }
    }
}
