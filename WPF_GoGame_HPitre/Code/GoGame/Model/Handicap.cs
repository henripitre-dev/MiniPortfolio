using GoGame.Model.Enums;

namespace GoGame.Model
{
    public static class Handicap
    {
        #region Star Points Handling
        public static Coords[] GetStarPoints(int size)
        {
            int maxStars = size == 9 || size == 13 ? 5 : 9;
            Coords[] stars = new Coords[maxStars];

            switch (size)
            {
                case 9:
                    stars = [new(6, 6), new(2, 2), new(6, 2), new(2, 6), new(4, 4)];
                    break;

                case 13:
                case 19:
                    throw new ArgumentException("Unsupported board size");
            }

            return stars;
        }

        #endregion

        #region Decide and Place Handicaps

        private static readonly Random r = new();

        public static List<Stone> GetHandicaps(int boardSize, Handicaps handicapCount, Player plr)
        {
            List<Stone> stones = [];
            int maxSpots = 0;

            switch (boardSize)
            {
                case 9:
                case 13:
                    maxSpots = 5;
                    break;
                case 19:
                    maxSpots = 9;
                    break;
            }

            Coords[] starPoints = GetStarPoints(boardSize);
            int nbStars = starPoints.Length;
            int handicapCountInt = Math.Min((int)handicapCount, nbStars);
            Coords newPos;

            //FOr x number of handicaps, 
            while (stones.Count < handicapCountInt && stones.Count < nbStars)
            {
                newPos = starPoints[r.Next(maxSpots)];

                if (!stones.Any(s => s.Coords.X == newPos.X && s.Coords.Y == newPos.Y))
                {
                    stones.Add(new Stone(plr, new(newPos.X, newPos.Y), true));
                }
            }

            return stones;
        }

        #endregion

    }
}