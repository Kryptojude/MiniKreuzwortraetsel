﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    /// <summary>
    /// This saves all Bitmaps generated by tile.Paint(),
    /// it is only used to paint gridPB, when a refresh is triggered by the system,
    /// so that gridPB isn't erased
    /// </summary>
    static class ScreenBuffer
    {
        static Bitmap screenBuffer;

        static public void Initialize(int ts, int gridSize)
        {
            screenBuffer = new Bitmap(gridSize * ts, gridSize * ts);
        }
        static public Bitmap GetScreenBuffer()
        {
            return screenBuffer;
        }
        static public void DrawToScreenBuffer(Bitmap tileBitmap, Rectangle tileBounds)
        {
            using (Graphics gfx = Graphics.FromImage(screenBuffer))
            {
                gfx.DrawImage(tileBitmap, tileBounds);
            }
        }
    }
}