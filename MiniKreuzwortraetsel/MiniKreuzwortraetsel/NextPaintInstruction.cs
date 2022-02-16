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
        // Next time: Too many Paint event calls are slowing down the program, so we need to implement a refresh list in this class,
        // where the Get() method always returns the next Paint instruction from that list and deletes it
        // Get() will be called like now by gridPB_Paint() and loops through the refresh list until its empty.
        // The invalidate/update is necessary cause only certain tiles will be drawn, so we dont lose the rest which would be the case with refresh().
        // The invalidate/update call cant be in here, we dont want every tile paint to cause gridPB_Paint() to be called, instead the pair will be called
        // in the places it was when refresh() was still used, so whenever a big operation is complete, like autofilling a word into the grid, at the very end there should be one call
        static public void Set(Rectangle bounds, Bitmap tileBitmap, PictureBox pb)
        {
            // Save these fields, they will be accessed by gridPB_Paint() to paint a single tile
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
