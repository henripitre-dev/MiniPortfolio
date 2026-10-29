using GoGame.Model.Enums;
using GoGame.Service;

namespace GoGame.Model
{
    public class Game
    {
        #region Player Properties

        public Player[] Players { get; }
        public Player CurrentPlayer { get; private set; }
        public Move? LastMove => Board.LastMove;

        #endregion

        #region Live Game Properties

        public Score? FinalScore { get; private set; }
        public double[] Pointages => FinalScore?.Points ?? [0, 0];

        public Board Board { get; }
        public Handicaps NbHandicaps { get; private set; }
        public bool IsFinished { get; private set; }
        public Player? Winner { get; private set; }
        public bool IsAbandon { get; private set; }
        public int NbMoves { get; private set; }
        public int AvgMoves { get; }

        #endregion

        #region Scoring Properties

        public double Komi { get; }


        #endregion


        public Game(Player p1, Player p2, BoardSize size, double komi, Handicaps nbHandicaps)
        {
            Players = [p1, p2];
            Board = new Board(size);
            CurrentPlayer = p1;
            Komi = komi;
            NbHandicaps = nbHandicaps;
            IsFinished = false;

            NbMoves = 0;
            AvgMoves = size switch
            {
                BoardSize.Small9 => 90,
                BoardSize.Medium13 => 170,
                BoardSize.Large19 => 257,
                _ => 0,
                //https://homepages.cwi.nl/~aeb/go/misc/gostat.html
            };

            foreach (Stone s in Handicap.GetHandicaps((int)size, nbHandicaps, Players.First(p => p.Color == StoneColor.Black)))
                Board.Grid.AddStone(s);
        }

        public void ResetState()
        {
            IsFinished = false;
            IsAbandon = false;
            NbMoves = 0;

            //Reset data for each player (e.g. captures, pass status, thinking time)
            foreach (Player p in Players)
                p.ResetDebugState();

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);

            Board.Clear();
            //Reapply handicap stones if any
            foreach (Stone s in Handicap.GetHandicaps((int)Board.Size, NbHandicaps, Players.First(p => p.Color == StoneColor.Black)))
                Board.Grid.AddStone(s);
        }

        //TODO: Méthode temporairement publique pour les tests, à rendre privée après
        public void SwitchTurn()
        {
            CurrentPlayer.ResetThinkingTime();

            //New CurrentPlayer is the one that is not the current player (lol)
            CurrentPlayer = Players.First(p => p != CurrentPlayer);

            CurrentPlayer.ResetPass();
        }

        public string? PlayTurn(Coords coords)
        {
            if (IsFinished)
                return StatusMsg.ErrGameFinished;

            Move move = new(CurrentPlayer, coords, CurrentPlayer.ThinkingTime);

            string? result = Board.PlayMove(move);
            if (result != null) return result;

            NbMoves++;
            CurrentPlayer.LastMove = LastMove;
            SwitchTurn();

            return null;
        }

        public void PassTurn()
        {
            CurrentPlayer.PassTurn();
            CurrentPlayer.LastMove = new(CurrentPlayer, new(), CurrentPlayer.ThinkingTime, true);
            Board.UpdateBoardMove(CurrentPlayer.LastMove);

            if (Players.All(p => p.HasPassed))
                EndGame();

            else
                SwitchTurn();
        }

        public void AbandonGame()
        {
            IsAbandon = true;
            IsFinished = true;

            Player winner = Players.First(p => p != CurrentPlayer);

            double[] points =
            [
                winner.Color == StoneColor.Black ? 1 : 0,
        winner.Color == StoneColor.White ? 1 : 0
            ];

            double[] times =
            [
                Players[0].TimePlayed,
        Players[1].TimePlayed
            ];

            FinalScore = new Score(
                Players,
                points,
                times,
                true
            );

            Winner = winner;
        }

        public void EndGame()
        {
            IsFinished = true;

            FinalScore = ScoreService.CalculateScore(this);

            Winner = FinalScore.GetWinner();
        }

        public void LoadPreset(DebugPreset preset)
        {
            ResetState();

            switch (preset)
            {
                case DebugPreset.SimpleCapture:
                    LoadSimpleCapture();
                    break;

                case DebugPreset.MultiCapture:
                    LoadMultiCapture();
                    break;

                case DebugPreset.EdgeCapture:
                    LoadEdgeCapture();
                    break;

                case DebugPreset.CornerCapture:
                    LoadCornerCapture();
                    break;

                case DebugPreset.KoFight:
                    LoadKoFight();
                    break;

                case DebugPreset.Snapback:
                    LoadSnapback();
                    break;

                case DebugPreset.SuicideMove:
                    LoadSuicideScenario();
                    break;

                case DebugPreset.HandicapOpening:
                    LoadHandicapOpening();
                    break;

                case DebugPreset.LateGame:
                    LoadLateGame();
                    break;

                case DebugPreset.TerritoryTest:
                    LoadTerritoryTest();
                    break;

                case DebugPreset.FullBoardStress:
                    LoadFullBoardStress();
                    break;

            }
        }


        private void LoadSimpleCapture()
        {
            Board.SetStone(4, 4, StoneColor.White);

            Board.SetStone(4, 3, StoneColor.Black);
            Board.SetStone(3, 4, StoneColor.Black);
            Board.SetStone(5, 4, StoneColor.Black);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);

