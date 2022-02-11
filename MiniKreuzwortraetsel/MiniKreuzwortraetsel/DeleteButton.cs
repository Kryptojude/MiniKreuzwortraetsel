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
        public static void SetInvisible(PictureBox pb, out bool needs_refresh)
        {
            if (lastVisibleDeleteButton != null)
            {
                // A deleteButton instance is currently visible, so turn it invisible and save refresh
                lastVisibleDeleteButton.visible = false;
                lastVisibleDeleteButton = null;
                pb.Cursor = Cursors.Default;
                needs_refresh = true;
            }
            else
            {
                // No deleteButton instance was visible, so don't do anything, no refresh
                needs_refresh = false;
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
                visible = true;
            }
            // other deleteButton to this
            else if (lastVisibleDeleteButton != null && this != lastVisibleDeleteButton)
            {
                lastVisibleDeleteButton.visible = false;
                lastVisibleDeleteButton = this;
                visible = true;
                needs_refresh = true;
            }
            else
            {
                throw new Exception("impossible branch reached in SetVisible()");
            }
        }

        void SetHover(bool hover, PictureBox pb)
        {
            this.hover = hover;
            if (hover)
                pb.Cursor = Cursors.Hand;
            else
                pb.Cursor = Cursors.Default;
        }

        public bool IsMouseOverMe(MouseEventArgs e, QuestionTile parentTile, int ts)
        {
            // Calculate mouse position in tile space
            Point mousePosition_tile_space = new Point(e.X - parentTile.GetWorldPosition(ts).X, e.Y - parentTile.GetWorldPosition(ts).Y);
            // Check if mouse is over deleteButton
            if (bounds_tile_space.X <= mousePosition_tile_space.X && bounds_tile_space.Height >= mousePosition_tile_space.Y)
                return true;
            else
                return false;
        }

        // Checks if mouse is hovering over deleteButton and makes hover visible
        public void MouseMove(MouseEventArgs e, QuestionTile parentTile, PictureBox pb, int ts)
        {
            if (IsMouseOverMe(e, parentTile, ts))
                SetHover(true, pb);
            else
                SetHover(false, pb);
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
