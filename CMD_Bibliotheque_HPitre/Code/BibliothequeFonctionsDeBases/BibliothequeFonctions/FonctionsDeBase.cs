using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BibliothequeFonctionsDeBase;

public class FonctionsDeBase
{
    #region Affichage de texte

    /// <summary>
    /// Affiche une ligne de texte lettre par lettre.
    /// </summary>
    /// <param name="texte">Le texte à afficher.</param>
    /// <param name="couleur">La couleur console du texte, par défaut White.</param>
    public static void AfficherLigne(string texte, ConsoleColor couleur = ConsoleColor.White)
    {
        Console.ForegroundColor = couleur;
        Console.WriteLine(texte);
        Console.ResetColor();
    }

    /// <summary>
    /// Affiche un texte lettre par lettre.
    /// </summary>
    /// <param name="texte">Le texte à afficher.</param>
    /// <param name="couleur">La couleur console du texte, par défaut White.</param>
    public static void AfficherTexteProgressif(string texte, ConsoleColor couleur = ConsoleColor.White)
    {
        Console.ForegroundColor = couleur;

        foreach (char lettre in texte)
        {
            Thread.Sleep(25);
            Console.Write(lettre);
        }

        Console.ResetColor();
    }

    /// <summary>
    /// Affiche un titre entre 2 lignes de n asterisques.
    /// </summary>
    /// <param name="titre">Le titre à afficher.</param>
    public static void AfficherTitre(string titre, int nbAsterisques = 30, ConsoleColor couleur = ConsoleColor.Cyan, bool centre = true)
    {
        // Déclaration des variables
        string asterisques = "".PadLeft(nbAsterisques, '*');

        // Centrage du titre
        if (centre)
        {
            int nbEspaces = (nbAsterisques - titre.Length) / 2;
            titre = titre.PadLeft(nbEspaces + titre.Length);
        }

        // Affichage de l'entête
        Console.ResetColor();
        Console.ForegroundColor = couleur;
        Console.WriteLine(asterisques);
        Console.WriteLine(titre.ToUpper());
        Console.WriteLine(asterisques);
        Console.WriteLine();
        Console.ResetColor();
    }

   

    #endregion

    #region Lecture de texte

    /// <summary>
    /// Lit un caractère dans la console.
    /// </summary>
    /// <returns>Le caractère lu dans la console.</returns>
    public static char LireCaractere()
    {
        return LireCaractere("");
    }

    /// <summary>
    /// Lit un caractère dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire un caractère dans la console.</param>
    /// <returns>Le caractère lu dans la console.</returns>
    public static char LireCaractere(string message)
    {
        string texte;

        Console.Write(message);
        texte = Console.ReadLine() ?? "";

        while (texte.Length != 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Nombre de caractères invalide, recommencez : ");
            Console.ResetColor();
            texte = Console.ReadLine() ?? "";
        }

        return texte[0];
    }

    /// <summary>
    /// Lit un texte dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire le texte dans la console.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <param name="accepteVide">Accepte un texte vide. Par défaut VRAI.</param>
    /// <returns>Le texte lu dans la console.</returns>
    public static string LireTexte(string message, string messageInvalide = "", bool accepteVide = true)
    {
        string reponse;

        Console.Write(message);
        reponse = Console.ReadLine() ?? "";

        if (accepteVide == false)
        {
            while (reponse.Trim() == "")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(messageInvalide);
                Console.ResetColor();
                reponse = Console.ReadLine() ?? "";
            }
        }

