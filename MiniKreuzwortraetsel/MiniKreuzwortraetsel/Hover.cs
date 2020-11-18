using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniKreuzwortraetsel
{
    class Hover
    {
        static Point[][] HoverTriangles = new Point[][] { new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1) }, new Point[] { new Point(0, 0), new Point(1, 1), new Point(0, 1) } };
        static Point Position = new Point(-1, -1); // -1 means hover not active
        static int SubTile; // 0 means upper right subtile, 1 means lower left subtile

        public static bool GetHoverTriangle(out Point[] triangle, out string arrow, out Point arrowPos, int ts)
        {
            triangle = null;
            arrow = null;
            arrowPos = new Point();
            if (Position.X != -1)
            {
                triangle = new Point[] { new Point(), new Point(), new Point(), };
                for (int i = 0; i < HoverTriangles[SubTile].Length; i++)
                {
                    triangle[i].X = (Position.X + HoverTriangles[SubTile][i].X) * ts;
                    triangle[i].Y = (Position.Y + HoverTriangles[SubTile][i].Y) * ts;
                }

                if (SubTile == 0)
                {
                    arrow = "►";
                    arrowPos = new Point(triangle[0].X + ts / 3, triangle[0].Y);
                }
                else
                {
                    arrow = "▼";
                    arrowPos = new Point(triangle[0].X - 3, triangle[0].Y + 2 * (ts / 5));
                }

                return true;
            }
            else
                return false;
        }

        public static bool HasHoverChanged(Point mousePosition, Tile[,] grid, int ts)
        {
            Point newPosition = new Point(Position.X, Position.Y);
            int newSubTile = SubTile;
            // Check if mouse is within grid
            Point tilePos = new Point(mousePosition.X / ts, mousePosition.Y / ts);
            if (tilePos.X < grid.GetLength(1) && tilePos.Y < grid.GetLength(1))
            {
                // Check if mouse is over tile that is highlighted
                Tile tile = grid[tilePos.Y, tilePos.X];
                if (tile.IsHighlighted())
                {
                    // Save current tilePosition
                    newPosition = new Point(tilePos.X, tilePos.Y);
                    // Determine subTile
                    Point posRelativeToTile = new Point(mousePosition.X - ts * tile.GetPosition().X, mousePosition.Y - ts * tile.GetPosition().Y);
                    newSubTile = (posRelativeToTile.X > posRelativeToTile.Y) ? 0 : 1;
                    // Check if that subTile corresponds to the possible directions
                    Point[] directions = new Point[2] { new Point(1, 0), new Point(0, 1) };
                    Point directionPoint = directions[newSubTile];
                    bool foundDirection = false;
                    for (int i = 0; i < tile.HighlightDirections.Count; i++)
                    {
                        if (tile.HighlightDirections[i].X == directionPoint.X && tile.HighlightDirections[i].Y == directionPoint.Y)
                            foundDirection = true;
                    }

                    // If the tile doesn't support that direction indicated by the newSubTile, then deactivate Hover
                    if (!foundDirection)
                        newPosition = new Point(-1, -1);
                }
                // Mouse is not over highlighted tile
                else
                    newPosition = new Point(-1, -1);
            }
            // Mouse is not within grid
            else
                newPosition = new Point(-1, -1);

            // Compare new values with old ones
            if (newPosition.X != Position.X || newPosition.Y != Position.Y || newSubTile != SubTile)
            {
                // Highlight properties have changed
                // Update Highlight Properties
                Position = new Point(newPosition.X, newPosition.Y);
                SubTile = newSubTile;
                return true;
            }
            else
                // Highlight stayed the same
                return false;
        }

        public static void RemoveAllHighlights(Tile[,] grid)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x].SetBackgroundColor(Brushes.White);
                    grid[y, x].HighlightDirections.Clear();
                }
            }
        }
    }
}
