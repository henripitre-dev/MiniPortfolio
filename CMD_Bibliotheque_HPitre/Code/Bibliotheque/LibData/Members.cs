using Bibliotheque.LibService;
using BibliothequeFonctionsDeBase;
using System.Text.RegularExpressions;

namespace Bibliotheque.LibData
{
    internal class Members : Humans
    {
        private string _phone = "XXX-XXX-XXXX";
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public Members(Guid guid, string lastName, string firstName, string phone) : base(guid, lastName, firstName) => Phone = phone;

        /// <summary>
        /// Converts raw fields into a Livre object
        /// </summary>
        /// <param name="fields">raw fields (data)</param>
        /// <param name="error">reason of error, if encountered. else null
        ///     </param>
        /// <returns>result if ok, else null</returns>
        public static Members Parse(string[] fields)
        {
            Members m;
            switch (Guid.TryParse(fields[0], out Guid g))
            {
                case true: //Guid is in fields and parsed
                    m = new(g, fields[1], fields[2], fields[3]);
                    break;
                default: //Guid doesnt exist yet or is invalid
                    g = Guid.NewGuid();
                    if (fields.Length == 4) //Guid is there but invalid. dont load this book
                        goto case true;
                    else //Assume guid is missing, parse the rest
                        m = new(g, fields[0], fields[1], fields[2]);
                    break;
            }
            return m;
        }

        public override bool IsValid(string[] fields, out string? error)
        {
            if (!base.IsValid(fields, out error))
                return false;

            if (fields.Length != 4)
            {
                error = "a Member should have 4 data fields.";
                return false;
            }

            if (!IsValidPhone(fields[3]))
            {
                error = "Invalid phone number.";
                return false;
            }

            return true;
        }

        public static bool IsValidPhone(string phone) => !Regex.IsMatch(phone, @"^0\d{9}$"); //French phone format

        /// <summary>
        /// Ajoute un nouveau membre dans le fichier CSV correspondant.
        /// </summary>
        /// <param name="fields">Tableau contenant les informations du membre.
        ///     </param>
        /// <returns>Vrai si l’enregistrement est réussi, faux sinon.</returns>
        public static bool AjouterMembre(Members m, string filepath, string header)
        {
            const string section = "Members";

            string line = $"{m.Guid},{m.LastName},{m.FirstName},{m.Phone}";
            FileWriter.EcrireLigne(header, filepath, section, line);
            return true;
        }

        public static bool SupprimerMembre(Guid guid, string filepath)
        {
            const string section = "Members";
            return FileWriter.Supprimer(filepath, section, guid);
        }



        //Rechercher un membre par son numéro
        public static int? RechercherMembre(List<Members> Members, Guid guid)
        {
            foreach (Members m in Members)
            {
                if (m.Guid == guid)
                {
                    return Members.IndexOf(m);
                }
            }
            return null;
        }

        //Rechercher un membre par son nom
        public static List<int> RechercherMembre(List<Members> Members, string nom)
        {
            List<int> results = [];
            foreach (Members m in Members)
            {
                if (m.LastName == nom)
                {
                    results.Add(Members.IndexOf(m));
                }
                else if (m.FirstName == nom)
                {
                    results.Add(Members.IndexOf(m));
                }
            }
            return results;
        }

        public static void AfficherMembres(List<Members> members)
        {
            foreach (Members m in members)
                m.Afficher();
        }

        protected override void Afficher()
        {
            FonctionsDeBase.AfficherLigne($"--------------------------------------------------", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"Name: {FirstName} {LastName}", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"Phone : {Phone}", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"GUID : {Guid}", ConsoleColor.Magenta);
            FonctionsDeBase.AfficherLigne($"--------------------------------------------------", ConsoleColor.Yellow);
        }

        internal static void AfficherMembre(Members members)
        {
            members.Afficher();
        }
    }
}
