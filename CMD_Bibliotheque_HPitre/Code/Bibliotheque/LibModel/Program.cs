using Bibliotheque.LibData;
using Bibliotheque.LibService;
using BibliothequeFonctionsDeBase;
using System.Globalization;

namespace Bibliotheque.LibModel
{
    internal class Program
    {
        #region Constants, Properties & Global Lists
        public static class StartLines
        {
            public static readonly string[] All = [books, members, employees, authors];

            //BOOKS HEADER LINES (Should be one only)
            public const string books = "guid,title,author,genre,pubDate,isbn,bookAddon,isavailable";

            //START OF HUMANS HEADER LINES
            public const string members = "guid,lname,fname,phone";
            public const string employees = "guid,lastname,name,position,phone";
            public const string authors = "guid,lastname,name";
        }

        // Fichiers de données copiés à côté de l'exécutable (voir Bibliotheque.csproj)
        static string FilepathBooks { get; }
            = Path.Combine(AppContext.BaseDirectory, "LibService", "files", "books.csv");
        static string FilepathHumans { get; }
            = Path.Combine(AppContext.BaseDirectory, "LibService", "files", "humans.csv");

        //Lists to hold data in memory
        public static List<Members> membres = [];
        public static List<Author> auteurs = [];
        public static List<Employee> employes = [];
        public static List<Livre> livres = [];


        #endregion
        static void Main()
        {
            string? errorReason = null;

            bool keepGoing = true;
            while (keepGoing)
            {

                Console.Clear();
                Console.ResetColor();



                #region Loading books & humans lists from files
                //Load data from files into the Global lists ^
                if (LoadLists(FileReader.ConvertLinesToDataArrayList(FilepathBooks),
                              FileReader.ConvertLinesToDataArrayList(FilepathHumans)))
                {
                    //Show initial menu, and ask for input
                    int input = Displays.ShowChooseSection(["Books Menu...", "Authors Menu...", "Members Menu...", "Employees Menu...", "Library Info", "Quit"], "Library Management");
                    switch (input)
                    {
                        case 1:
                            try
                            { BookHandler(); }
                            catch (Exception ex) { keepGoing = false; errorReason = ex.ToString(); }
                            break;

                        case 2:

                            try
                            { AuthorHandler(); }
                            catch (Exception ex) { keepGoing = false; errorReason = ex.ToString(); }
                            break;

                        case 3:
                            try
                            { MemberHandler(); }
                            catch (Exception ex) { keepGoing = false; errorReason = ex.ToString(); }
                            break;

                        case 4:
                            try
                            { EmployeeHandler(); }
                            catch (Exception ex) { keepGoing = false; errorReason = ex.ToString(); }
                            break;

                        case 5: //Display lib info
                            Displays.DisplayLibStats("Project 2 - Library Management", FilepathBooks, FilepathHumans, livres);
                            break;
                        default: //Exit Prg (When input is the last option (6))
                            keepGoing = false;
                            break;
                    }
                }
                else //Else means loading failed
                {
                    keepGoing = false;
                    errorReason = "Critical error while loading data files. Please check files integrity and restart the program.";
                }
                #endregion


            }




            //Program end
            if (!string.IsNullOrWhiteSpace(errorReason))
                Displays.ShowError(errorReason);


            Displays.LoadingEffect("PROGRAM END", ConsoleColor.Red);
            FonctionsDeBase.MasquerTexte();
        }

        #region Loading from files into local lists

