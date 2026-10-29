using System.Text.RegularExpressions;

namespace Bibliotheque.LibData
{
    internal class Member : Humans
    {
        private string _phone = "XXX-XXX-XXXX";
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public Member(Guid guid, string lastName, string firstName, string phone) : base(guid, lastName, firstName)
        {
            Phone = phone;
        }

        /// <summary>
        /// Converts raw fields into a member object
        /// </summary>
        /// <param name="fields">raw fields (data)</param>
        /// <param name="error">reason of error, if encountered. else null
        ///     </param>
        /// <returns>result if ok, else null</returns>
        public static Member Parse(string[] fields)
        {
            return new Member(Guid.Parse(fields[0]), fields[1], fields[2], fields[3]);
        }

        /// <summary>
        /// Verifies fields is a valid Livre object, then converts it
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public Member? TryParse(string[] fields, out string? error)
        {
            Member? m = null;
            if (IsValid(fields, out error))
                m = Parse(fields);

            return m;
        }

        public override bool IsValid(string[] fields, out string? error)
        {
            if (!base.IsValid(fields, out error))
                return false;

            if (fields.Length != 4)
            {
                error = "Le membre doit contenir 4 champs.";
                return false;
            }

            if (!IsValidPhone(fields[3]))
            {
                error = "Téléphone du membre invalide.";
                return false;
            }

            return true;
        }

        public static bool IsValidPhone(string phone)
        {
            return !Regex.IsMatch(phone, @"^0\d{9}$"); // French phone format
        }


    }
}
