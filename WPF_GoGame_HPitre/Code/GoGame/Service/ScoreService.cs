using GoGame.Model;
using GoGame.Model.Enums;

namespace GoGame.Service
{
    public static class ScoreService
    {
        /// <summary>
        /// Calculates the score for using the territory scoring method, based on the current state of the game.
        /// </summary>
        /// <param name="game">The game for which to calculate the score.</param>
        /// <returns>The calculated score.</returns>
        public static Score CalculateScore(Game game)
        {
            Player black = game.Players.First(p => p.Color == StoneColor.Black);
            Player white = game.Players.First(p => p.Color == StoneColor.White);

            int blackTerritory = 0;
            int whiteTerritory = 0;

            HashSet<Coords> visited = [];

            foreach (Cell cell in game.Board.Grid.Cells)
            {
                //If no stone or already visited, skip
                if (cell.Stone != null || visited.Contains(cell.Coords))
                    continue;

                //else get the territory and its owner Stonecolor
                var (territory, owner) = BFSService.GetTerritory(game.Board.Grid, cell.Coords, visited);

                if (owner == StoneColor.Black)
                    blackTerritory += territory.Count;

                else if (owner == StoneColor.White)
                    whiteTerritory += territory.Count;
            }

            double blackPoints = blackTerritory;
            double whitePoints = whiteTerritory + game.Komi;

            return new Score([black, white], [blackPoints, whitePoints], [black.TimePlayed, white.TimePlayed], game.IsAbandon);
        }
    }
}