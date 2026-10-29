using Bibliotheque.LibModel;
using Bibliotheque.LibService;
using BibliothequeFonctionsDeBase;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("ProjetdeTest")]


namespace Bibliotheque.LibData
{
    internal class Livre
    {
        #region Instanciation des propriétés et Constructeur

        private Guid _guid = Guid.Empty;
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _author = "";
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }
        private string _genre = "";
        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }
        private DateTime _pubDate = DateTime.MinValue;
        public DateTime PubDate
        {
            get { return _pubDate; }
            set { _pubDate = value; }
        }

        private string _isbn = "";
        public string ISBN
        {
            get { return _isbn; }
            set { _isbn = value; }
        }
        private string? _bookAddon;
        public string? BookAddon
        {
            get { return _bookAddon; }
            set { _bookAddon = value; }
        }
        private bool _isAvailable;
        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { _isAvailable = value; }
        }

        /// <summary>
        /// Constructeur logique de l'objet Livre. Assume que l'objet créé est
        /// valide
        /// </summary>
        public Livre(Guid guid, string title, string author, string genre, DateTime pubDate, string isbn, string? bookAddon, bool isavailable)
        {
            Guid = guid;
            Title = title;
            Author = author;
            Genre = genre;
            PubDate = pubDate;
            ISBN = isbn;
            BookAddon = bookAddon;
            IsAvailable = isavailable;
        }

        #endregion

        #region Validity checks 


        /// <summary>
        /// Verifies that the fields correspond to a valid Livre object, and can
        /// be parsed succesfully
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool IsValid(string[] fields, out string? error)
        {
            error = null;

            if (fields.Length != 8)
            {
                error = "Nombre de champs invalide pour un livre.";
                return false;
            }

            string[] trimmed = [.. fields.Select(f => f.Trim())];

            string guidTxt = trimmed[0],
                    titre = trimmed[1],
                   auteur = trimmed[2],
                   genre = trimmed[3],
                   datePubStr = trimmed[4],
                   isbn = trimmed[5],
                   addon = trimmed[6],
                   disponibleStr = trimmed[7];

            if (string.IsNullOrWhiteSpace(titre) || string.IsNullOrWhiteSpace(auteur) || string.IsNullOrWhiteSpace(guidTxt))
            {
                error = "Le titre, l'auteur et/ou l'identitifiant ne doivent pas être vides.";
                return false;
            }

            if (!Guid.TryParse(guidTxt, out Guid guid))
            {
                error = "GUID invalide.";
                return false;
            }

            if (!DateTime.TryParse(datePubStr, out _))
            {
                error = "Date de publication invalide.";
                return false;
            }

            if (!Regex.IsMatch(isbn, @"^(97(8|9))?\d{9}(\d|X)$"))  // Basic ISBN-10 or ISBN-13
            {
                error = "ISBN invalide (doit être 10 ou 13 chiffres+Prefix).";
                return false;
            }

            if (!bool.TryParse(disponibleStr, out _))
            {
                error = "Valeur de disponibilité invalide (doit être 'true' ou 'false').";
                return false;
            }
            return true;



        }

        #endregion

        #region Parsing and csv handling

        /// <summary>
        /// Converts raw fields into a Livre object
        /// </summary>
        /// <param name="fields">raw fields (data)</param>
        /// <returns>result if ok, else null</returns>
        public static Livre Parse(string[] fields)
        {
            Livre l;
            switch (Guid.TryParse(fields[0], out Guid g))
            {
                case true: //Guid is in fields and parsed
                    l = new(g, fields[1], fields[2], fields[3], DateTime.Parse(fields[4]), fields[5], fields[6], bool.Parse(fields[7].ToLower()));
                    break;
                default: //Guid doesnt exist yet or is invalid
                    g = Guid.NewGuid();
                    if (fields.Length == 8) //Guid is there but invalid. dont load this book
                        goto case true;
                    else //Assume guid is missing, parse the rest
                        l = new(g, fields[0], fields[1], fields[2], DateTime.Parse(fields[3]), fields[4], fields[5], bool.Parse(fields[6].ToLower()));
                    break;
            }
            return l;
        }

        /// <summary>
        /// Adds a book to the CSV file
        /// </summary>
        /// <param name="l">book to add</param>
        /// <param name="filepath">books filepath</param>
        /// <param name="header">csv startline of the books section</param>
        /// <returns></returns>
        public static bool AjouterLivre(Livre l, string filepath, string header)
        {
            const string section = "books";

            // Construction de la ligne CSV à écrire
            string line = $"{l.Guid},{l.Title},{l.Author},{l.Genre},{l.PubDate:yyyy-MM-dd},{l.ISBN},{l.BookAddon},{l.IsAvailable}";

            // Écriture dans le fichier
            FileWriter.EcrireLigne(header, filepath, section, line);
            return true;
        }

        /// <summary>
        /// Delete a book from the CSV file by its guid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool SupprimerLivre(Guid guid, string filepath)
        {
            return FileWriter.Supprimer(filepath, "books", guid);
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Rechercher un livre par son numéro(guid)
        /// </summary>
        /// <param name="livres">Where to search</param>
        /// <param name="guid">guid to search</param>
        /// <returns>the index of the found book, or null</returns>
        public static int? RechercherLivre(List<Livre> livres, Guid guid)
        {
            foreach (Livre livre in livres)
            {
                if (livre.Guid == guid)
                    return livres.IndexOf(livre);
            }
            return null;
        }

        /// <summary>
        /// Search a book by its title (or part of it/ with or without
        /// caps/accents)
        /// </summary>
        /// <param name="livres">Where to search</param>
        /// <param name="titre">what to search</param>
        /// <returns>All the indexes that contain this title, or empty list
        ///     </returns>
        public static List<int> RechercherLivre(List<Livre> livres, string titre)
        {
            List<int> results = [];
            foreach (Livre livre in livres)
            {
                if (Program.ContainsInsensitive(livre.Title, titre)) //Cherche avec ou sans accents, majuscules/minuscules
                    results.Add(livres.IndexOf(livre));
            }
            return results;
        }
        /// <summary>
        /// Search a book by its author (or part of it/ with or without
        /// caps/accents)
        /// </summary>
        /// <param name="livres">Where to search</param>
        /// <param name="auteur">what to search</param>
        /// <returns>All the indexes that contain this author, or empty list
        ///     </returns>
        public static List<int> RechercherLivreAuteur(List<Livre> livres, string auteur)
        {
            List<int> results = [];
            foreach (Livre livre in livres)
            {

                if (Program.ContainsInsensitive(livre.Author, auteur)) //Cherche avec ou sans accents, majuscules/minuscules
                    results.Add(livres.IndexOf(livre));
            }
            return results;
        }

        #endregion

        #region Book Loan methods

        /// <summary>
        /// Change the availability of a book, with textual context
        /// </summary>
        /// <param name="livres">Where to make the change locally</param>
        /// <param name="filepath">where to make the change in files</param>
        /// <param name="error">reason if error encountered (else null)</param>
        /// <returns>true if success</returns>
        public static bool InvertAvailability(List<Livre> livres, Guid bookGuid, string filepath, out string? error)
        {
            bool borrowSuccess = Livre.ChangeLoanState(livres, bookGuid, filepath, out error);
            if (borrowSuccess)
            {
                int bookIdx = (int)RechercherLivre(livres, bookGuid)!;
                if (livres[bookIdx].IsAvailable) //Book is returned
                    FonctionsDeBase.AfficherTexteProgressif($"{DateTime.Today:yyyy-MM-dd} - {livres[bookIdx].Title} has been succesfully returned\n", ConsoleColor.Green);
                else //Books is now borrowed
                    FonctionsDeBase.AfficherTexteProgressif($"{livres[bookIdx].Title} has been succesfully loaned\n", ConsoleColor.Green);
                Displays.WaitKey();
                return true;
            }
            else
                error = "Couldn't change loan state: " + error;
            return false;
        }

        /// <summary>
        /// Switch the loan state of a book (available/borrowed) both in the
        /// local list and in the CSV file
        /// </summary>
        /// <returns>true if success</returns>
        private static bool ChangeLoanState(List<Livre> livres, Guid guid, string filepath, out string? errorReason)
        {
            //sélectionner le livre
            int? bIdx = RechercherLivre(livres, guid);

            if (bIdx != null)
            {
                //modifier l'état de l'objet dans la liste locale
                if (livres[(int)bIdx].IsAvailable)
                    livres[(int)bIdx].IsAvailable = false;
                else
                    livres[(int)bIdx].IsAvailable = true;

                //Remplacer la ligne du csv par une nouvelle contenant le bon état
                errorReason = SupprimerLivre(livres[(int)bIdx].Guid, filepath)
                    ? null
                    : "Couldn't delete old book in CSV";
                errorReason ??= AjouterLivre(livres[(int)bIdx], filepath, Program.StartLines.books)
                    ? null
                    : "Couldn't add new book in CSV";
            }
            else //book not found
                errorReason = "Book has not been found.";
            if (errorReason != null)
                return false;
            return true;
        }

        #endregion

        #region Books display

        /// <summary>
        /// Show all books that are available (or not, depending on the
        /// availableState parameter)
        /// </summary>
        /// <param name="livres">books to show</param>
        /// <param name="availableState">display books that are available ?
        ///     </param>
        /// <returns>true if theres books to show</returns>
        public static bool AfficherLivresDisponibles(List<Livre> livres, bool availableState = true)
        {
            bool livresDisponible = false;
            foreach (Livre livre in livres)
            {
                if (livre.IsAvailable == availableState)
                {
                    Console.WriteLine(livre.Title);
                    livresDisponible = true;
                }
            }
            if (livresDisponible)
                Displays.WaitKey();
            else
                FonctionsDeBase.AfficherTexteProgressif("Aucun livre trouvé.", ConsoleColor.DarkYellow);
            return livresDisponible;
        }
        public static bool AfficherLivresIndisponibles(List<Livre> livres) => AfficherLivresDisponibles(livres, false);

        /// <summary>
        /// Affiche les stats du livre
        /// </summary>
        /// <param name="livre">book to display</param>
        public static void AfficherLivre(Livre livre)
        {
            FonctionsDeBase.AfficherLigne($"---------------------------------------------", ConsoleColor.DarkGray);
            string disponible = "Non disponible";
            if (livre.IsAvailable)
            {
                disponible = "Disponible";
            }
            FonctionsDeBase.AfficherLigne($"{livre.Title},{livre.Author}\n{livre.ISBN}\n{livre.Genre} | {disponible}", ConsoleColor.Yellow);
            if (!string.IsNullOrWhiteSpace(livre.BookAddon))
                FonctionsDeBase.AfficherLigne($"{livre.BookAddon}", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"---------------------------------------------", ConsoleColor.DarkGray);
        }

        #endregion

    }
}
