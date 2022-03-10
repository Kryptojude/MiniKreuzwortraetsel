using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MiniKreuzwortraetsel
{
    class EmptyTile : Tile
    {
        static public readonly List<EmptyTile> EmptyTileList = new List<EmptyTile>();

        public static void RemoveAllHighlights()
        {
            for (int i = 0; i < EmptyTileList.Count; i++)
            {
                EmptyTile emptyTile = EmptyTileList[i];
                // Reset the Subtiles so highlights disappear
                emptyTile.MakeSubTiles();
            }
        }

        bool reserved = false;
        public SubTile[] SubTiles { get; } = new SubTile[2];

        public EmptyTile(Point position) : base(position)
        {
            EmptyTileList.Add(this);
            MakeSubTiles();
        }

        void MakeSubTiles()
        {
            SubTiles[0] = new SubTile(direction: 0, parentTile: this);
            SubTiles[1] = new SubTile(direction: 1, parentTile: this);
        }
        public LetterTile ToLetterTile(Tile[,] grid, QuestionTile questionTile, string text, PictureBox pb)
        {
            EmptyTileList.Remove(this);
            grid[GetPosition().Y, GetPosition().X] = new LetterTile(GetPosition(), questionTile, text);
            return grid[GetPosition().Y, GetPosition().X] as LetterTile;
        }
        public QuestionTile ToQuestionTile(Tile[,] grid, string question, int direction)
        {
            EmptyTileList.Remove(this);
            grid[GetPosition().Y, GetPosition().X] = new QuestionTile(GetPosition(), question, direction);
            return grid[GetPosition().Y, GetPosition().X] as QuestionTile;
        }

        public BaseWordTile ToBaseWordTile(Tile[,] grid, string question, int direction)
        {
            EmptyTileList.Remove(this);
            grid[GetPosition().Y, GetPosition().X] = new BaseWordTile(GetPosition());
            return grid[GetPosition().Y, GetPosition().X] as BaseWordTile;
        }
        public override void Paint(Graphics g)
        {
            TranslateTransformGraphics(g, GetBounds().Location);
            int ts = Form1.TS;

            // Call subtile painting routines
            SubTiles[0].Paint(g);
            SubTiles[0].Paint(g);

            // Draw Rectangle
            // Condition: At least one subtile is highlighted
            if (SubTiles[0].IsHighlighted() || SubTiles[1].IsHighlighted())
                g.DrawRectangle(Pens.Black, 0, 0, ts - 1, ts - 1);

            // Draw extendedHover
            switch (extendedHover)
            {
                case ExtendedHover.Two_Outlines_Horizontal:
                    g.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                    g.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                    break;
                case ExtendedHover.Three_Outlines_Horizontal:
                    g.DrawLine(extendedHoverPen, 0, 0, ts, 0);
                    g.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                    g.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                    break;
                case ExtendedHover.Two_Outlines_Vertical:
                    g.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                    g.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                    break;
                case ExtendedHover.Three_Outlines_Vertical:
                    g.DrawLine(extendedHoverPen, 0, 0, 0, ts);
                    g.DrawLine(extendedHoverPen, ts, 0, ts, ts);
                    g.DrawLine(extendedHoverPen, 0, ts, ts, ts);
                    break;
            }

            TranslateTransformGraphics(g, new Point(-GetBounds().Location.X, -GetBounds().Location.Y));
        }

        public void Reserve()
        {
            reserved = true;
        }

        public void Unreserve()
        {
            reserved = false;
        }

        public bool IsReservedForQuestionTile()
        {
            return reserved;
        }
        private void RemoveHoverFlagFromBothSubtiles()
        {
            SubTiles[0].SetHoverFlag(false);
            SubTiles[1].SetHoverFlag(false);
        }
        public override void MouseMove(MouseEventArgs e, PictureBox pb, Point[] directions, Tile[,] grid)
        {
            RemoveHoverFlagFromBothSubtiles();
            // Which subtile is mouse over?
            int mouseSubtile = (e.X - GetBounds().X < e.Y - GetBounds().Y) ? 1 : 0;
            SubTile hoverSubTile = SubTiles[mouseSubtile];
            // Check if that subtile has a highlight
            if (hoverSubTile.IsHighlighted())
            {
                // If so, then set hover_flag to true
                hoverSubTile.SetHoverFlag(true);

                // And Activate extendedHover outline for adjacent tiles
                Point directionPoint = directions[hoverSubTile.Direction];
                for (int i = 0; i < TupleToBeFilled.Answer.Length; i++)
                {
                    int letterX = GetPosition().X + directionPoint.X * (1 + i);
                    int letterY = GetPosition().Y + directionPoint.Y * (1 + i);
                    // Out of bounds check
                    if (letterX <= grid.GetUpperBound(1) && letterY <= grid.GetUpperBound(0))
                    {
                        Tile tile = grid[letterY, letterX];
                        // End or middle outline
                        if (i < TupleToBeFilled.Answer.Length - 1)
                            tile.extendedHover = ExtendedHover.Two_Outlines_Horizontal;
                        else
                            tile.extendedHover = ExtendedHover.Three_Outlines_Horizontal;

                        // Vertical mode
                        if (directionPoint.Y == 1)
                            tile.extendedHover += 2;

                        // Save tile with extended hover in list
                        tiles_with_extended_hover_list.Add(tile);
                    }
                }

            }


        }
        public override void MouseClick(MouseEventArgs e, Tile[,] grid) { }
        public override void MouseLeave(MouseEventArgs e, PictureBox pb)
        {
            RemoveHoverFlagFromBothSubtiles();
        }
    }
}
