using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace MiniKreuzwortraetsel
{
    abstract class Tile
    {
        static public (string Question, string Answer) TupleToBeFilled;
        public enum ExtendedHover
        {
            Off = -1,
            Two_Outlines_Horizontal = 0,
            Three_Outlines_Horizontal = 1,
            Two_Outlines_Vertical = 2,
            Three_Outlines_Vertical = 3,
        }
        static public void RemoveAllExtendedHover(Tile[,] grid)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x].extendedHover = ExtendedHover.Off;
                }
            }
        }

        Point position;
        protected Font font = new Font("Verdana", 9.75f, FontStyle.Bold);
        protected Brush foregroundColor = Brushes.Blue;
        protected int oldHashCode = 0;
        /// <summary>
        /// Determines if this tile should have red outline based on question tile hover pointing to it, 
        /// -1 = off, 0 = 2 outlines horizontal, 1 = 3 outlines horizontal, 2 = 2 outlines vertical, 3 = 3 outlines vertical
        /// </summary>
        public ExtendedHover extendedHover = ExtendedHover.Off;
        protected Pen extendedHoverPen = new Pen(Brushes.Red, 5);

        public Tile(Point position)
        {
            this.position = position;
        }

        /// <summary>
        /// Draws all the visuals of this tile on an image and returns that image
        /// </summary>
        public abstract void Paint(int ts, Bitmap screenBuffer, PictureBox pb);

        public Point GetPosition()
        {
            return position;
        }
        public Point GetWorldPosition(int ts)
        {
            return new Point(position.X * ts, position.Y * ts);
        }
        protected void PaintToScreenBuffer(int ts, Bitmap screenBuffer, Bitmap tileBitmap, PictureBox pb)
        {
            // Draw the visuals into the screenBuffer
            using (Graphics screenBufferGfx = Graphics.FromImage(screenBuffer))
            {
                screenBufferGfx.DrawImage(tileBitmap, GetWorldPosition(ts));
                //screenBuffer.Save("screenBuffer.jpg");
                pb.Invalidate(new Rectangle(GetWorldPosition(ts), new Size(ts, ts)));
                pb.Update();
            }
        }
        /// <summary>
        /// This will be called when the mouse has moved, 
        /// the called method belongs to the tile instance that the mouse is on
        /// </summary>
        public abstract void MouseMove(MouseEventArgs e, PictureBox pb, int ts, Bitmap screenBuffer);
        /// <summary>
        /// This will be called when the mouse has moved from one tile to another,
        /// the called method belongs to the tile instance that the mouse was on before the movement
        /// </summary>
        public abstract void MouseLeave(MouseEventArgs e, PictureBox pb, int ts, Bitmap screenBuffer);
        public abstract void MouseClick(MouseEventArgs e, Tile[,] grid, int ts);
    }
}
