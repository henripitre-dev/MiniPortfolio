using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoGame.Model.Enums;

namespace GoGame.Model
{
    public struct Coords
    {
        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString() => $"({X}, {Y})";

        //Copilot
        public override bool Equals(object obj)
        {
            if (obj is Coords other)
                return X == other.X && Y == other.Y;
            return false;
        }
    }
}