        /// <summary>
        /// Vérifie que les fichiers sont valides, puis charge les données dans
        /// les listes correspondantes
        /// </summary>
        /// <param name="bookFields">Données livres</param>
        /// <param name="humanFields">Données des 3 types d'humains</param>
        /// <returns>True if success, else false. Lists are updated using their
        ///     portée</returns>
        private static bool LoadLists(List<string[]>? bookFields, List<string[]>? humanFields)
        {
            bool firstLoad = livres.Count == 0 && membres.Count == 0 && auteurs.Count == 0 && employes.Count == 0;
            bool success = true;
            bool perfectLoad = true;
            if (FileReader.IsValidFile(FilepathBooks, out string? errorReason))
            {
                int nbErrors = 0;

                //Load books
                if (bookFields != null)
                    foreach (string[] fields in bookFields)
                    {
                        if (FileReader.IsHeader(fields))
                            continue;
                        Livre l = Livre.Parse(fields);
                        if (FileReader.IsValidBook(fields, out errorReason) && !FileReader.IsHeader(fields))
                            if (!livres.Any(x => x.Guid == l.Guid))
                                livres.Add(l);
                            else continue;
                        else
                        {
                            FonctionsDeBase.AfficherLigne($"BBOK LINE #{bookFields.IndexOf(fields) + 3} - " + errorReason ?? "UNKNOWN BOOK ERROR", ConsoleColor.Red);
                            nbErrors++;
                            perfectLoad = false;
                        }
                    }
                else
                {
                    success = false;
                    FonctionsDeBase.AfficherTexteProgressif($"No books have been loaded.\n", ConsoleColor.Red);
                }


                if (firstLoad)
                    if (nbErrors > 0 && bookFields != null)
                        FonctionsDeBase.AfficherTexteProgressif($"{livres.Count} books over {bookFields.Count - 1} total have been loaded.\n", ConsoleColor.DarkYellow); //-1 to remove headerLine from count
                    else
                        FonctionsDeBase.AfficherLigne($"All {livres.Count} books have been loaded.", ConsoleColor.Green);

                //Load humans
                if (humanFields != null)
                {
                    nbErrors = 0;
                    foreach (string[] fields in humanFields)
                    {
                        if (FileReader.IsHeader(fields))
                            continue;
                        if (FileReader.IsValidHuman(fields, out errorReason, out char? validType) && !FileReader.IsHeader(fields))
                        {
                            switch (validType)
                            {
                                case 'a': //field represents an Author, if it doesnt exist: add to list.
                                    Author a = Author.Parse(fields);
                                    if (!auteurs.Any(x => x.Guid == a.Guid))
                                        auteurs.Add(a);
                                    break;
                                case 'm': //field represents a Members, if it doesnt exist: add to list.
                                    Members m = Members.Parse(fields);
                                    if (!membres.Any(x => x.Guid == m.Guid))
                                        membres.Add(m);
                                    break;
                                case 'e': //field represents an employee, if it doesnt exist: add to list.
                                    Employee e = Employee.Parse(fields);
                                    if (!membres.Any(x => x.Guid == e.Guid))
                                        employes.Add(e);
                                    break;
                                default: //type Error handler
                                    nbErrors++;
                                    FonctionsDeBase.AfficherLigne($"HUMAN LINE #{humanFields.IndexOf(fields) + 3} - TYPE UNKNOWN", ConsoleColor.Red);
                                    perfectLoad = false;
                                    break;
                            }
                        }
                        else //Human is invalid
                        {
                            FonctionsDeBase.AfficherLigne($"HUMAN LINE #{humanFields.IndexOf(fields) + 3} - " + errorReason ?? "UNKNOWN HUMAN ERROR", ConsoleColor.Red);
                            nbErrors++;
                            perfectLoad = false;
                        }
                    }
                    if (firstLoad)
                        if (nbErrors > 0)
                            FonctionsDeBase.AfficherTexteProgressif($"{membres.Count + auteurs.Count + employes.Count} humans on {humanFields.Count} total have been loaded.\n", ConsoleColor.DarkYellow);
                        else
                            FonctionsDeBase.AfficherLigne($"All humans have been loaded.", ConsoleColor.Green);
                }
                else
                {
                    success = false;
                    FonctionsDeBase.AfficherTexteProgressif($"No humans have loaded succesfully.\n", ConsoleColor.Red);
                }
                if (!perfectLoad) Displays.WaitKey();
            }
            return success;
        }

        #endregion

