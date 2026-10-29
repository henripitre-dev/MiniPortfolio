using Bibliotheque.LibService;
using BibliothequeFonctionsDeBase;
using System.Text.RegularExpressions;

namespace Bibliotheque.LibData
{
    public class Employee : Humans
    {
        #region Instanciations des propriétés & Constructeur

        private string _position = "?";
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private string _phone = "XXX-XXX-XXXX";
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public Employee(Guid guid, string lastName, string firstName, string position, string phone)
            : base(guid, lastName, firstName)
        {
            Position = position;
            Phone = phone;
        }

        #endregion

        #region Parsing & Csv handling

        /// <summary>
        /// Converts raw fields into a Members object
        /// </summary>
        /// <param name="fields">raw fields (data)</param>
        /// <param name="error">reason of error, if encountered. else null
        ///     </param>
        /// <returns>result if ok, else null</returns>
        public static Employee Parse(string[] fields)
        {
            Employee e;
            switch (Guid.TryParse(fields[0], out Guid g))
            {
                case true: //Guid is in fields and parsed
                    e = new(g, fields[1], fields[2], fields[3], fields[4]);
                    break;
                default: //Guid doesnt exist yet or is invalid
                    g = Guid.NewGuid();
                    if (fields.Length == 5) //Guid is there but invalid. create new guid and parse the rest
                        goto case true;
                    else //Assume guid is missing, parse the rest
                        e = new(g, fields[0], fields[1], fields[2], fields[3]);
                    break;
            }
            return e;
        }

        /// <summary>
        /// Adds an employee to the CSV file.
        /// </summary>
        /// <param name="e">employee to add</param>
        /// <param name="filepath">Humans filepath</param>
        /// <param name="header">startline of the employee section</param>
        /// <returns></returns>
        public static bool AjouterEmploye(Employee e, string filepath, string header)
        {
            const string section = "employees";

            string line = $"{Guid.NewGuid()},{e.LastName},{e.FirstName},{e.Position},{e.Phone}";
            FileWriter.EcrireLigne(header, filepath, section, line);
            return true;
        }

        /// <summary>
        /// Deletes an employee from the CSV file based on their GUID.
        /// </summary>
        /// <param name="guid">employee guid</param>
        /// <param name="filepath">humans filepath</param>
        /// <returns>true if success</returns>
        public static bool SupprimerEmploye(Guid guid, string filepath)
        {
            const string section = "employees";
            return FileWriter.Supprimer(filepath, section, guid);
        }

        #endregion

        #region Validation checks
        public override bool IsValid(string[] fields, out string? error)
        {
            if (!base.IsValid(fields, out error))
                return false;

            if (fields.Length != 5)
            {
                error = "L'employé doit contenir 5 champs.";
                return false;
            }

            if (!IsValidPhone(fields[4].Trim()))
            {
                error = "Téléphone de l'employé invalide.";
                return false;
            }

            return true;
        }

        public static bool IsValidPhone(string phone)
        {
            return !Regex.IsMatch(phone, @"^0\d{9}$"); // French phone format
        }

        #endregion

        #region Displays methods

        public static void AfficherEmployes(List<Employee> employees)
        {
            FonctionsDeBase.AfficherLigne("--------------------------------------------------------------------", ConsoleColor.Cyan);
            foreach (Employee e in employees)
                e.Afficher();
            FonctionsDeBase.AfficherLigne("--------------------------------------------------------------------", ConsoleColor.Cyan);

        }

        protected override void Afficher()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{FirstName} {LastName}" + $" ({Position}) | ", ConsoleColor.Yellow);
            Console.ResetColor();
            FonctionsDeBase.AfficherLigne($"{Guid}", ConsoleColor.Magenta);
        }

        #endregion

    }
}
