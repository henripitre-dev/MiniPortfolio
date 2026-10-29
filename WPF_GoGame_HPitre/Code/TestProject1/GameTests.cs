using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoGame.Model;
using GoGame.Model.Enums;

namespace Tests
{
    [TestClass]
    public class GameTests
    {
        private Game _game;

        [TestInitialize]
        public void Setup()
        {
            Player black = new Player("Black", StoneColor.Black);
            Player white = new Player("White", StoneColor.White);

            _game = new Game(black, white, BoardSize.Small9, 0, 0);
        }

        [TestMethod]
        public void SwitchTurn_ChangesCurrentPlayer()
        {
            Player initial = _game.CurrentPlayer;

            //Méthode temporairement publique pour les tests, à rendre privée après
            _game.SwitchTurn();

            Assert.AreNotEqual(initial, _game.CurrentPlayer);
        }

        [TestMethod]
        public void TwoPasses_EndGame()
        {
            _game.PassTurn();
            _game.PassTurn();

            Assert.IsTrue(_game.IsFinished);
        }

        [TestMethod]
        public void Abandon_SetsFinished()
        {
            _game.AbandonGame();

            Assert.IsTrue(_game.IsFinished);
            Assert.IsTrue(_game.IsAbandon);
        }
    }
}