using GoGame.Model;
using GoGame.ViewModels;

public class CellVM : BaseVM
{
    private readonly Cell _cell;

    public CellVM(Cell cell)
    {
        _cell = cell;

        _cell.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Cell.Stone))
                OnPropertyChanged(nameof(Stone));
        };
    }

    public Coords Coords => _cell.Coords;
    public bool IsStarPoint => _cell.IsStarPoint;

    public Stone? Stone => _cell.Stone;

    public void Refresh()
    {
        OnPropertyChanged(nameof(Stone));
    }
}