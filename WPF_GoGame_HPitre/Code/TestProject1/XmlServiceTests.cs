using GoGame.Model;
using GoGame.Model.Enums;
using GoGame.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Tests
{
    [TestClass]
    public class XmlServiceTests
    {
        [TestMethod]
        public void SaveGame_CreatesFile()
        {
            Player black = new Player("Black", StoneColor.Black);
            Player white = new Player("White", StoneColor.White);

            Game game = new Game(black, white, BoardSize.Small9, 0, 0);
            game.EndGame();

            string path = XMLService.ExportGameResults(game);

            Assert.IsTrue(File.Exists(path));
        }
    }
}