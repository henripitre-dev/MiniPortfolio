using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GoGame.Model.Enums;

namespace GoGame.Model
{
    public class Stone
    {
        public Player Owner { get; }
        public StoneColor Color => Owner.Color;
        public Coords Coords { get; }
        public bool IsHandicap;
        public DateTime TimeStamp { get; }

        public Stone(Player owner, Coords coords, bool isHandicap = false)
        {
            Owner = owner;
            Coords = coords;
            TimeStamp = DateTime.Now;
            IsHandicap = isHandicap;
        }

    }
}
