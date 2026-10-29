using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoGame.Model;
using GoGame.Model.Enums;

namespace Tests
{
    [TestClass]
    public class BoardTests
    {
        private Board _board;
        private Player _black;
        private Player _white;

        [TestInitialize]
        public void Setup()
        {
            _board = new Board(BoardSize.Small9);
            _black = new Player("Black", StoneColor.Black);
            _white = new Player("White", StoneColor.White);
        }

        [TestMethod]
        public void PlaceStone_ValidMove_ReturnsTrue()
        {
            Move move = new Move(_black, new(4, 4), 0);

            string? result = _board.PlayMove(move);
            bool resultBool = result == null;

            Assert.IsTrue(resultBool);
            Assert.IsNotNull(_board.Grid[4, 4]);
        }

        [TestMethod]
        public void PlaceStone_OnOccupiedCell_ReturnsFalse()
        {
            _board.PlayMove(new Move(_black, new(4, 4), 0));

            string result = _board.PlayMove(new Move(_white, new(4, 4), 0));
            bool resultBool = result == null;

            Assert.IsFalse(resultBool);
        }

        [TestMethod]
        public void CaptureSingleStone_RemovesStone()
        {
            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_black, new(2, 1), 0));
            _board.PlayMove(new Move(_black, new(1, 2), 0));

            _board.PlayMove(new Move(_white, new(1, 1), 0));

            Assert.IsNull(_board.Grid[1, 1].Stone);
        }

        [TestMethod]
        public void SuicideMove_ReturnsFalse()
        {
            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_black, new(2, 1), 0));
            _board.PlayMove(new Move(_black, new(1, 2), 0));

            string? result = _board.PlayMove(new Move(_white, new(1, 1), 0));
            bool resultBool = result == null;

            Assert.IsFalse(resultBool);
        }
    }

}