        return reponse;
    }

    /// <summary>
    /// Lit un texte non vide dans la console.
    /// </summary>
    /// <returns>Le texte non vide lu dans la console.</returns>
    public static string LireTexteNonVide()
    {
        return LireTexteNonVide("");
    }

    /// <summary>
    /// Lit un texte non vide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire le texte non vide dans la console.</param>
    /// <returns>Le texte non vide lu dans la console.</returns>
    public static string LireTexteNonVide(string message)
    {
        string texte;

        Console.Write(message);
        texte = Console.ReadLine() ?? "";
        texte = texte.Trim();

        while (texte == "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Valeur obligatoire, recommencez : ");
            Console.ResetColor();
            texte = Console.ReadLine() ?? "";
        }

        return texte;
    }



    #endregion

    #region string manipulation

    /// <summary>
    /// Convertit un tableau de chaînes de caractères en une seule chaîne de caractères, les champs étant séparés par des virgules.
    /// </summary>
    /// <param name="fieldLine"></param>
    /// <returns></returns>
    public static string FieldsToString(string[] fieldLine)
    {
        string joinedLine = string.Join(",", fieldLine).Trim();
        return joinedLine;

    }

    #endregion

    #region Lecture de double

    /// <summary>
    /// Lit un nombre réel (double) valide dans la console.
    /// </summary>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDouble()
    {
        return LireDouble("");
    }

    /// <summary>
    /// Lit un nombre réel (double) valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire le nombre réel dans la console.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDouble(string message)
    {
        return LireDouble(message, "Nombre réel invalide, recommencez : ");
    }

    /// <summary>
    /// Lit un nombre réel (double) valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire le nombre réel dans la console.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDouble(string message, string messageInvalide)
    {
        bool valide;

        Console.Write(message);
        valide = double.TryParse(Console.ReadLine(), out double reel);

        while (valide == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(messageInvalide);
            Console.ResetColor();
            valide = double.TryParse(Console.ReadLine(), out reel);
        }

        return reel;
    }

    /// <summary>
    /// Lit un double valide dans la console.
    /// </summary>
    /// <param name="min">La valeur minimale valide.</param>
    /// <param name="max">La valeur maximale valide.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDoubleMinMax(double min, double max)
    {
        return LireDoubleMinMax("", min, max);
    }

    /// <summary>
    /// Lit un double valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="min">La valeur minimale valide.</param>
    /// <param name="max">La valeur maximale valide.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDoubleMinMax(string message, double min, double max)
    {
        return LireDoubleMinMax(message, $"Cette valeur est invalide ({min} à {max}), recommencez : ", min, max);
    }

    /// <summary>
    /// Lit un double valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <param name="min">La valeur minimale valide.</param>
    /// <param name="max">La valeur maximale valide.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDoubleMinMax(string message, string messageInvalide, double min, double max)
    {
        double reel;
        bool valide;

        Console.Write(message);
        valide = double.TryParse(Console.ReadLine(), out reel);

        while (valide == false || reel < min || reel > max)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(messageInvalide);
            Console.ResetColor();
            valide = double.TryParse(Console.ReadLine(), out reel);
        }

        return reel;
    }

    /// <summary>
    /// Lit un double plus grand que zéro dans la console.
    /// </summary>
    /// <param name="message">Le message à afficher.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDoublePositif(string message)
    {
        return LireDoublePositif(message, "Cette valeur est invalide. Le nombre doit être plus grand que zéro. Recommencez :");
    }

    /// <summary>
    /// Lit un double plus grand que zéro dans la console.
    /// </summary>
    /// <param name="message">Le message à afficher.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static double LireDoublePositif(string message, string messageInvalide)
    {
        // Déclaration des variables
        double reel;
        bool valide;

        // Écriture du message à la console
        Console.Write(message);
        valide = double.TryParse(Console.ReadLine(), out reel);

        while (valide == false || reel <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(messageInvalide);
            Console.ResetColor();
            valide = double.TryParse(Console.ReadLine(), out reel);
        }

        return reel;
    }

    /// <summary>
    /// Lit un nombre de doubles valides dans la console.
    /// </summary>
    /// <param name="nbDoubles">Nombre de doubles à lire.</param>
    /// <returns>Le nombre réel (double) lu dans la console.</returns>
    public static List<double> LireDoubles(int nbDouble)
    {
        return LireDoubles(nbDouble);
    }

    /// <summary>
    /// Lit un nombre de doubles valides dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="nbDoubles">Nombre de doubles à lire.</param>
    /// <returns>Les nombres réels (double) lus dans la console.</returns>
    public static List<double> LireDoubles(string message, int nbDoubles)
    {
        List<double> doubles = new List<double>();

        Console.WriteLine(message);

        for (double index = 1; index <= nbDoubles; index++)
        {
            double reel = LireDouble($"Entier #{index} : ");
            doubles.Add(reel);
        }

        return doubles;
    }


    /// <summary>
    /// Lit un nombre de doubles valides dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="nbDoubles">Nombre de doubles à lire.</param>
    /// <returns>Les nombres réels (double) lus dans la console.</returns>
    public static List<double> LireDoublesMinMax(string message, int nbDoubles, double min, double max)
    {
        List<double> doubles = new List<double>();

        Console.WriteLine(message);

        for (double index = 1; index <= nbDoubles; index++)
        {
            double reel = LireDoubleMinMax($"Entier #{index} : ", min, max);
            doubles.Add(reel);
        }

        return doubles;
    }


    /// <summary>
    /// Lit un nombre de doubles valides dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="nbDoubles">Nombre de doubles à lire.</param>
    /// <returns>Les nombres réels (double) lus dans la console.</returns>
    public static List<double> LireDoublesPositifs(string message, int nbDoubles)
    {
        List<double> doubles = new List<double>();

        Console.WriteLine(message);

        for (double index = 1; index <= nbDoubles; index++)
        {
            double reel = LireDoublePositif($"Entier #{index} : ");
            doubles.Add(reel);
        }

        return doubles;
    }

    #endregion

    #region LireEntier

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <returns>L'entier lu dans la console.</returns>
    public static int LireEntier()
    {
        return LireEntier("");
    }

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static int LireEntier(string message)
    {
        return LireEntier(message, "Nombre entier invalide, recommencez : ");
    }

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static int LireEntier(string message, string messageInvalide)
    {
        int entier;
        bool valide;

        Console.Write(message);
        valide = int.TryParse(Console.ReadLine(), out entier);

        while (valide == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(messageInvalide);
            Console.ResetColor();
            valide = int.TryParse(Console.ReadLine(), out entier);
        }

        return entier;
    }

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <param name="min">La valeur minimale valide.</param>
    /// <param name="max">La valeur maximale valide.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static int LireEntierMinMax(int min, int max)
    {
        return LireEntierMinMax("", min, max);
    }

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="min">La valeur minimale valide.</param>
    /// <param name="max">La valeur maximale valide.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static int LireEntierMinMax(string message, int min, int max)
    {
        return LireEntierMinMax(message, $"Nombre entier invalide ({min} à {max}), recommencez : ", min, max);
    }

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <param name="min">La valeur minimale valide.</param>
    /// <param name="max">La valeur maximale valide.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static int LireEntierMinMax(string message, string messageInvalide, int min, int max)
    {
        int entier;
        bool valide;

        Console.Write(message);
        valide = int.TryParse(Console.ReadLine(), out entier);

        while (valide == false || entier < min || entier > max)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(messageInvalide);
            Console.ResetColor();
            valide = int.TryParse(Console.ReadLine(), out entier);
        }

        return entier;
    }

    /// <summary>
    /// Lit un nombre entier plus grand que zéro dans la console.
    /// </summary>
    /// <param name="message">Le message à afficher.</param>
    /// <returns>Le nombre entier lu dans la console.</returns>
    public static int LireEntierPositif(string message)
    {
        return LireEntierPositif(message, "Le nombre entier saisi est invalide. Il doit être plus grand que zéro. Recommencez :");
    }

    /// <summary>
    /// Lit un nombre entier plus grand que zéro dans la console.
    /// </summary>
    /// <param name="message">Le message à afficher.</param>
    /// <param name="messageInvalide">Le message à afficher lorsque l'lue est invalide.</param>
    /// <returns>Le nombre entier lu dans la console.</returns>
    public static int LireEntierPositif(string message, string messageInvalide)
    {
        // Déclaration des variables
        int entier;
        bool valide;

        // Écriture du message à la console
        Console.Write(message);
        valide = int.TryParse(Console.ReadLine(), out entier);

        while (valide == false || entier <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(messageInvalide);
            Console.ResetColor();
            valide = int.TryParse(Console.ReadLine(), out entier);
        }

        return entier;
    }

    /// <summary>
    /// Lit un entier valide dans la console.
    /// </summary>
    /// <param name="nbEntiers">Nombre d'entiers à lire.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static List<int> LireEntiers(int nbEntiers)
    {
        return LireEntiers(nbEntiers);
    }

    /// <summary>
    /// Lit un nombre d'entiers valides dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="nbEntiers">Nombre d'entiers à lire.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static List<int> LireEntiers(string message, int nbEntiers)
    {
        List<int> entiers = new List<int>();

        Console.WriteLine(message);

        for (int index = 1; index <= nbEntiers; index++)
        {
            int entier = LireEntier($"Entier #{index} : ");
            entiers.Add(entier);
        }

        return entiers;
    }

    /// <summary>
    /// Lit un nombre d'entiers valides dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="nbEntiers">Nombre d'entiers à lire.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static List<int> LireEntiersMinMax(string message, int nbEntiers, int min, int max)
    {
        List<int> entiers = new List<int>();

        Console.WriteLine(message);

        for (int index = 1; index <= nbEntiers; index++)
        {
            int entier = LireEntierMinMax($"Entier #{index} : ", min, max);
            entiers.Add(entier);
        }

        return entiers;
    }


    /// <summary>
    /// Lit un nombre d'entiers valides dans la console.
    /// </summary>
    /// <param name="message">Le message affiché avant de lire l'entier dans la console.</param>
    /// <param name="nbEntiers">Nombre d'entiers à lire.</param>
    /// <returns>L'entier lu dans la console.</returns>
    public static List<int> LireEntiersPositifs(string message, int nbEntiers)
    {
        List<int> entiers = new List<int>();

        Console.WriteLine(message);

        for (int index = 1; index <= nbEntiers; index++)
        {
            int entier = LireEntierPositif($"Entier #{index} : ");
            entiers.Add(entier);
        }

        return entiers;
    }


    #endregion

    #region Aesthetic

    /// <summary>
    /// Cammoufle le texte affiché à la console à la fin de l'excéution.
    /// </summary>
    public static void MasquerTexte()
    {
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Black;
    }

    /// <summary>
    /// Affiche un titre encadré dans la console.
    /// </summary>
    /// <param name="title"></param>
    public static void DisplayTitle(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=====================================");
        Console.WriteLine($"   {title}");
        Console.WriteLine("=====================================\n");
        Console.ResetColor();
    }


    /// <summary>
    /// affiche un effet de chargement dans la console.
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

    /// <summary>
    /// Attend la pression d'une touche pour continuer.
    /// </summary>
    public static void WaitKey()
    {
        FonctionsDeBase.AfficherTexteProgressif("\nAppuyez sur une touche pour continuer...", ConsoleColor.DarkGray);
        Console.ReadKey();
        Console.WriteLine();
    }

    #endregion

    #region Regex

    /// <summary>
    ///  Vérifie si le format du numéro de téléphone est valide.
    /// </summary>
    /// <param name="phone"></param>
    /// <returns></returns>
    public static bool IsValidPhone(string phone)
    {
        return !Regex.IsMatch(phone, @"^0\d{9}$"); // French phone format
    }


    #endregion

    #region streamWriter

    /// <summary>
    /// Ajoute une ligne à la fin d'un fichier texte.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="textToWrite"></param>
    public static void AppendLineToFile(string filePath, string textToWrite)
    {
        try
        {
            // Open the file for appending. If the file doesn't exist, it will be created.
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine(textToWrite);
            }
            Console.WriteLine($"Text appended successfully at {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    /// <summary>
    /// Supprime une ligne contenant un GUID spécifique dans un fichier texte.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="guid"></param>
    public static void DeleteLineWithGuid(string filePath, string guid)
    {
        try
        {
            // Read all lines from the file
            var lines = File.ReadAllLines(filePath).ToList();

            // Remove the line that contains the GUID
            var lineToDelete = lines.FirstOrDefault(line => line.Contains(guid));
            if (lineToDelete != null)
            {
                lines.Remove(lineToDelete);
                // Write the remaining lines back to the file
                File.WriteAllLines(filePath, lines);
                Console.WriteLine("Line with GUID deleted successfully.");
            }
            else
            {
                Console.WriteLine("GUID not found in the file.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    /// <summary>
    /// Supprime un fichier spécifique.
    /// </summary>
    /// <param name="filePath"></param>
    public static void DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("File deleted successfully.");
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting file: " + ex.Message);
        }
    }

    /// <summary>
    /// Crée un fichier à l'emplacement spécifié.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    public static void CreateFile(string filePath, string fileName = null)
    {
        try
        {
            // Determine the full path
            string fullPath = fileName == null
                ? filePath
                : Path.Combine(filePath, fileName);

            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create the file if it doesn't exist
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Dispose(); // Dispose to release file handle immediately
                Console.WriteLine($"File created: {fullPath}");
            }
            else
            {
                Console.WriteLine("File already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating file: " + ex.Message);
        }
    }

    #endregion

    #region Encryption

    /// <summary>
    /// Encrypts a file using AES encryption with a random IV.
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="password"></param>
    public static void EncryptFile(string inputFilePath, string outputFilePath, string password)
    {
        try
        {
            byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV(); // Random IV

                using (FileStream outFs = new FileStream(outputFilePath, FileMode.Create))
                {
                    // Write the IV at the beginning of the file
                    outFs.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(outFs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (FileStream inFs = new FileStream(inputFilePath, FileMode.Open))
                    {
                        inFs.CopyTo(cryptoStream);
                    }
                }
            }

            Console.WriteLine("File encrypted with random IV.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Encryption error: " + ex.Message);
        }
    }


    /// <summary>
    /// Decrypts a file using AES decryption with the IV embedded in the file.
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="password"></param>
    public static void DecryptFile(string inputFilePath, string outputFilePath, string password)
    {
        try
        {
            byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

            using (FileStream inFs = new FileStream(inputFilePath, FileMode.Open))
            {
                byte[] iv = new byte[16];
                inFs.Read(iv, 0, iv.Length); // Read IV from the start of the file

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (CryptoStream cryptoStream = new CryptoStream(inFs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream outFs = new FileStream(outputFilePath, FileMode.Create))
                    {
                        cryptoStream.CopyTo(outFs);
                    }
                }
            }

            Console.WriteLine("File decrypted using embedded IV.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Decryption error: " + ex.Message);
        }
    }


    /// <summary>
    /// Gets a StreamReader for a decrypted file using AES decryption with the IV embedded in the file.
    /// </summary>
    /// <param name="encryptedFilePath"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static StreamReader GetDecryptedStreamReader(string encryptedFilePath, string password)
    {
        byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        FileStream encryptedFileStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read);

        // Read IV from beginning of file
        byte[] iv = new byte[16];
        int bytesRead = encryptedFileStream.Read(iv, 0, iv.Length);
        if (bytesRead != iv.Length)
        {
            encryptedFileStream.Dispose();
            throw new Exception("Could not read IV from encrypted file.");
        }

        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        CryptoStream cryptoStream = new CryptoStream(encryptedFileStream, aes.CreateDecryptor(), CryptoStreamMode.Read);

        return new StreamReader(cryptoStream, Encoding.UTF8);
    }

    /// <summary>
    /// Decrypts encryptedFilePath (IV prepended) to a temp file and returns that temp file path.
    /// </summary>
    /// <param name="encryptedFilePath"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    // Decrypts encryptedFilePath (IV prepended) to a temp file and returns that temp file path.
    // Caller must delete the returned file when done.
    public static string DecryptToTempFile(string encryptedFilePath, string password)
    {
        //string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
        string tempPath = Path.Combine(Path.GetTempPath(), "humans.csv");

        byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        using (FileStream inFs = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
        {
            // Read IV (first 16 bytes)
            byte[] iv = new byte[16];
            int read = inFs.Read(iv, 0, iv.Length);
            if (read != iv.Length)
                throw new InvalidOperationException("Encrypted file missing IV or corrupt.");

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // create temp file and write decrypted content to it
                using (FileStream outFs = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (CryptoStream cryptoStream = new CryptoStream(inFs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outFs);
                }
            }
        }

        return tempPath;
    }



    #endregion
}



