using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoGame.Model.Enums;

namespace GoGame.Model
{
    public class Player
    {
        #region Fixed Properties

        public string Name { get; }
        public StoneColor Color { get; }

        #endregion

        #region Time Properties & Methods

        public int TimePlayed { get; private set; }
        public int ThinkingTime { get; private set; }
        public void TickThinkingTime()
        {
            ThinkingTime++;
            TimePlayed++;
        }

        public void ResetThinkingTime() => ThinkingTime = 0;

        #endregion

        public Move? LastMove { get; set; } //Can be changed publicly
        public int NbCaptures { get; set; } //Can be changed publicly

        public bool HasPassed { get; private set; }

        //Constructeur
        public Player(string name, StoneColor color)
        {
            Name = name;
            Color = color;
            NbCaptures = 0;
        }

        public void PassTurn() => HasPassed = true;
        public void ResetPass() => HasPassed = false;

        public void ResetDebugState()
        {
            TimePlayed = 0;
            ThinkingTime = 0;

            NbCaptures = 0;
            HasPassed = false;
            LastMove = null;
        }

    }
}
