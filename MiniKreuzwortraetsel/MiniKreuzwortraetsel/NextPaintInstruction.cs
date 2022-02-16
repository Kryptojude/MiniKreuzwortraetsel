using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    static class NextPaintInstruction
    {
        static Rectangle Rectangle;
        static Bitmap Bitmap;

        static public Rectangle GetRectangle()
        {
            return Rectangle;
        }
        static public Bitmap GetBitmap()
        {
            return Bitmap;
        }
        static public void Set(Rectangle rectangle, Bitmap bitmap, PictureBox pb)
        {
            Rectangle = rectangle;
            Bitmap = bitmap;

            // This should cause a repaint of only the invalidated area (Debug to verify)
            pb.Invalidate(Rectangle);
            //pb.Update();
        }
    }
}