        #region Book Management Menus/Submenus
        /// <summary>
        /// Book main menu
        /// </summary>
        private static void BookHandler()
        {
            int input = Displays.ShowChooseSection(["Add a book", "Remove a book", "Searching...", "List available books", "Loans...", "Returns"], "Book Handler");

            switch (input)
            {
                case 1:
                    Displays.DisplayTitle("Ajouter un livre", true);
                    string[] data = FileReader.ReadLineToFields(FileReader.CsvTypes.books);

                    // Parse the data into a Livre object
                    Livre l = Livre.Parse(data);

                    // Ajouter le livre (le GUID sera généré dans cette méthode)
                    bool success = Livre.AjouterLivre(l, FilepathBooks, Program.StartLines.books);
                    FonctionsDeBase.AfficherLigne(
                        success ? "Book added succesfully." : "Book addition failed.",
                        success ? ConsoleColor.Green : ConsoleColor.Red);
                    Displays.WaitKey();
                    break;
                case 2:
                    Displays.DisplayTitle("Book Deletion", true);
                    string bookGuidTxt = FonctionsDeBase.LireTexteNonVide("Input book GUID : ");

                    if (Guid.TryParse(bookGuidTxt, out Guid guid))
                    {
                        success = Livre.SupprimerLivre(guid, FilepathBooks);
                        FonctionsDeBase.AfficherLigne(
                        success ? "Book deleted succesfully." : "Book deletion failed.",
                        success ? ConsoleColor.Green : ConsoleColor.Red);
                    }
                    else
                        Displays.ShowError("Invalid GUID format.");
                    Displays.WaitKey();
                    break;
                case 3: //Recherche de livres
                    SearchHandler();
                    break;
                case 4:
                    Displays.DisplayTitle("List available books", true);
                    Livre.AfficherLivresDisponibles(livres);
                    break;
                case 5: //Gestion des emprunts
                    LoanHandler();
                    break;
                case 6:
                    return;
                default:
                    Displays.ShowError("Invalid Choice !");
                    break;
            }
        }

        /// <summary>
        /// Book searching submenu
        /// </summary>
        private static void SearchHandler()
        {
            int choix = Displays.ShowChooseSection(["By ID", "By Author", "By title", "Return"], "Book Search");
            List<int> results;
            switch (choix)
            {
                case 1:
                    string bookGuidTxt = FonctionsDeBase.LireTexteNonVide($"\nInput book GUID : ");

                    if (Guid.TryParse(bookGuidTxt, out Guid guid))
                    {
                        int? result = Livre.RechercherLivre(livres, guid);
                        if (result != null)
                            Livre.AfficherLivre(livres[(int)result]);
                        else
                            Displays.ShowError("No books found.");
                    }
                    else
                        Console.WriteLine("Invalid GUID format.");
                    break;

                case 2:
                    string bookAuthTxt = FonctionsDeBase.LireTexteNonVide("\nInput book author : ");
                    results = Livre.RechercherLivreAuteur(livres, bookAuthTxt);
                    if (results.Count != 0)
                        foreach (int idx in results)
                            Livre.AfficherLivre(livres[idx]);
                    else
                        Displays.ShowError("No books found.");
                    break;
                case 3:
                    string bookTitleTxt = FonctionsDeBase.LireTexteNonVide("\nInput book title : ");
                    results = Livre.RechercherLivre(livres, bookTitleTxt);
                    if (results.Count != 0)
                        foreach (int idx in results)
                            Livre.AfficherLivre(livres[idx]);
                    else
                        Displays.ShowError("No books found.");
                    break;
                default: return;
            }
            Displays.WaitKey();
        }

