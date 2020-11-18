using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class Highlight
    {
        static Point[][] HoverTriangles = new Point[][] { new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1) }, new Point[] { new Point(0, 0), new Point(1, 1), new Point(0, 1) } };
        static Point Position = new Point(-1, -1); // -1 means hover not active
        static int subTile; // 0 means upper right subtile, 1 means lower left subtile

        public static bool GetHoverTriangle(out Point[] triangle, int ts)
        {
            if (HoverEffect.Active)
            {
                triangle = new Point[] { new Point(), new Point(), new Point(), };
                for (int i = 0; i < HoverTriangles[HoverEffect.index].Length; i++)
                {
                    triangle[i].X = (HoverEffect.Position.X + HoverTriangles[HoverEffect.index][i].X) * ts;
                    triangle[i].Y = (HoverEffect.Position.Y + HoverTriangles[HoverEffect.index][i].Y) * ts;
                }
                return true;
            }
            else
            {
                triangle = null;
                return false;
            }

            // Hover Highlighting
            Point tilePos = new Point(e.X / ts, e.Y / ts);
            if (tilePos.X < grid.GetLength(1) && tilePos.Y < grid.GetLength(1))
            {
                Tile tile = grid[e.Y / ts, e.X / ts];
                if (tile.IsHighlighted())
                {
                    Point posRelativeToTile = new Point(e.X - ts * tile.GetPosition().X, e.Y - ts * tile.GetPosition().Y);
                    int index = (posRelativeToTile.X > posRelativeToTile.Y) ? 0 : 1;
                    // Check if HoverEffect needs to change
                    if (Tile.HoverEffect.index != index || Tile.HoverEffect.Position.X != tile.GetPosition().X || Tile.HoverEffect.Position.Y != tile.GetPosition().Y)
                    {
                        Tile.HoverEffect = (true, new Point(tile.GetPosition().X, tile.GetPosition().Y), index);
                        Refresh();
                    }
                }
                else
                {
                    if (Tile.HoverEffect.Active)
                    {
                        Tile.HoverEffect.Active = false;
                        Refresh();
                    }
                }
            }
        }

        public static bool HasHighlightChanged(Point mousePosition, Tile[,] grid, int ts)
        {
            // Check if mouse is within grid
            Point tilePos = new Point(mousePosition.X / ts, mousePosition.Y / ts);
            if (tilePos.X < grid.GetLength(1) && tilePos.Y < grid.GetLength(1))
            {
                // Check if mouse is over tile that is highlighted
                Tile tile = grid[tilePos.Y, tilePos.X];
                if (tile.IsHighlighted())
                {
                    // De
                    Point posRelativeToTile = new Point(mousePosition.X - ts * tile.GetPosition().X, mousePosition.Y - ts * tile.GetPosition().Y);
                    int index = (posRelativeToTile.X > posRelativeToTile.Y) ? 0 : 1;
                    // Check if HoverEffect needs to change
                    if (Tile.HoverEffect.index != index || Tile.HoverEffect.Position.X != tile.GetPosition().X || Tile.HoverEffect.Position.Y != tile.GetPosition().Y)
                    {
                        Tile.HoverEffect = (true, new Point(tile.GetPosition().X, tile.GetPosition().Y), index);
                        Refresh();
                    }
                }
                else
                {
                    if (Tile.HoverEffect.Active)
                    {
                        Tile.HoverEffect.Active = false;
                        Refresh();
                    }
                }
            }
        }

        public static void RemoveAllHighlights(Tile[,] grid)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x].SetBackgroundColor(Brushes.White);
                }
            }
        }
    }
}
