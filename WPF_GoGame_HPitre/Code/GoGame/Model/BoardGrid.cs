using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoGame.Model;
using GoGame.Model.Enums;

namespace GoGame.Model
{
    //Utilité de cette fonction:
    //https://chatgpt.com/share/69fa49c1-bb4c-83ea-bd1e-30c7d78d40a7

    //Similaire à la la méthode "typedef" in C/C++, elle permet de créer une classe qui encapsule un tableau 2D de cellules,
    //avec des indexeurs pour accéder aux cellules par coordonnées. Cela rend le code plus propre et plus facile à lire,
    //en évitant d'avoir à manipuler directement les tableaux 2D dans le reste du code.

    public class BoardGrid
    {
        public int Size { get; }
        private readonly Cell[,] _grid;
        public BoardGrid(int size)
        {
            Size = size;
            _grid = new Cell[size, size];
        }

        // Indexer (Facon d'appel) 1: Grid[x, y]
        public Cell this[int x, int y]
        {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }

        // Indexer (Facon d'appel) 2: Grid[obj Coords]
        public Cell this[Coords c]
        {
            get => _grid[c.X, c.Y];
            set => _grid[c.X, c.Y] = value;
        }

        public Cell this[Coords? c]
        {
            get => _grid[c.Value.X, c.Value.Y];
            set => _grid[c.Value.X, c.Value.Y] = value;
        }

        public List<Cell> Cells
        {
            get
            {
                List<Cell> cells = new List<Cell>();
                for (int x = 0; x < _grid.GetLength(0); x++)
                    for (int y = 0; y < _grid.GetLength(1); y++)
                        cells.Add(_grid[x, y]);
                return cells;
            }
        }

        public void AddStone(Stone s) => _grid[s.Coords.X, s.Coords.Y].AddStoneToCell(s);
        public void RemoveStone(Stone s) => _grid[s.Coords.X, s.Coords.Y].RemoveStone();

        public bool IsInside(Coords xy)
        {
            return xy.X >= 0 &&
                   xy.Y >= 0 &&
                   xy.X < _grid.GetLength(0) &&
                   xy.Y < _grid.GetLength(1);
        }
    }
}