        /// <summary>
        /// Book loaning/returning/searching submenu
        /// </summary>
        private static void LoanHandler()
        {
            int choice = Displays.ShowChooseSection(["Loan a book", "Return a book", "List lent books", "Return"], "Loan Handler");
            switch (choice)
            {
                case 1: //Emrpunter livre
                    Displays.DisplayTitle("Book loaning", true);
                    string bookGuidTxt = FonctionsDeBase.LireTexteNonVide("Input book GUID : ").Trim();
                    if (Guid.TryParse(bookGuidTxt, out Guid bookGuid))
                    {
                        if (!Livre.InvertAvailability(livres, bookGuid, FilepathBooks, out string? error))
                            if (error != null) Displays.ShowError(error);
                    }
                    else Displays.ShowError("Invalid GUID format.");
                    break;
                case 2: //Retourner livre
                    Displays.DisplayTitle("Book return", true);
                    bookGuidTxt = FonctionsDeBase.LireTexteNonVide("Input book GUID : ").Trim();
                    if (Guid.TryParse(bookGuidTxt, out bookGuid))
                    {
                        if (!Livre.InvertAvailability(livres, bookGuid, FilepathBooks, out string? error))
                            if (error != null) Displays.ShowError(error);
                    }
                    else Displays.ShowError("Invalid GUID format.");
                    break;
                case 3: //Afficher livres prêtés
                    Displays.DisplayTitle("List available books", true);
                    Livre.AfficherLivresIndisponibles(livres);
                    break;
                case 4: return;
            }
        }

        #endregion

        #region Humand Handler Menus
        /// <summary>
        /// Author management menu
        /// </summary>
        private static void AuthorHandler()
        {
            int choice = Displays.ShowChooseSection(["Add an author", "Remove an author", "List Authors", "Return"], "Author Handler");
            switch (choice)
            {
                case 1:
                    Console.Clear();

                    Displays.DisplayTitle("Author addition");

                    // Diviser en champs (séparés par virgule)
                    string[] data = FileReader.ReadLineToFields(FileReader.CsvTypes.author);

                    // Créer l'auteur SANS le GUID (sera généré automatiquement)
                    Author a = Author.Parse(data);
                    // Ajouter l'auteur (le GUID sera généré dans cette méthode)
                    bool success = Author.AjouterAuteur(a, FilepathHumans, StartLines.authors);
                    FonctionsDeBase.AfficherLigne(
                        success ? "Author added successfully." : "Author addition failed.",
                        success ? ConsoleColor.Green : ConsoleColor.Red
                    );
                    break;

                case 2:
                    Console.Clear();
                    Displays.DisplayTitle("Author Deletion");

                    string guidInput = FonctionsDeBase.LireTexteNonVide("Input author ID : ");

                    if (Guid.TryParse(guidInput, out Guid guid))
                    {
                        bool deleteSuccess = Author.SupprimerAuteur(guid, FilepathHumans);
                        FonctionsDeBase.AfficherLigne(
                            deleteSuccess ? "Author deleted successfully." : "Author deletion failed..",
                            deleteSuccess ? ConsoleColor.Green : ConsoleColor.Red
                        );
                    }
                    else
                    {
                        Displays.ShowError("Invalid GUID Format.");
                    }
                    break;
                case 3:
                    Author.AfficherAuteurs(auteurs);
                    break;
                case 4: return;
                default: Displays.ShowError("Invalid choice !"); break;
            }
        }

