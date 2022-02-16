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
        static Rectangle Bounds;
        static Bitmap Bitmap;

        //static public Rectangle GetRectangle()
        //{
        //    return Bounds;
        //}
        static public bool Get(out Bitmap bitmap, out Rectangle rectangle)
        {
            // Returns
            bitmap = Bitmap;
            rectangle = Bounds;
            // Ready check
            if (!Bounds.IsEmpty && Bitmap != null)
            {
                // Reset
                Bounds = new Rectangle();
                Bitmap = null;
                return true;
            }
            else
                return false;
        }
        //static public Bitmap GetBitmap()
        //{
        //    return Bitmap;
        //}
        static public void Set(Rectangle bounds, Bitmap tileBitmap, PictureBox pb)
        {
            // Save these fields, they will be accessed by gridPB.Paint() to paint a single tile
            Bounds = bounds;
            Bitmap = tileBitmap;

            // Also write this tile bitmap into the screenBuffer in case system forces complete refresh
            ScreenBuffer.DrawToScreenBuffer(Bitmap, Bounds);

            // This should cause a repaint of only the invalidated area (Debug to verify)
            pb.Invalidate(Bounds);
            pb.Update();
        }
        //static public bool Ready()
        //{
        //    if (!Bounds.IsEmpty && Bitmap != null)
        //        return true;
        //    else
        //        return false;
        //}
        //static public void Reset()
        //{
        //    Bounds = new Rectangle();
        //    Bitmap = null;
        //}
    }
}
