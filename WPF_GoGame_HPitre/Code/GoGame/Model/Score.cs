using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoGame.Model.Enums;

namespace GoGame.Model
{
    public class Score
    {
        public Player[] Players { get; }
        public double[] Points { get; }
        public double[] Times { get; }
        public bool IsAbandon { get; }

        public Score(Player[] players, double[] points, double[] times, bool isAbandon)
        {
            Players = players;
            Points = points;
            Times = times;
            IsAbandon = isAbandon;
        }

        public Player GetWinner()
            => Points[0] > Points[1] ? Players[0] : Players[1];
    }
}
