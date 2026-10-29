using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoGame.Model;
using GoGame.Service;
using GoGame.Model.Enums;
using System.Windows.Controls;

namespace Tests
{
    [TestClass]
    public class ScoreTests
    {
        [TestMethod]
        public void Komi_IsAddedToWhite()
        {
            Player black = new Player("Black", StoneColor.Black);
            Player white = new Player("White", StoneColor.White);

            Game game = new Game(black, white, BoardSize.Small9, 6.5, 0);
            Board board = new Board(BoardSize.Small9);

            Score result = ScoreService.CalculateScore(game);

            Assert.AreEqual(6.5, result.Points[1]);
        }

        [TestMethod]
        public void Winner_IsCorrect()
        {
            Player black = new Player("Black", StoneColor.Black);
            Player white = new Player("White", StoneColor.White);

            Game game = new(black, white, BoardSize.Small9, 6.5, 0);
            Board board = new(BoardSize.Small9);

            Score result = ScoreService.CalculateScore(game);


            Player winner = result.GetWinner();

            //White should have 6.5 points more than black, so white should win
            Assert.AreEqual(StoneColor.White, winner.Color);
        }
    }
}