        /// <summary>
        /// Member management menu
        /// </summary>
        static void MemberHandler()
        {
            List<string> menuChoices = ["Add a member", "Remove a member", "List members", "Member Search...", "Return"];
            int input = Displays.ShowChooseSection(menuChoices, "Member Handler");

            switch (input)
            {
                case 1:
                    Displays.DisplayTitle("Adding a Member", true);
                    Console.WriteLine("Input member info : (LName, FName, Phone)");
                    string[] data = FileReader.ReadLineToFields(FileReader.CsvTypes.members);

                    Members m = Members.Parse(data);
                    membres.Add(m);
                    bool success = Members.AjouterMembre(m, FilepathHumans, StartLines.members);
                    FonctionsDeBase.AfficherLigne(success ? "Member added succesfully" : "Member deletion failed.", success ? ConsoleColor.Green : ConsoleColor.Red);
                    break;

                case 2:
                    Displays.DisplayTitle("Member deletion", true);

                    string guidInput = FonctionsDeBase.LireTexteNonVide("Input author ID : ");
                    if (Guid.TryParse(guidInput, out Guid guid))
                    {
                        success = Members.SupprimerMembre(guid, FilepathHumans);
                        FonctionsDeBase.AfficherLigne(success ? "Member deleted succesfully" : "Member deletion failed.", success ? ConsoleColor.Green : ConsoleColor.Red);
                    }
                    else
                        Console.WriteLine("Invalid GUID Format !");
                    break;
                case 3:
                    Displays.DisplayTitle("Members list", true);
                    Members.AfficherMembres(membres);
                    Displays.WaitKey();
                    break;
                case 4:
                    int choix = Displays.ShowChooseSection(["By ID", "By Name", "Return"], "Member Search");
                    int? result;

                    switch (choix)
                    {
                        case 1:
                            string MembersGuidTxt = FonctionsDeBase.LireTexteNonVide("Input Member ID : ");

                            if (Guid.TryParse(MembersGuidTxt, out Guid mGuid))
                            {
                                result = Members.RechercherMembre(membres, mGuid);
                                if (result != null)
                                    Members.AfficherMembre(membres[result.Value]);
                                else Displays.ShowError("Member not found.");
                            }
                            else
                                Console.WriteLine("Invalid GUID Format.");
                            break;

                        case 2:
                            string MembersNameTxt = FonctionsDeBase.LireTexteNonVide("Input Member Name : ");

                            List<int> results = Members.RechercherMembre(membres, MembersNameTxt);
                            if (results != null)
                                foreach (int idx in results)
                                    Members.AfficherMembre(membres[idx]);

                            else
                                Displays.ShowError("No members found.");
                            break;
                        default:
                            return;
                    }
                    break;
            }
        }


        /// <summary>
        /// Employee management menu
        /// </summary>
        private static void EmployeeHandler()
        {
            List<string> menuChoices = ["Add an employee", "Remove an employee", "List Employees' GUIDs", "Return"];
            int input = Displays.ShowChooseSection(menuChoices, "Employee Handler");

            switch (input)
            {
                case 1:
                    Displays.DisplayTitle("Add an employee", true);
                    Console.WriteLine("Input employee info: (LName, FName, PostName, Phone)");

                    string[] data = FileReader.ReadLineToFields(FileReader.CsvTypes.employees);

                    //Ajouter le membre à la liste et au fichier
                    Employee m = Employee.Parse(data);
                    employes.Add(m);
                    bool success = Employee.AjouterEmploye(m, FilepathHumans, StartLines.employees);
                    FonctionsDeBase.AfficherLigne(success ? "Employee added succesfully" : "Employee addition failed.", success ? ConsoleColor.Green : ConsoleColor.Red);
                    Displays.WaitKey();
                    break;

                case 2:
                    Displays.DisplayTitle("Remove an employee", true);
                    Console.WriteLine("Input employee ID");

                    if (Guid.TryParse(FonctionsDeBase.LireTexteNonVide("GUID : "), out Guid guid))
                    {
                        success = Employee.SupprimerEmploye(guid, FilepathHumans);
                        FonctionsDeBase.AfficherLigne(success ? "Employee deleted successfully." : "Employee deletion failed.", success ? ConsoleColor.Green : ConsoleColor.Red);
                        Displays.WaitKey();
                    }
                    else
                        Displays.ShowError("Invalid GUID format.");
                    break;
                case 3:
                    Employee.AfficherEmployes(employes);
                    Displays.WaitKey();
                    break;
                case 4: return;
                default: Displays.ShowError("Invalid Choice !"); break;
            }
        }

        #endregion

        #region String Comparison Checker
        /// <summary>
        /// Checks wether a string is the same as another, ignoring case and
        /// accents
        /// </summary>
        /// <param name="source">comparer</param>
        /// <param name="toCheck">compared</param>
        /// <returns>true if theyre the same</returns>
        public static bool ContainsInsensitive(string source, string toCheck)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(toCheck))
                return false;

            CompareInfo compareInfo = CultureInfo.GetCultureInfo("fr-FR").CompareInfo;
            return compareInfo.IndexOf(source, toCheck,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
        }
        #endregion
    }
}
