using BibliothequeFonctionsDeBase;

namespace Bibliotheque.LibData
{
    public abstract class Humans
    {
        private Guid _guid = Guid.Empty;
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        private string _lastName = "?";
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        private string _firstName = "?";
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public Humans(Guid guid, string lastName, string firstName)
        {
            Guid = guid;
            LastName = lastName;
            FirstName = firstName;
        }

        public virtual bool IsValid(string[] fields, out string? error)
        {
            error = null;

            if (fields == null || fields.Length < 3)
            {
                error = "Informations insuffisantes sur la personne.";
                return false;
            }

            string guid = fields[0].Trim();
            string lastName = fields[1].Trim();
            string firstName = fields[2].Trim();

            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(guid))
            {
                error = $"Le nom, prénom et/ou l'identifiant d'une personne ne peut être vide.";
                return false;
            }


            return true;
        }
        protected virtual void Afficher()
        {
            FonctionsDeBase.AfficherLigne($"--------------------------------------------------", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"{FirstName} {LastName}", ConsoleColor.Yellow);
            FonctionsDeBase.AfficherLigne($"GUID : {Guid}", ConsoleColor.Magenta);
            FonctionsDeBase.AfficherLigne($"--------------------------------------------------", ConsoleColor.Yellow);
        }
    }
}
