using System.ComponentModel;

namespace GoGame.Model
{
    public class Cell : INotifyPropertyChanged
    {
        //Quick fix: CellVM is not refreshing when a stone is added/removed from the cell, so we need to implement INotifyPropertyChanged in Cell and raise the event when the Stone property changes.
        public Coords Coords { get; set; }

        private Stone? _stone;
        public Stone? Stone
        {
            get => _stone;
            set
            {
                if (_stone != value)
                {
                    _stone = value;
                    OnPropertyChanged(nameof(Stone));
                }
            }
        }
        public bool IsStarPoint { get; set; }

        //Constructeur
        public Cell(Coords c) => Coords = c;

        public void AddStoneToCell(Stone s) => Stone = s;
        public void RemoveStone() => Stone = null;

        #region PropertyChange Init

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion


    }
}