            // black plays at (4,5)
        }

        private void LoadMultiCapture()
        {
            Board.SetStone(4, 4, StoneColor.White);
            Board.SetStone(4, 5, StoneColor.White);
            Board.SetStone(5, 4, StoneColor.White);

            Board.SetStone(3, 4, StoneColor.Black);
            Board.SetStone(3, 5, StoneColor.Black);

            Board.SetStone(4, 3, StoneColor.Black);
            Board.SetStone(5, 3, StoneColor.Black);

            Board.SetStone(6, 4, StoneColor.Black);
            Board.SetStone(6, 5, StoneColor.Black);

            Board.SetStone(4, 6, StoneColor.Black);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);

            // play 5,5
        }

        private void LoadKoFight()
        {
            /*
                Classic ko shape
            */

            Board.SetStone(2, 1, StoneColor.Black);
            Board.SetStone(1, 2, StoneColor.Black);
            Board.SetStone(3, 2, StoneColor.Black);
            Board.SetStone(2, 3, StoneColor.Black);

            Board.SetStone(2, 2, StoneColor.White);
            Board.SetStone(3, 1, StoneColor.White);
            Board.SetStone(1, 1, StoneColor.White);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);

            // black captures
            // white immediate recapture should fail
        }

        private void LoadSuicideScenario()
        {
            Board.SetStone(4, 3, StoneColor.Black);
            Board.SetStone(3, 4, StoneColor.Black);
            Board.SetStone(5, 4, StoneColor.Black);
            Board.SetStone(4, 5, StoneColor.Black);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.White);

            // white plays center
        }

        private void LoadTerritoryTest()
        {
            // black enclosure
            for (int i = 1; i <= 3; i++)
            {
                Board.SetStone(i, 1, StoneColor.Black);
                Board.SetStone(i, 3, StoneColor.Black);
            }

            Board.SetStone(1, 2, StoneColor.Black);
            Board.SetStone(3, 2, StoneColor.Black);

            // white enclosure
            for (int i = 5; i <= 7; i++)
            {
                Board.SetStone(i, 5, StoneColor.White);
                Board.SetStone(i, 7, StoneColor.White);
            }

            Board.SetStone(5, 6, StoneColor.White);
            Board.SetStone(7, 6, StoneColor.White);
        }

        private void LoadHandicapOpening()
        {
            Board.SetStone(2, 2, StoneColor.Black, true);
            Board.SetStone(6, 6, StoneColor.Black, true);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.White);
        }

        private void LoadLateGame()
        {
            ResetState();

            // =========================
            // BLACK TERRITORY (TOP LEFT)
            // =========================

            for (int x = 0; x <= 3; x++)
            {
                Board.SetStone(x, 0, StoneColor.Black);
                Board.SetStone(x, 3, StoneColor.Black);
            }

            Board.SetStone(0, 1, StoneColor.Black);
            Board.SetStone(0, 2, StoneColor.Black);

            Board.SetStone(3, 1, StoneColor.Black);
            Board.SetStone(3, 2, StoneColor.Black);

            // invasion pressure
            Board.SetStone(2, 2, StoneColor.White);

            // =========================
            // WHITE TERRITORY (BOTTOM RIGHT)
            // =========================

            for (int x = 5; x <= 8; x++)
            {
                Board.SetStone(x, 5, StoneColor.White);
                Board.SetStone(x, 8, StoneColor.White);
            }

            Board.SetStone(5, 6, StoneColor.White);
            Board.SetStone(5, 7, StoneColor.White);

            Board.SetStone(8, 6, StoneColor.White);
            Board.SetStone(8, 7, StoneColor.White);

            // black invasion
            Board.SetStone(6, 6, StoneColor.Black);

            // =========================
            // CENTER FIGHT
            // =========================

            Board.SetStone(4, 3, StoneColor.Black);
            Board.SetStone(4, 4, StoneColor.Black);
            Board.SetStone(5, 4, StoneColor.Black);

            Board.SetStone(3, 4, StoneColor.White);
            Board.SetStone(3, 5, StoneColor.White);
            Board.SetStone(4, 5, StoneColor.White);

            // =========================
            // SIDE BATTLES
            // =========================

            Board.SetStone(7, 1, StoneColor.Black);
            Board.SetStone(7, 2, StoneColor.Black);

            Board.SetStone(8, 2, StoneColor.White);
            Board.SetStone(6, 2, StoneColor.White);

            // =========================
            // CAPTURES
            // =========================

            for (int i = 0; i < 8; i++)
                Players[0].NbCaptures++;

            for (int i = 0; i < 5; i++)
                Players[1].NbCaptures++;

            // =========================
            // CURRENT PLAYER
            // =========================

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);
        }

        private void LoadEdgeCapture()
        {
            Board.SetStone(0, 1, StoneColor.White);

            Board.SetStone(0, 0, StoneColor.Black);
            Board.SetStone(1, 1, StoneColor.Black);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);

            // play 0,2
        }
        private void LoadCornerCapture()
        {
            Board.SetStone(0, 0, StoneColor.White);

            Board.SetStone(1, 0, StoneColor.Black);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);

            // play 0,1
        }
        private void LoadSnapback()
        {
            Board.SetStone(2, 1, StoneColor.Black);
            Board.SetStone(1, 2, StoneColor.Black);
            Board.SetStone(3, 2, StoneColor.Black);
            Board.SetStone(2, 3, StoneColor.Black);

            Board.SetStone(2, 2, StoneColor.White);
            Board.SetStone(1, 1, StoneColor.White);

            CurrentPlayer = Players.First(p => p.Color == StoneColor.Black);
        }

        private void LoadFullBoardStress()
        {
            Random rnd = new();

            for (int x = 0; x < (int)Board.Size; x++)
            {
                for (int y = 0; y < (int)Board.Size; y++)
                {
                    if (rnd.NextDouble() < 0.75)
                    {
                        StoneColor color =
                            rnd.Next(2) == 0
                            ? StoneColor.Black
                            : StoneColor.White;

                        Board.SetStone(x, y, color);
                    }
                }
            }
        }

    }
}
