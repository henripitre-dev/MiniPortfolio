using GoGame.Commands;
using GoGame.Model;
using GoGame.Model.Enums;
using GoGame.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GoGame.ViewModels
{
    public class CreateGameVM : BaseVM
    {
        #region Propriétés changeables

        public string PlayerBlackName { get; set; } = "Noir";
        public string PlayerWhiteName { get; set; } = "Blanc";

        public BoardSize SelectedBoardSize
        {
            get => _selectedBoardSize;
            set
            {
                _selectedBoardSize = value;
                OnPropertyChanged(nameof(SelectedBoardSize));
                OnPropertyChanged(nameof(CanStartGame));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private BoardSize _selectedBoardSize = BoardSize.Small9;


        private double _komi = 6.5;
        public double Komi
        {
            get => _komi;
            set
            {
                _komi = value;
                OnPropertyChanged(nameof(Komi));
                OnPropertyChanged(nameof(CanStartGame));
            }
        }

        private Handicaps _nbHandicaps = Handicaps.None;
        public Handicaps NbHandicaps
        {
            get => _nbHandicaps;
            set
            {
                _nbHandicaps = value;
                OnPropertyChanged(nameof(NbHandicaps));
                OnPropertyChanged(nameof(CanStartGame));
            }
        }

        #endregion

        #region Commandes
        public ICommand StartGameCommand { get; }
        public ICommand QuitCommand { get; }
        public ICommand GoBackCommand { get; }

        #endregion

        private INavigationService _navigation;
        public bool CanStartGame => SelectedBoardSize == BoardSize.Small9;

        public CreateGameVM(INavigationService nav)
        {
            _navigation = nav;
            StartGameCommand = new RelaisCommande(StartGame, () => CanStartGame);
            QuitCommand = new RelaisCommandeParametre(Quit);
            GoBackCommand = new RelaisCommande(GoBack);
        }

        private void StartGame()
        {
            Player p1 = new(PlayerBlackName, StoneColor.Black);
            Player p2 = new(PlayerWhiteName, StoneColor.White);

            Game game = new(p1, p2, SelectedBoardSize, Komi, NbHandicaps);

            _navigation.NavigateToGame(game);
        }

        private void Quit(object cmdParam)
        {
            if (cmdParam is Window window)
                window.Close();
        }

        private void GoBack() => _navigation.NavigateToNull();
    }
}
