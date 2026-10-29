using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoGame.Model;
using GoGame.Model.Enums;

namespace Tests
{
    [TestClass]
    public class KoRuleTests
    {
        private Board _board = null!;
        private Player _black = null!;
        private Player _white = null!;

        [TestInitialize]
        public void Setup()
        {
            _board = new Board(BoardSize.Small9);

            _black = new Player("Black", StoneColor.Black);
            _white = new Player("White", StoneColor.White);
        }

        [TestMethod]
        public void KoMove_IsRejected()
        {
            // Setup classique d'un Ko

            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_white, new(0, 0), 0));

            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_white, new(2, 1), 0));

            _board.PlayMove(new Move(_black, new(1, 2), 0));
            _board.PlayMove(new Move(_white, new(2, 0), 0));

            _board.PlayMove(new Move(_black, new(2, 2), 0));
            _board.PlayMove(new Move(_white, new(1, 1), 0));

            _board.PlayMove(new Move(_black, new(1, 0), 0));

            string result = _board.PlayMove(new Move(_white, new(1, 1), 0));
            bool resultBool = result == null;

            Assert.IsFalse(resultBool);
        }
    }
}