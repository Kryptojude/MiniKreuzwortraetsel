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
        static protected List<Tile> tiles_with_extended_hover_list = new List<Tile>();
        public enum ExtendedHover
        {
            Off = -1,
            Two_Outlines_Horizontal = 0,
            Three_Outlines_Horizontal = 1,
            Two_Outlines_Vertical = 2,
            Three_Outlines_Vertical = 3,
        }
        static public void RemoveAllExtendedHover()
        {
            for (int i = 0; i < tiles_with_extended_hover_list.Count; i++)
            {
                tiles_with_extended_hover_list[i].extendedHover = ExtendedHover.Off;
            }
            tiles_with_extended_hover_list.Clear();
        }

        public interface ILikeQuestionTile
        {
            int GetDirection();
        }

        static readonly Dictionary<string, string> Arrows = new Dictionary<string, string>() {
                                                                    { "horizontal", "►" },
                                                                    { "vertical", "▼" } };
        static public string GetArrow(string direction)
        {
            return Arrows[direction];
        }
        static public string GetArrow(int direction)
        {
            return Arrows.ElementAt(direction).Value;
        }

        Point Position;
        Rectangle Bounds;
        protected Font font = new Font("Verdana", 9.75f, FontStyle.Bold);
        protected Brush foregroundColor = Brushes.Blue;
        /// <summary>
        /// Determines if this tile should have red outline based on question tile hover pointing to it, 
        /// -1 = off, 0 = 2 outlines horizontal, 1 = 3 outlines horizontal, 2 = 2 outlines vertical, 3 = 3 outlines vertical
        /// </summary>
        public ExtendedHover extendedHover = ExtendedHover.Off;
        protected Pen extendedHoverPen = new Pen(Brushes.Red, 5);

        public Tile(Point position)
        {
            int ts = Form1.TS;
            Position = position;
            Bounds = new Rectangle(Position.X * ts, Position.Y * ts, ts, ts);
        }
        public Rectangle GetBounds()
        {
            return Bounds;
        }
        /// <summary>
        /// Refers to the position in grid[,] array
        /// </summary>
        public Point GetPosition()
        {
            return Position;
        }
        /// <summary>
        /// This will be called when the mouse has moved, 
        /// the called method belongs to the tile instance that the mouse is on after the movement
        /// </summary>
        public abstract void MouseMove(MouseEventArgs e, PictureBox pb, Point[] directions, Tile[,] grid);
        /// <summary>
        /// This will be called when the mouse has moved from one tile to another,
        /// the called method belongs to the tile instance that the mouse was on before the movement
        /// </summary>
        public abstract void MouseLeave(MouseEventArgs e, PictureBox pb);
        public abstract void MouseClick(MouseEventArgs e, Tile[,] grid);
        public abstract void Paint(Graphics g);
        /// <summary>
        /// Moves the origin of the grid
        /// </summary>
        static public void TranslateTransformGraphics(Graphics g, Point location)
        {
            g.TranslateTransform(location.X, location.Y);
        }
    }
}
