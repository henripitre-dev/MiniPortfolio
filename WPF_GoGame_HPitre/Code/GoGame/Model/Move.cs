using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoGame.Model
{
    public class Move
    {
        public Player Player { get; }
        public Coords Coords { get; }
        public DateTime TimeStamp { get; }
        public int ThinkingTime { get; }
        public bool IsPass { get; }

        public Move(Player player, Coords xy, int ThinkingTimeSecs, bool isPass = false)
        {
            Player = player;
            Coords = xy;
            TimeStamp = DateTime.Now;
            ThinkingTime = ThinkingTimeSecs;
            IsPass = isPass;

        }
    }
}
