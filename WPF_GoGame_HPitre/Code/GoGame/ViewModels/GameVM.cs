using GoGame.Commands;
using GoGame.Model;
using GoGame.Model.Enums;
using GoGame.Service;
using GoGame.View;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GoGame.ViewModels
{
    public class GameVM : BaseVM
    {
        //Sommaire des champs, propriétés et méthodes de cette classe générés par IA (En tentative de vous aider à comprendre mon bordel plus rapidement😵‍💫)

        #region Fields

        private readonly DispatcherTimer _timer = new();

        private readonly INavigationService _navigation;

        private Game _game;

        private bool _showDebugPresets;

        private string _statusMessage = string.Empty;

        #endregion

        #region Constructor

        public GameVM(INavigationService nav, Game game)
        {
            _navigation = nav;
            _game = game;

            StartGameTimer();

            Cells = new ObservableCollection<CellVM>();

            foreach (Cell cell in _game.Board.Grid.Cells)
            {
                Cells.Add(new CellVM(cell));
            }

            ClickCellCommand = new RelaisCommandeParametre(ClickCell);
            RestartGameCommand = new RelaisCommande(RestartGame);
            AbandonGameCommand = new RelaisCommande(AbandonGame);
            PlayerPassCommand = new RelaisCommande(PlayerPass);
            QuitGameCommand = new RelaisCommande(QuitGame);
            LoadPresetCommand = new RelaisCommandeParametre(LoadPreset);
            ToggleDebugPresetsCommand = new RelaisCommande(ToggleDebug);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current active game instance.
        /// </summary>
        public Game Game => _game;

        /// <summary>
        /// Collection used to display board cells in the UI. Contains the Stone state and coordinates of each cell, and notifies the UI when a stone is placed or removed.
        /// </summary>
        public ObservableCollection<CellVM> Cells { get; }

        /// <summary>
        /// Indicates whether debug presets should be visible.
        /// </summary>
        public bool ShowDebugPresets
        {
            get => _showDebugPresets;
            set
            {
                _showDebugPresets = value;
                OnPropertyChanged(nameof(ShowDebugPresets));
            }
        }

        /// <summary>
        /// Player currently taking their turn.
        /// </summary>
        public Player CurrentPlayer => _game.CurrentPlayer;

        /// <summary>
        /// Black player reference. (Unmodifiable)
        /// </summary>
        public Player BlackPlayer =>
            _game.Players.First(p => p.Color == StoneColor.Black);

        /// <summary>
        /// White player reference. (Unmodifiable)
        /// </summary>
        public Player WhitePlayer =>
            _game.Players.First(p => p.Color == StoneColor.White);

        /// <summary>
        /// Status message displayed to the user.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        /// <summary>
        /// Fun game estimate of current game progression; Fins esthétiques et qualité seulemenent 
        /// </summary>
        public string GamePhase
        {
            get
            {
                if (Game.IsFinished)
                    return "Les pierres ont parlé.";

                if (Game.NbMoves == 0)
                    return "Les pierres se réchauffent.";

                if (Game.Board.Size == BoardSize.Small9)
                {
                    return Game.NbMoves switch
                    {
                        <= 30 => "Mise en bouche",
                        <= 60 => "Creusée des méninges...",
                        <= 80 => "Le combat fait rage !",
                        <= 100 => "Il commence à se faire tard...",
                        <= 120 => "Fin imminente !",
                        _ => string.Empty
                    };
                }

                return string.Empty;
            }
        }

        #endregion

        #region Commands

        public ICommand ClickCellCommand { get; }

        public ICommand RestartGameCommand { get; }

        public ICommand AbandonGameCommand { get; }

        public ICommand PlayerPassCommand { get; }

        public ICommand QuitGameCommand { get; }

        public ICommand LoadPresetCommand { get; }

        public ICommand ToggleDebugPresetsCommand { get; }

        #endregion

        #region Game Actions

        /// <summary>
        /// Handles a player clicking on a board cell. Initiates PlayTurn 
        /// </summary>
        /// <param name="cellObj">Selected cell view model.</param>
        private void ClickCell(object cellObj)
        {
            CellVM cellVM = (CellVM)cellObj;

            string? error = Game.PlayTurn(cellVM.Coords);

            StatusMessage = error ?? string.Empty;

            if (error == null)
            {
                NotifyPlayerUIChange();

                OnPropertyChanged(nameof(Game));
                OnPropertyChanged(nameof(GamePhase));
            }
        }

        /// <summary>
        /// Passes the current player's turn, ends the game if both players pass consecutively, and opens results window if the game is finished.
        /// Notifies UI changes
        /// </summary>
        private void PlayerPass()
        {
            Game.PassTurn();

            NotifyPlayerUIChange();

            OnPropertyChanged(nameof(Game));
            OnPropertyChanged(nameof(Game.LastMove));
            OnPropertyChanged(nameof(GamePhase));

            if (Game.IsFinished)
            {
                OpenResultsWindow();
            }
        }

        /// <summary>
        /// Restarts the current game. 
        /// Resets the board, players and game state to their initial conditions, and notifies the UI of all changes. 
        /// Does not navigate away from the current game view.
        /// </summary>
        private void RestartGame()
        {
            Game.ResetState();

            OnPropertyChanged(nameof(Game));
            OnPropertyChanged(nameof(GamePhase));
            OnPropertyChanged(nameof(Game.LastMove));

            NotifyPlayerUIChange();

            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Ends the game by resignation. 
        /// Sets the game state to finished with the opponent as the winner, and opens the results window.
        /// </summary>
        private void AbandonGame()
        {
            Game.AbandonGame();
            OpenResultsWindow();
        }

        /// <summary>
        /// Quits the game and goes to welcome page. 
        /// Resets the game state and notifies the UI of all changes.
        /// </summary>
        private void QuitGame()
        {
            Game.ResetState();

            OnPropertyChanged(nameof(Game));
            OnPropertyChanged(nameof(GamePhase));
            OnPropertyChanged(nameof(Game.LastMove));

            NotifyPlayerUIChange();

            StatusMessage = string.Empty;

            Game.EndGame();

            _game = null!;

            _navigation.NavigateToNull();
        }

        #endregion

        #region Debug

        /// <summary>
        /// Toggles visibility of debug presets (raccourcis).
        /// </summary>
        private void ToggleDebug() => ShowDebugPresets = !ShowDebugPresets;

        /// <summary>
        /// Loads a predefined debug board state (equivalent d'une interface)
        /// </summary>
        /// <param name="presetObj">Preset to load.</param>
        private void LoadPreset(object presetObj)
        {
            if (presetObj is not DebugPreset preset)
                return;

            Game.LoadPreset(preset);

            NotifyPlayerUIChange();
            OnPropertyChanged(nameof(Game));

            //Refresh all cell view models to update their stone based on new config.
            foreach (CellVM cell in Cells)
                cell.Refresh();
        }

        #endregion

        #region Timer

        /// <summary>
        /// Starts the game timer and updates player thinking time every second.
        /// </summary>
        private void StartGameTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);

            _timer.Tick += (_, _) =>
            {
                if (Game == null || Game.IsFinished)
                {
                    _timer.Stop();
                    return;
                }

                Game.CurrentPlayer.TickThinkingTime();

                NotifyPlayerUIChange();
            };

            _timer.Start();
        }

        #endregion

        #region UI Updates

        /// <summary>
        /// Refreshes player-related UI bindings.
        /// </summary>
        private void NotifyPlayerUIChange()
        {
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(BlackPlayer));
            OnPropertyChanged(nameof(WhitePlayer));
        }

        /// <summary>
        /// Opens the end game results window.
        /// </summary>
        private void OpenResultsWindow()
        {
            ResultsWnd wnd = new()
            {
                DataContext = new ResultsVM(
                    Game,
                    (NavigationService)_navigation),

                Owner = Application.Current.MainWindow
            };

            wnd.ShowDialog();
        }

        #endregion
    }
}