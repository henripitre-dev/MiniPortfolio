using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoGame.Model;
using GoGame.Model.Enums;

namespace Tests
{
    [TestClass]
    public class CaptureTests
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
        public void Capture_SingleStone_RemovesStone()
        {
            // White stone
            _board.PlayMove(new Move(_white, new(1, 1), 0));

            // Surround with black
            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_black, new(2, 1), 0));

            // Last liberty removed here
            _board.PlayMove(new Move(_black, new(1, 2), 0));

            Assert.IsNull(_board.Grid[1, 1].Stone);
        }

        [TestMethod]
        public void Capture_Group_RemovesEntireGroup()
        {
            // White group
            _board.PlayMove(new Move(_white, new(1, 1), 0));
            _board.PlayMove(new Move(_white, new(1, 2), 0));

            // Surround with black
            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_black, new(0, 2), 0));

            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_black, new(1, 3), 0));

            _board.PlayMove(new Move(_black, new(2, 1), 0));

            // Final liberty removed
            _board.PlayMove(new Move(_black, new(2, 2), 0));

            Assert.IsNull(_board.Grid[1, 1].Stone);
            Assert.IsNull(_board.Grid[1, 2].Stone);
        }

        [TestMethod]
        public void Capture_DoesNotRemove_WhenLibertyExists()
        {
            _board.PlayMove(new Move(_white, new(1, 1), 0));

            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_black, new(2, 1), 0));

            // One liberty still open at (1,2)

            Assert.IsNotNull(_board.Grid[1, 1].Stone);
        }

        [TestMethod]
        public void MultiCapture_RemovesMultipleGroups()
        {
            // White group 1
            _board.PlayMove(new Move(_white, new(1, 1), 0));

            // White group 2
            _board.PlayMove(new Move(_white, new(3, 1), 0));

            // Surround first
            _board.PlayMove(new Move(_black, new(1, 0), 0));
            _board.PlayMove(new Move(_black, new(0, 1), 0));
            _board.PlayMove(new Move(_black, new(2, 1), 0));
            _board.PlayMove(new Move(_black, new(1, 2), 0));

            // Surround second
            _board.PlayMove(new Move(_black, new(3, 0), 0));
            _board.PlayMove(new Move(_black, new(2, 1), 0));
            _board.PlayMove(new Move(_black, new(4, 1), 0));
            _board.PlayMove(new Move(_black, new(3, 2), 0));

            Assert.IsNull(_board.Grid[1, 1].Stone);
            Assert.IsNull(_board.Grid[3, 1].Stone);
        }

        [TestMethod]
        public void Capture_UpdatesBoardCorrectly()
        {
            _board.PlayMove(new Move(_white, new(4, 4), 0));

            _board.PlayMove(new Move(_black, new(4, 3), 0));
            _board.PlayMove(new Move(_black, new(3, 4), 0));
            _board.PlayMove(new Move(_black, new(5, 4), 0));
            _board.PlayMove(new Move(_black, new(4, 5), 0));

            Assert.IsNull(_board.Grid[4, 4].Stone);

            Assert.IsNotNull(_board.Grid[4, 3].Stone);
            Assert.IsNotNull(_board.Grid[3, 4].Stone);
            Assert.IsNotNull(_board.Grid[5, 4].Stone);
            Assert.IsNotNull(_board.Grid[4, 5].Stone);
        }
    }
}