using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoGame.Model;
using GoGame.Model.Enums;
using GoGame.Service;

namespace Tests
{
    [TestClass]
    public class LibertyTests
    {
        private Board _board;
        private Player _black;

        [TestInitialize]
        public void Setup()
        {
            _board = new Board(BoardSize.Small9);
            _black = new Player("Black", StoneColor.Black);
        }

        [TestMethod]
        public void Group_WithLiberty_ReturnsTrue()
        {
            _board.PlayMove(new Move(_black, new(4, 4), 0));

            bool hasLiberty = BFSService.HasLiberties(_board.Grid, new(4, 4));

            Assert.IsTrue(hasLiberty);
        }

        [TestMethod]
        public void Group_WithoutLiberty_ReturnsFalse()
        {
            Player white = new Player("White", StoneColor.White);

            _board.PlayMove(new Move(_black, new(1, 1), 0));

            _board.PlayMove(new Move(white, new(1, 0), 0));
            _board.PlayMove(new Move(white, new(0, 1), 0));
            _board.PlayMove(new Move(white, new(2, 1), 0));
            _board.PlayMove(new Move(white, new(1, 2), 0));

            bool hasLiberty = BFSService.HasLiberties(_board.Grid, new(1, 1));

            Assert.IsFalse(hasLiberty);
        }
    }
}