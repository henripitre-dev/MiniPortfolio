using Bibliotheque.LibModel;
using Bibliotheque.LibService;
using BibliothequeFonctionsDeBase;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProjetdeTest")]

namespace Bibliotheque.LibData
{
    /// <summary>
    /// Humans subclass representing an author.
    /// </summary>
    internal class Author(Guid guid, string lastName, string firstName) : Humans(guid, lastName, firstName)
    {

        #region Parsing and CSV handling

        /// <summary>
        /// Converts raw fields into a Members object
        /// </summary>
        /// <param name="fields">raw fields (data)</param>
        /// <param name="error">reason of error, if encountered. else null
        ///     </param>
        /// <returns>result if ok, else null</returns>
        public static Author Parse(string[] fields)
        {
            Author a;
            switch (Guid.TryParse(fields[0], out Guid g))
            {
                case true: //Guid is in fields and parsed
                    a = new(g, fields[1], fields[2]);
                    break;
                default: //Guid doesnt exist yet or is invalid
                    g = Guid.NewGuid();
                    if (fields.Length == 3) //Guid is there but invalid. dont load this book
                        goto case true;
                    else //Assume guid is missing, parse the rest
                        a = new(g, fields[0], fields[1]);
                    break;
            }
            return a;
        }

        /// <summary>
        /// Ajoute un nouvel auteur dans le fichier CSV correspondant.
        /// </summary>
        /// <param name="a">The author to add</param>
        /// <param name="filepath">Humans filepath</param>
        /// <param name="header">startline of the author section</param>
        /// <returns>true si réussi</returns>
        public static bool AjouterAuteur(Author a, string filepath, string header)
        {
            const string section = "authors";

            string line = $"{a.Guid},{a.LastName},{a.FirstName}";
            FileWriter.EcrireLigne(header, filepath, section, line);

            FonctionsDeBase.AfficherTexteProgressif($"New author : {a.FirstName} {a.LastName}.\n", ConsoleColor.Green);
            return true;
        }

        /// <summary>
        /// Supprime un auteur du fichier CSV correspondant.
        /// </summary>
        /// <param name="guid">author guid to remove</param>
        /// <param name="filepath">humans filepath</param>
        /// <returns>true si réussi</returns>
        public static bool SupprimerAuteur(Guid guid, string filepath)
        {
            const string section = "authors";
            if (FileWriter.Supprimer(filepath, section, guid))
            {
                FonctionsDeBase.AfficherTexteProgressif($"Deleted author succesfully.", ConsoleColor.Green);
                return true;
            }
            else
            {
                FonctionsDeBase.AfficherTexteProgressif($"Deletion failed.", ConsoleColor.Red);
                Displays.WaitKey();
                return false;
            }
        }
        #endregion

        #region Display methods

        public static void AfficherAuteurs(List<Author> authors)
        {
            foreach (Author a in authors)
                a.Afficher();
            Displays.WaitKey();
        }
        /// <summary>
        /// Uses basic Humans Afficher method
        /// </summary>
        protected override void Afficher() => base.Afficher();

        #endregion
    }
}


