using Bibliotheque.LibData;
using BibliothequeFonctionsDeBase;

namespace Bibliotheque.LibModel
{
    internal class Displays
    {
        #region "Show And Choose" methods
        public static int ShowChooseSection(List<string> sectionChoices, string titleName, bool clearBefore = true)
        {
            int choice;
            do
            {
                if (clearBefore) Console.Clear();
                DisplayTitle(titleName);
                DisplayLineChoices(sectionChoices);

                Console.WriteLine();
                Console.Write("Choice : ");
                choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                choice -= '0'; // Convert char to int
            } while (choice < 1 || choice > sectionChoices.Count);
            return choice;
        }

        #endregion

        #region Aesthetics
        public static void DisplayTitle(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.WriteLine($"   {title}");
            Console.WriteLine("=====================================\n");
            Console.ResetColor();
        }

        public static void DisplayTitle(string title, bool clear = false)
        {
            if (clear) Console.Clear();
            DisplayTitle(title);
        }
        /// <summary>
        /// Displays a loading effect with a message and animated dots.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="c"></param>
        public static void LoadingEffect(string message, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.Write($"\n{message}");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.ResetColor();
            Console.WriteLine("\n");
        }

        public static void WaitKey()
        {
            FonctionsDeBase.AfficherTexteProgressif("\nAppuyez sur une touche pour continuer...", ConsoleColor.DarkGray);
            Console.ReadKey();
            Console.WriteLine();
        }

        #endregion

        #region System / Utility prints

        /// <summary>
        /// Shows an error message in red color in the console.
        /// </summary>
        /// <param name="errorCode">msg to show</param>
        public static void ShowError(string errorCode)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nErreur - {errorCode}\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Outputs a list of choices in the console, each prefixed with its
        /// index number.
        /// </summary>
        public static void DisplayLineChoices(List<string> choices)
        {
            foreach (string choice in choices)
                Console.WriteLine($"{choices.IndexOf(choice) + 1} " + " - " + choice);
        }

        /// <summary>
        /// Show general stats about the library, including name, address,
        /// phone, number of books loaded, file paths, authors, and creation
        /// date.
        /// </summary>
        /// >
        internal static void DisplayLibStats(string libName, string filepathBooks, string filepathHumans, List<Livre> livres)
        {
            DisplayTitle("Library Stats", true);
            FonctionsDeBase.AfficherLigne($"Library name :   \t{libName}", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"Library Address :\t123, rue des Bouquins, Saguenay, QC G7H 0A1", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"Library Phone:   \t(418) 555-0142\n", ConsoleColor.Yellow);

            FonctionsDeBase.AfficherLigne($"Books loaded :   \t{livres.Count}", ConsoleColor.DarkMagenta);
            FonctionsDeBase.AfficherLigne($"BOOKS Path:      \t{filepathBooks}", ConsoleColor.Gray);
            FonctionsDeBase.AfficherLigne($"HUMANS Path:     \t{filepathHumans}", ConsoleColor.Gray);

            Console.Write("\n\nProject by :\n");
            FonctionsDeBase.AfficherTexteProgressif("\tTOMMY-FRÉDÉRIC BERNAQUEZ,\n\tHENRI PITRE", ConsoleColor.Blue);
            FonctionsDeBase.AfficherLigne("\nCreation date: October 2025", ConsoleColor.DarkYellow);
            Displays.WaitKey();
        }
        #endregion
    }
}
