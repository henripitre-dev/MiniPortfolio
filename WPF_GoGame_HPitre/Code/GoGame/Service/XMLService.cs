using GoGame.Model;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GoGame.Service
{
    public static class XMLService
    {
        private static readonly string SavePath = Path.Combine(Environment.CurrentDirectory, "UNLEASHEDGO_Results.xml");

        public static string ExportGameResults(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));


            if (!game.IsFinished)
                throw new InvalidOperationException("La partie doit être terminée.");
            //Get players and their scores
            Player blackPlayer = game.Players.First(p => p.Color == Model.Enums.StoneColor.Black);
            Player whitePlayer = game.Players.First(p => p.Color == Model.Enums.StoneColor.White);

            int blackIndex = Array.IndexOf(game.Players, blackPlayer);
            int whiteIndex = Array.IndexOf(game.Players, whitePlayer);

            double blackScore = game.Pointages[blackIndex];
            double whiteScore = game.Pointages[whiteIndex];

            //Create XML format for the game results with values
            XElement gameElement =
                new XElement("Game",

                    new XAttribute("Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),

                    new XElement("GameInfo",
                        new XElement("BoardSize", (int)game.Board.Size),
                        new XElement("Komi", game.Komi),
                        new XElement("Handicap", ((int)game.NbHandicaps)),
                        new XElement("TotalMoves", game.NbMoves),
                        new XElement("IsAbandon", game.IsAbandon),
                        new XElement("EndReason",
                            game.IsAbandon
                                ? "Abandon"
                                : "Deux passes consécutives")
                    ),

                    new XElement("Winner",
                        new XElement("Name", game.Winner.Name),
                        new XElement("Color", game.Winner.Color.ToString())
                    ),

                    new XElement("Players",

                        new XElement("Player",
                            new XAttribute("Color", "Black"),

                            new XElement("Name", blackPlayer.Name),
                            new XElement("Score", blackScore),
                            new XElement("Territory", blackScore),
                            new XElement("TimePlayedSeconds", blackPlayer.TimePlayed)
                        ),

                        new XElement("Player",
                            new XAttribute("Color", "White"),

                            new XElement("Name", whitePlayer.Name),
                            new XElement("Score", whiteScore),
                            new XElement("Territory", whiteScore - game.Komi),
                            new XElement("Komi", game.Komi),
                            new XElement("TimePlayedSeconds", whitePlayer.TimePlayed)
                        )
                    ),

                    new XElement("Statistics",
                        new XElement("TotalDurationSeconds",
                            blackPlayer.TimePlayed + whitePlayer.TimePlayed)
                    )
                );

            XDocument document;

            if (File.Exists(SavePath))
                document = XDocument.Load(SavePath);
            else
                document = new(new XElement("Games"));

            document.Root!.Add(gameElement);
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);

            document.Save(SavePath);
            return SavePath;
        }
    }
}