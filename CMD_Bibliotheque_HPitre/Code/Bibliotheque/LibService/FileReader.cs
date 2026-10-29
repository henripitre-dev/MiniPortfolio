using Bibliotheque.LibData;
using Bibliotheque.LibModel;
using BibliothequeFonctionsDeBase;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace Bibliotheque.LibService
{
    public static class FileReader
    {
        #region Variables et Enums
        //tood: implémenter cet enum
        public enum HumanTypes
        {
            Members,
            employees,
            authors
        }
        public enum CsvTypes
        {
            none,
            books,
            humans,
            author,
            members,
            employees
        }

        #endregion

        #region Direct deserialization of files
        public static List<string[]>? ConvertLinesToDataArrayList(string filepath)
        {
            var dataLines = new List<string[]>();
            bool success = true;
            CsvTypes type = CsvTypes.none;
            int nbHeadersFound = 0;
            int headerLength = 0;

            // Basic checks
            if (string.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath))
                success = false;

            // Determine type from file name
            if (success)
                type = GetCSVTypeFromPath(filepath);

            if (success)
            {
                try
                {
                    using TextFieldParser csvParser = new(filepath)
                    {
                        CommentTokens = ["#"],
                        HasFieldsEnclosedInQuotes = true,  // FIXED
                        TrimWhiteSpace = true
                    };
                    csvParser.SetDelimiters(",");

                    while (!csvParser.EndOfData && success)
                    {
                        string[]? fields = csvParser.ReadFields();
                        if (fields == null || fields.All(string.IsNullOrWhiteSpace))
                            continue;

                        if (fields.Length == 1 && fields[0].StartsWith("###"))
                            continue;

                        if (type == CsvTypes.books)
                        {
                            if (nbHeadersFound <= 0)
                            {
                                if (IsHeader(fields))
                                {
                                    dataLines.Add(fields); //Add the header in datalines
                                    nbHeadersFound++;
                                }
                                else
                                    continue; //Aller à la prochaine ligne, pour voir si le header s'y trouve
                            }
                            else
                            {
                                // Ajouter la ligne de données à la liste
                                dataLines.Add(fields);
                            }
                        }
                        else if (type == CsvTypes.humans)
                        {
                            if (IsHeader(fields))
                            {
                                headerLength = fields.Length;
                                dataLines.Add(fields); //Add the header in datalines
                                nbHeadersFound++;
                            }
                            else if (nbHeadersFound > 0) //There's atleast one header found
                            {
                                if (fields.Length == headerLength)
                                    dataLines.Add(fields);
                            }
                            else
                                continue; //Aller à la prochaine ligne, pour voir si le header s'y trouve
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Gère les erreurs (I/O, format, etc.)
                    success = false;
                    Console.WriteLine($"Erreur lors du parsing CSV : {ex.Message}");
                }
            }
            if (nbHeadersFound <= 0)
                success = false;
            return success ? dataLines : null;
        }

        #endregion

        #region Validation methods
        private static CsvTypes GetCSVTypeFromPath(string filepath)
        {
            CsvTypes type;
            string fileLower = Path.GetFileName(filepath)?.ToLowerInvariant() ?? string.Empty;
            type = fileLower.Contains("books") ? CsvTypes.books
                 : fileLower.Contains("humans") ? CsvTypes.humans
                 : CsvTypes.none;

            return type;
        }
        /// <summary>
        /// Checks if fields from a CSV is part of the known header list or not
        /// </summary>
        /// <param name="fieldLine">the fields in the line</param>
        /// <returns></returns>
        public static bool IsHeader(string[] fieldLine)
        {
            bool isHeader = false;
            string joinedLine = FieldsToString(fieldLine);

            if (Program.StartLines.All.Contains(joinedLine))
                isHeader = true;

            return isHeader;
        }

        ///<summary>
        ///File is valid if: - Exists; - Not empty; - If type is books, has the
        ///books header line (startLines[0]) - If type is humans, has the three
        ///other start lines - If type is none, invalid file - If there are
        ///values after all header line(s) present in file - Filepath contains
        ///"books.csv" or "humans.csv"
        ///</summary>
        ///<param name="filepath"></param>
        ///<param name="errorReason"></param>
        ///<returns></returns>
        internal static bool IsValidFile(string filepath, out string? errorReason)
        {
            errorReason = null;

            // --- 1. Basic existence checks ---
            if (string.IsNullOrWhiteSpace(filepath))
            {
                errorReason = "Le chemin fichier est vide ou null.";
                return false;
            }

            if (!File.Exists(filepath))
            {
                errorReason = $"Fichier introuvable: {Path.GetFileName(filepath)}.";
                return false;
            }

            FileInfo fileInfo = new(filepath);
            if (fileInfo.Length == 0)
            {
                errorReason = $"Le fichier {Path.GetFileName(filepath)} est vide.";
                return false;
            }

            // --- 2. Determine type from file name ---
            string filename = Path.GetFileName(filepath).ToLowerInvariant();
            CsvTypes type = GetCSVTypeFromPath(filepath);

            if (type == CsvTypes.none)
            {
                errorReason = "Filename must contain 'books.csv' or 'humans.csv'.";
                return false;
            }

            // --- 3. Initialize parsing flags ---
            bool hasHeader = false;
            bool hasDataAfterHeader = false;
            int headerLength = 0;
            int headerCount = 0;

            try
            {
                using TextFieldParser parser = new(filepath)
                {
                    CommentTokens = ["#"],
                    HasFieldsEnclosedInQuotes = true,
                    TrimWhiteSpace = true
                };
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();
                    if (fields == null || fields.All(string.IsNullOrWhiteSpace))
                        continue;

                    string joined = FieldsToString(fields);

                    // --- Skip section markers like ###Memberss###
                    if (joined.StartsWith("###", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // --- 4. Header detection ---
                    if (IsHeader(fields))
                    {
                        if (hasHeader)
                        {
                            headerCount++;
                            errorReason = "Duplicate header lines have been found in file";
                            return false;
                        }

                        hasHeader = true;
                        headerLength = fields.Length;
                        headerCount++;
                        continue;
                    }

                    // --- 5. Data after header ---
                    if (hasHeader && fields.Length == headerLength && !fields.All(string.IsNullOrWhiteSpace))
                    {
                        hasDataAfterHeader = true;
                        break; // Once found, no need to continue
                    }
                }
            }
            catch (Exception ex)
            {
                errorReason = $"Error while reading file: {ex.Message}";
                return false;
            }

            // --- 6. Type-specific header validation ---
            bool headerMatchesType = type switch
            {
                CsvTypes.books => hasHeader && headerCount == 1,
                CsvTypes.humans => hasHeader && headerCount >= 1,
                _ => false
            };

            if (!headerMatchesType)
            {
                errorReason = type == CsvTypes.books
                    ? "Missing book header line."
                    : "Missing human header line.";
                return false;
            }

            if (!hasDataAfterHeader)
            {
                errorReason = "No data lines found after header.";
                return false;
            }

            // --- 7. Final success ---
            return true;
        }

        internal static bool IsValidBook(string[] fields, out string? errorReason)
        {
            bool valide = false;

            if (Livre.IsValid(fields, out errorReason))// Exact fields for humans
                valide = true;
            return valide;
        }
        internal static bool IsValidHuman(string[] fields, out string? errorReason, out char? validType)
        {
            bool valide = false;
            errorReason = null;
            validType = null;

            //Load human verifiers
            Author aValidator = new(Guid.Empty, string.Empty, string.Empty);
            Members mValidator = new(Guid.Empty, string.Empty, string.Empty, string.Empty);
            Employee eValidator = new(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            //Check validity for each human type
            bool isEmployeeValid = false, isAuthorValid = false;
            bool isMembersValid = mValidator.IsValid(fields, out string? error);
            if (!isMembersValid)
            {
                isEmployeeValid = eValidator.IsValid(fields, out error);
                if (!isEmployeeValid)
                    isAuthorValid = aValidator.IsValid(fields, out error);
            }
            int validCount = (isAuthorValid ? 1 : 0)
                           + (isMembersValid ? 1 : 0)
                           + (isEmployeeValid ? 1 : 0);

            if (validCount == 1)
            {
                valide = true;

                if (isAuthorValid)
                    validType = 'a';
                else if (isMembersValid)
                    validType = 'm';
                else if (isEmployeeValid)
                    validType = 'e';
            }
            else
            {
                // Handle invalid cases
                if (validCount == 0)
                    errorReason = error;
                else
                    errorReason = "Invalid human: matches multiple human types.";
            }
            return valide;
        }
        #endregion

        /// <summary>
        /// Obtenir le nom de tous les fichiers se retrouvant dans le dossier, à
        /// partir du filepath de chacun des fichiers
        /// </summary>
        /// <param name="chemin">filepath</param>
        /// <returns>La liste des noms des fichiers</returns>
        public static List<string> GetTxtFilenames(string chemin, bool csvOnly = true)
        {
            List<string> nomsFichiers = [];

            // Use filter if csvOnly is true, otherwise get all files
            string condition = csvOnly ? "*.csv" : "*";

            //FileWriter.CreerFichierSiInexistant();

            // Get the file paths matching the condition
            List<string> cheminFichiers = [.. Directory.GetFiles(chemin, condition)]; //[..Array[]] == Array.ToList()

            // Extract just the file names (without full path)
            foreach (string fichier in cheminFichiers)
            {
                nomsFichiers.Add(Path.GetFileName(fichier));
            }

            return nomsFichiers;
        }

        public static string[] ReadLineToFields(CsvTypes type)
        {
            string[] fieldNames = GetFieldNames(type);
            string[] fields = new string[fieldNames.Length];

            for (int i = 0; i < fieldNames.Length; i++)
            {
                string name = fieldNames[i].Trim().ToLower();

                // Leave GUID blank automatically
                if (name == "guid")
                {
                    fields[i] = "";
                    continue;
                }

                string prompt = GetFriendlyPrompt(name);
                string input;
                bool valid;

                do
                {
                    input = FonctionsDeBase.LireTexte(prompt).Trim();
                    bool accepteVide = name == "bookaddon";
                    valid = ValidateField(name, input, accepteVide);

                    if (!valid)
                        FonctionsDeBase.AfficherLigne($"Valeur invalide pour '{fieldNames[i]}', recommencez.", ConsoleColor.Red);

                } while (!valid);

                fields[i] = input;
            }

            return fields;
        }

        private static string[] GetFieldNames(CsvTypes type)
        {
            return type switch
            {
                CsvTypes.books => Program.StartLines.books.Split(','),
                CsvTypes.employees => Program.StartLines.employees.Split(','),
                CsvTypes.author => Program.StartLines.authors.Split(','),
                CsvTypes.members => Program.StartLines.members.Split(','),
                _ => throw new ArgumentException("Type CSV inconnu.")
            };
        }

        private static string GetFriendlyPrompt(string name)
        {
            return name.ToLower() switch
            {
                "lname" or "lastname" => "Nom de famille: ",
                "fname" or "firstname" => "Prénom : ",
                "postname" or "position" => "Poste : ",
                "phone" => "Numéro de téléphone (format : XXX-XXX-XXXX) : ",
                "title" => "Titre du livre : ",
                "author" => "Nom de l’auteur : ",
                "genre" => "Genre : ",
                "pubdate" => "Date de publication (format : yyyy-MM-dd) : ",
                "isbn" => "ISBN (10 ou 13 chiffres valides) : ",
                "bookaddon" => "Add-on (laisser vide si aucun) : ",
                "isavailable" => "Disponible ? (true / false) : ",
                _ => $"Nom : "
            };
        }

        private static bool ValidateField(string name, string input, bool accepteVide)
        {
            string lower = name.ToLower();

            if (accepteVide && string.IsNullOrWhiteSpace(input))
                return true;

            if (lower == "pubdate")
                return DateTime.TryParse(input, out _);

            if (lower == "isbn")
                return Regex.IsMatch(input, @"^(97(8|9))?\d{9}(\d|X)$");

            if (lower == "isavailable")
                return bool.TryParse(input, out _);

            if (lower == "phone")
                return Members.IsValidPhone(input);

            // Basic text validation
            return !string.IsNullOrWhiteSpace(input);
        }

        static string FieldsToString(string[] fieldLine)
        {
            string joinedLine = string.Join(",", fieldLine).Trim();
            return joinedLine;

        }

    }

}

