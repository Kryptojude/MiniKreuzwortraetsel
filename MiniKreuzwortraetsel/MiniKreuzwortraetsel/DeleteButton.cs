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
            if (lastVisibleDeleteButton != null)
            {
                lastVisibleDeleteButton.visible = false;
                lastVisibleDeleteButton = null;
            }
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
            // Same deleteButton, no change
            if (this == lastVisibleDeleteButton)
                needs_refresh = false;
            // null to this
            else if (lastVisibleDeleteButton == null)
            {
                lastVisibleDeleteButton = this;
                needs_refresh = true;
            }
            // other deleteButton to this
            else if (lastVisibleDeleteButton != null && this != lastVisibleDeleteButton)
            {
                lastVisibleDeleteButton.visible = false;
                lastVisibleDeleteButton = this;
                needs_refresh = true;
            }
            else
            {
                throw new Exception("");
            }

            visible = true;
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
            if (bounds_tile_space.X <= mousePosition_tile_space.X && bounds_tile_space.Height >= mousePosition_tile_space.Y)
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
                    graphics.DrawRectangle(pen, 0, 0, absoluteSize, absoluteSize - 1);
                    graphics.DrawLine(pen, 0, 0, absoluteSize, absoluteSize - 1);
                    graphics.DrawLine(pen, 0, absoluteSize - 1, absoluteSize, 0);

                    //graphics.FillRectangle(Brushes.Black, 0, 0, absoluteSize, absoluteSize);
                }

                return canvas;
            }
        }
    }
}
