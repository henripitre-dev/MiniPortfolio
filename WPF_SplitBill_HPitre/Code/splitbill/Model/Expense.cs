using System.Diagnostics;
using splitbill.ViewModels;

namespace splitbill.Model
{
    public class Expense
    {
        #region Propriétés

        private string _name;

        private string _description;

        private decimal _amount;

        private Participant _owner;

        private Activity _parent;

        private List<Participant> _participants = new();

        private bool _isActive;

        #region Propriétés Get/Publiques/Readonly

        public string Name => _name;
        public string Description => _description;
        public decimal Amount => _amount;
        public Participant Owner => _owner;
        public Activity Parent => _parent;
        public bool IsActive => _isActive;
        public IReadOnlyList<Participant> Participants => _participants;

        #endregion

        #endregion

        public Expense(Activity parent, string name, decimal amount, Participant owner, List<Participant> participants,string? description = null)
        {
            _parent = parent;
            _name = name;
            _owner = owner;
            _amount = amount;

            _description = string.IsNullOrWhiteSpace(description)
                ? "Sans description"
                : description.Trim();

            _participants.Add(owner);
            foreach (Participant p in participants)
                if (!_participants.Contains(p))
                    _participants.Add(p);

            _isActive = true;
        }

        #region Méthodes

        /// <summary>
        /// Invert this activity's Active state (IsActive)
        /// </summary>
        /// <returns>The new IsActive state</returns>
        public void SwitchState() => _isActive = !_isActive;
        
        #region UNUSED
        /// <summary>
        /// Adds a Participant to the Participants expense's list
        /// </summary>
        /// <param name="participant">Participant object to add</param>
        public void AddParticipant(Participant participant)
        {
            if (!_participants.Contains(participant))
                _participants.Add(participant);
        }

        #endregion
        
        #endregion
    }
}
