using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoGame.Model;
using GoGame.Model.Enums;
using GoGame.Service;

namespace GoGame.Model
{
    public class Board
    {
        #region Properties
        public BoardSize Size { get; }
        public BoardGrid Grid { get; }
        public Move? LastMove { get; private set; }
        private Coords? _koPoint = null;
        public Coords[] StarPoints;

        #endregion

        public Board(BoardSize size)
        {
            Size = size;
            Grid = new BoardGrid((int)size);

            //Populate grid with empty cells (not null)
            for (int x = 0; x < (int)size; x++)
                for (int y = 0; y < (int)size; y++)
                    Grid[x, y] = new Cell(new(x, y));

            //Make the starpoints
            StarPoints = Handicap.GetStarPoints((int)size);
            foreach (Coords star in StarPoints)
                Grid[star].IsStarPoint = true;

        }

        #region Validation Methods

        //public bool IsEmpty(int x, int y) => Grid[x, y].Stone == null;

        private string? ValidateMove(Move move, Player currentPlayer)
        {
            if (!Grid.IsInside(move.Coords))
                return StatusMsg.ErrInvalidMove;

            if (Grid[move.Coords].Stone != null)
                return StatusMsg.ErrCellOccupied;

            if (IsKo(move, currentPlayer))
                return StatusMsg.ErrKoRule;

            if (IsSuicide(move))
                return StatusMsg.ErrSuicideMove;

            return null;
        }

        private bool IsKo(Move move, Player currentPlayer)
        {
            if (_koPoint == null)
                return false;

            return move.Coords.Equals(_koPoint);
        }

        private bool IsSuicide(Move move)
        {
            if (Grid[move.Coords].Stone != null)
                return false;

            // place temporary stone
            Stone placedStone = new(move.Player, move.Coords);

            Grid[move.Coords].Stone = placedStone;

            // simulate captures
            List<Stone> captured =
                BFSService.CaptureStones(Grid, move);

            // temporarily remove captured stones
            foreach (Stone stone in captured)
            {
                Grid[stone.Coords].Stone = null;
            }

            // test liberties without captured
            bool hasLiberties =
                BFSService.HasLiberties(Grid, move.Coords);

            // rollback captures
            foreach (Stone stone in captured)
                Grid[stone.Coords].Stone = stone;

            // rollback placed stone
            Grid[move.Coords].Stone = null;

            return !hasLiberties;
        }

        #endregion

        //public Stone? GetStone(int x, int y) => Grid[x, y].Stone;

        /// <summary>
        /// Plays a move on the board.
        /// </summary>
        /// <param name="move">The move to be played.</param>
        /// <returns>A string indicating an error message if the move is invalid, or null if the move is valid.</returns>
        public string? PlayMove(Move move)
        {
            string? error = ValidateMove(move, move.Player);

            if (error != null)
                return error;

            // place stone first to check
            Grid[move.Coords].AddStoneToCell(new Stone(move.Player, move.Coords));

            //Remove captured stones
            List<Stone> capturedStones = BFSService.CaptureStones(Grid, move);
            foreach (Stone captured in capturedStones)
            {
                Grid[captured.Coords].RemoveStone();
                move.Player.NbCaptures++;
            }

            LastMove = move;
            UpdateKo(move.Coords, capturedStones);

            return null; //ok
        }

        /// <summary>
        /// Bypass the validation and directly set a stone on the board. Used for loading games and setting up handicap stones.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        /// <param name="color">The color of the stone.</param>
        /// <param name="isHandicap">Indicates if the stone is a handicap stone.</param>
        public void SetStone(int x, int y, StoneColor color, bool isHandicap = false)
        {
            Cell cell = Grid.Cells.First(c => c.Coords.X == x && c.Coords.Y == y);

            cell.Stone = new(new("", color), new(x, y), isHandicap);
        }


        /// <summary>
        /// Clear all Stones on grid, including ko and last move. Used for resetting the board for a new game / preset.
        /// </summary>
        public void Clear()
        {
            foreach (Cell cell in Grid.Cells)
                cell.Stone = null;

            LastMove = null;
            _koPoint = null;
        }

        public void UpdateBoardMove(Move newLastMove) => LastMove = newLastMove;

        /// <summary>
        /// Updates the Ko point based on the last move and captured stones.
        /// </summary>
        /// <param name="placedStone">The coordinates of the placed stone.</param>
        /// <param name="capturedStones">The list of captured stones.</param>
        private void UpdateKo(Coords placedStone, List<Stone> capturedStones)
        {
            // Simple Ko only applies if 1 stone was captured
            if (capturedStones.Count == 1)
                _koPoint = capturedStones[0].Coords;
            else
                _koPoint = null;
        }


    }
}
