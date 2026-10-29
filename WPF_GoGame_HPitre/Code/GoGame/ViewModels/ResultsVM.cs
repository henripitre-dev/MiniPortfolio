using GoGame.Commands;
using GoGame.Model;
using GoGame.Service;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GoGame.ViewModels
{
    public class ResultsVM : BaseVM
    {
        private readonly NavigationService _navigation;
        private readonly Game theGame;

        #region ALL Display Properties

        public string EndReason { get; set; }
        public string WinnerText { get; set; }
        public string WinnerSummary { get; set; }
        public Brush WinnerStoneBrush { get; set; }
        public double BlackScore { get; set; }
        public double WhiteScore { get; set; }
        public string ScoreDifferenceText { get; set; }
        public string BlackPlayerName { get; set; }
        public string WhitePlayerName { get; set; }
        public string BlackPlayerTime { get; set; }
        public string WhitePlayerTime { get; set; }
        public string BlackTerritoryText { get; set; }
        public string WhiteTerritoryText { get; set; }
        public string TotalMovesText { get; set; }
        public string BoardSizeText { get; set; }
        public string KomiText { get; set; }
        public string HandicapText { get; set; }
        public string TotalDurationText { get; set; }
        public string SummaryText { get; set; }

        #endregion

        #region Commands
        public ICommand NewGameCommand { get; }
        public ICommand ReplayCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand QuitCommand { get; }

        #endregion

        public ResultsVM(Game game, NavigationService nav)
        {
            _navigation = nav;
            theGame = game;

            if (game.IsFinished == false)
                throw new InvalidOperationException("Le jeu n'est pas terminé; Cette page est inaccessible.");

            Player winner = game.Winner;

            #region Assignation des Display Properties

            WinnerText = $"{winner.Name} gagne";
            WinnerSummary = $"Victoire des {(winner.Color == Model.Enums.StoneColor.White ? "Blancs" : "Noirs")}";

            WinnerStoneBrush = winner.Color == Model.Enums.StoneColor.White
                ? Brushes.White
                : Brushes.Black;

            Player blackPlayer = game.Players.First(p => p.Color == Model.Enums.StoneColor.Black);
            Player whitePlayer = game.Players.First(p => p.Color == Model.Enums.StoneColor.White);

            int blackIndex = Array.IndexOf(game.Players, blackPlayer);
            int whiteIndex = Array.IndexOf(game.Players, whitePlayer);

            BlackScore = game.Pointages[blackIndex];
            WhiteScore = game.Pointages[whiteIndex];

            double diff = Math.Abs(BlackScore - WhiteScore);

            //Affiche la différence de score avec une décimale et "pts"
            ScoreDifferenceText = $"{diff:F1} pts";

            //Noms des joueurs
            BlackPlayerName = blackPlayer.Name;
            WhitePlayerName = whitePlayer.Name;

            //Temps formaté en h/m/s si plus d'une heure, sinon m/s
            BlackPlayerTime = $"Temps : {FormatTime(blackPlayer.TimePlayed)}";
            WhitePlayerTime = $"Temps : {FormatTime(whitePlayer.TimePlayed)}";

            //Vide si abandon, sinon affiche le score de territoire
            if (game.IsAbandon) BlackTerritoryText = $"";
            else BlackTerritoryText = $"Territoires : {BlackScore:F1}";

            //^^
            if (game.IsAbandon) WhiteTerritoryText = $"";
            else WhiteTerritoryText = $"Territoires : {WhiteScore - game.Komi:F1}";

            //Affiche le nombre total de coups joués
            TotalMovesText = $"Coups joués : {game.NbMoves}";

            //Affiche la taille du plateau
            BoardSizeText = $"Plateau : {(int)game.Board.Size}×{(int)game.Board.Size}";

            //Affiche la valeur du komi
            KomiText = $"Komi : {game.Komi:F1} (Blanc)";

            //Affiche le nombre de handicaps
            HandicapText = $"Handicap : {(int)game.NbHandicaps} (Noir)";

            //Affiche ;mps total de jeu en format h/m/s si plus d'une heure, sinon m/s
            TotalDurationText = $"Durée totale : {FormatTime(blackPlayer.TimePlayed + whitePlayer.TimePlayed)}";

            //Affiche la raison de la fin de partie
            EndReason = game.IsAbandon
                ? "Victoire par abandon"
                : "Victoire après deux passes consécutives";

            //Récapitule la victoire en fonction de la raison de fin de partie
            SummaryText = game.IsAbandon
                ? $"{winner.Name} remporte la partie par abandon."
                : $"{winner.Name} contrôle davantage de territoire et remporte la partie.";

            #endregion

            Export();

            //Assignation des Commandes
            NewGameCommand = new RelaisCommandeParametre(NewGame);
            ReplayCommand = new RelaisCommandeParametre(Replay);
            CloseCommand = new RelaisCommandeParametre(Close);
            QuitCommand = new RelaisCommande(Quit);
        }


        /// <summary>
        /// Formate le temps en heures, minutes et secondes.
        /// </summary>
        /// <param name="totalSeconds">Le temps total en secondes.</param>
        /// <returns>Une chaîne de caractères représentant le temps formaté.</returns>
        private static string FormatTime(int totalSeconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);

            if (t.TotalHours >= 1)
                return $"{(int)t.TotalHours}h {t.Minutes}m {t.Seconds}s";

            return $"{t.Minutes}m {t.Seconds}s";
        }

        /// <summary>
        /// Démarre une nouvelle partie et ferme la fenêtre actuelle.
        /// </summary>
        /// <param name="cmdParam">Le paramètre de commande, attendu comme une fenêtre.</param>
        private void NewGame(object cmdParam)
        {
            _navigation.NavigateToCreateGame();

            if (cmdParam is Window window)
                window.Close();
        }
        /// <summary>
        /// Rejoue la partie actuelle et ferme la fenêtre de résultats.
        /// </summary>
        private void Replay(object cmdParam)
        {
            Game replayGame = new(theGame.Players[0], theGame.Players[1], theGame.Board.Size, theGame.Komi, theGame.NbHandicaps);
            _navigation.NavigateToGame(replayGame);

            if (cmdParam is Window window)
                window.Close();
        }

        /// <summary>
        /// Exporte les résultats de la partie actuelle au format XML.
        /// </summary>
        private void Export()
        {
            try
            {
                XMLService.ExportGameResults(theGame);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'exportation XML.\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void Close(object cmdParam)
        {
            if (cmdParam is Window window)
                window.Close();
        }

        private void Quit() => Application.Current.Shutdown();
    }
}