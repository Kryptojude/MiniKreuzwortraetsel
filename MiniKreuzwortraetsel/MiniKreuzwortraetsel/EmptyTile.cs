using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

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
                emptyTile.SubTiles[0].RemoveHighlight();
                emptyTile.SubTiles[1].RemoveHighlight();
            }
        }

        bool reserved = false;
        public SubTile[] SubTiles { get; } = new SubTile[2];

        public EmptyTile(Point position) : base(position)
        {
            EmptyTileList.Add(this);
            SubTiles[0] = new SubTile(direction: 0, parentTile: this);
            SubTiles[1] = new SubTile(direction: 1, parentTile: this);
        }

        public LetterTile ToLetterTile(Tile[,] grid, QuestionOrBaseWordTile questionOrBaseWordTile, string text, PictureBox pb)
        {
            EmptyTileList.Remove(this);
            grid[GetPosition().Y, GetPosition().X] = new LetterTile(GetPosition(), questionOrBaseWordTile, text);
            return grid[GetPosition().Y, GetPosition().X] as LetterTile;
        }
        public QuestionTile ToQuestionTile(Tile[,] grid, string question, int direction)
        {
            EmptyTileList.Remove(this);
            grid[GetPosition().Y, GetPosition().X] = new QuestionTile(GetPosition(), question, direction);
            return grid[GetPosition().Y, GetPosition().X] as QuestionTile;
        }

        public BaseWordTile ToBaseWordTile(Tile[,] grid, int direction)
        {
            EmptyTileList.Remove(this);
            grid[GetPosition().Y, GetPosition().X] = new BaseWordTile(GetPosition(), direction);
            return grid[GetPosition().Y, GetPosition().X] as BaseWordTile;
        }
        public override void Paint(Graphics g)
        {
            BeginPaint(g);

            Rectangle LocalBounds = GetLocalBounds();
            // Call subtile painting routines
            SubTiles[0].Paint(g);
            SubTiles[1].Paint(g);

            // Draw Rectangle
            // Condition: At least one subtile is highlighted
            if (SubTiles[0].IsHighlighted() || SubTiles[1].IsHighlighted())
                g.DrawRectangle(Pens.Black, 0, 0, LocalBounds.Width - 1, LocalBounds.Height - 1);

            DrawExtendedHover(g);

            EndPaint(g);
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
            RemoveAllExtendedHover();
            // Which subtile is mouse over?
            int mouseSubtile = (e.X - GetGlobalBounds().X < e.Y - GetGlobalBounds().Y) ? 1 : 0;
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
                            tile.SetExtendedHover(ExtendedHover.Two_Outlines_Horizontal);
                        else
                            tile.SetExtendedHover(ExtendedHover.Three_Outlines_Horizontal);

                        // Vertical mode
                        if (directionPoint.Y == 1)
                            tile.SetExtendedHover(tile.GetExtendedHover() + 2);

                        // Save tile with extended hover in list
                        tiles_with_extended_hover_list.Add(tile);
                    }
                }

            }

            SetRepaintFlag(true);
        }
        /// <returns>Returns whether FillAnswer() method should be called</returns>
        public bool MouseClick(MouseEventArgs e, out int direction) 
        {
            bool callFillAnswer = false;
            // Which subTile was clicked?
            int subTileIdx = (e.X - GetGlobalBounds().Location.X > e.Y - GetGlobalBounds().Location.Y) ? 0:1;
            direction = subTileIdx;
            SubTile clickedSubTile = SubTiles[subTileIdx];
            // Is clicked subTile highlighted?
            if (clickedSubTile.IsHighlighted())
            {
                RemoveAllHighlights();
                RemoveAllExtendedHover();
                callFillAnswer = true;
            }

            return callFillAnswer;
        }
        public override void MouseLeave(MouseEventArgs e, PictureBox pb)
        {
            RemoveHoverFlagFromBothSubtiles();
            SetRepaintFlag(true);
            RemoveAllExtendedHover();
        }
    }
}
