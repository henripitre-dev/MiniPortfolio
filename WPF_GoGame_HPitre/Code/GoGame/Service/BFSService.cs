using GoGame.Model;
using GoGame.Model.Enums;

namespace GoGame.Service
{
    public static class BFSService
    {

        #region GridLike BFS Search

        private static readonly int[][] dirs =
            {
                [1, 0],
                [-1, 0],
                [0, 1],
                [0, -1]
            };

        private static List<Coords> GetNeighbors(BoardGrid Grid, Coords c)
        {
            List<Coords> neighbors = [];

            foreach (int[] d in dirs)
            {
                int x = c.X + d[0];
                int y = c.Y + d[1];

                if (Grid.IsInside(new(x, y)))
                    neighbors.Add(new Coords(x, y));
            }

            return neighbors;
        }

        #endregion

        #region GoGame Specific BFS Search

        public static bool HasLiberties(BoardGrid grid, Coords start)
        {
            Stone? initial = grid[start].Stone;
            if (initial == null)
                return false;

            HashSet<Coords> visited = new();
            Queue<Coords> queue = new();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Coords current = queue.Dequeue();

                foreach (Coords n in GetNeighbors(grid, current))
                {
                    Cell cell = grid[n];

                    if (cell.Stone == null)
                        return true;

                    if (cell.Stone.Color == initial.Color && visited.Add(n))
                        queue.Enqueue(n);
                }
            }

            return false;
        }
        public static List<Stone> CaptureStones(BoardGrid grid, Move move)
        {
            StoneColor opponentColor = move.Player.Color == StoneColor.Black
                ? StoneColor.White
                : StoneColor.Black;

            List<Stone> captured = new();

            HashSet<Coords> visitedGroups = new();

            foreach (Coords neighbor in GetNeighbors(grid, move.Coords))
            {
                Cell cell = grid[neighbor];
                // Only opponent stones
                if (cell.Stone == null || cell.Stone.Color != opponentColor)
                    continue;

                // Avoid recheking same group
                if (visitedGroups.Contains(neighbor))
                    continue;

                // Get group + liberty check in ONE BFS
                var (group, hasLiberty) = GetGroupAndLiberties(grid, neighbor);

                foreach (Coords c in group)
                    visitedGroups.Add(c);

                if (!hasLiberty)
                    foreach (Coords d in group)
                        captured.Add(grid[d].Stone);
            }

            return captured;
        }
        public static (List<Coords> group, bool hasLiberty) GetGroupAndLiberties(BoardGrid grid, Coords start)
        {
            Stone? initial = grid[start].Stone;
            if (initial == null)
                return (new List<Coords>(), false);

            List<Coords> group = [];
            HashSet<Coords> visited = [];
            Queue<Coords> queue = new();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Coords current = queue.Dequeue();
                group.Add(current);

                foreach (Coords n in GetNeighbors(grid, current))
                {
                    Cell cell = grid[n];

                    // EMPTY = LIBERTY FOUND IMMEDIATELY
                    if (cell.Stone == null)
                        return (group, true);

                    // SAME COLOR = continue BFS
                    if (cell.Stone.Color == initial.Color && visited.Add(n))
                        queue.Enqueue(n);
                }
            }

            return (group, false);
        }


        public static (List<Coords> territory, StoneColor? owner)
        GetTerritory(BoardGrid grid, Coords start, HashSet<Coords> globalVisited)
        {
            List<Coords> territory = new();
            Queue<Coords> queue = new();
            HashSet<StoneColor> borderingColors = new();

            bool touchesEdge = false;

            queue.Enqueue(start);
            globalVisited.Add(start);

            while (queue.Count > 0)
            {
                Coords current = queue.Dequeue();
                territory.Add(current);

                if (current.X == 0 || current.Y == 0 || current.X == grid.Size - 1 || current.Y == grid.Size - 1)
                    touchesEdge = true;

                foreach (Coords neighbor in GetNeighbors(grid, current))
                {
                    Cell neighborCell = grid[neighbor];

                    if (neighborCell.Stone == null)
                    {
                        if (globalVisited.Add(neighbor))
                            queue.Enqueue(neighbor);
                    }
                    else
                        borderingColors.Add(neighborCell.Stone.Color);
                }
            }

            if (touchesEdge)
                return (territory, null);

            if (borderingColors.Count == 1)
                return (territory, borderingColors.First());

            return (territory, null);
        }
        #endregion
    }
}
