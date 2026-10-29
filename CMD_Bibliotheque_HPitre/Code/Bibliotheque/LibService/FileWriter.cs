using Bibliotheque.LibModel;
using BibliothequeFonctionsDeBase;

namespace Bibliotheque.LibService
{
    internal class FileWriter
    {

        /// <summary>
        /// Écrit une ligne dans un fichier CSV, en créant le fichier et
        /// l’en-tête s’ils n’existent pas.
        /// </summary>
        /// <param name="header">En-tête du fichier CSV.</param>
        /// <param name="filename">Nom du fichier cible.</param>
        /// <param name="section">Nom de la section dans laquelle ajouter la
        ///     ligne.</param>
        /// <param name="line">Ligne CSV à ajouter.</param>
        public static void EcrireLigne(string header, string filePath, string section, string line)
        {

            // Création du fichier s’il n’existe pas
            if (!File.Exists(filePath))
            {
                using StreamWriter sw = new(filePath);
                sw.WriteLine("#WARNING : ANY MANUAL MODIFICATION MAY MAKE THE FILE UNREADABLE FOR THE PROGRAM.");
                sw.WriteLine($"###{section}###");
                sw.WriteLine(header);
                sw.WriteLine($"###/{section}###");
            }

            // Ajout de la ligne à la section spécifiée
            AjouterALaSection(filePath, section, line);
        }

        /// <summary>
        /// Insère une ligne dans la section appropriée d’un fichier CSV. La
        /// section est repérée par une balise du type ###section###.
        /// </summary>
        /// <param name="filePath">Chemin complet du fichier cible.</param>
        /// <param name="section">Nom de la section où insérer la ligne.</param>
        /// <param name="line">Contenu à insérer.</param>
        private static void AjouterALaSection(string filePath, string section, string line)
        {
            string tempFile = Path.GetTempFileName();

            using (var sr = new StreamReader(filePath))
            using (var sw = new StreamWriter(tempFile))
            {
                string? currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    // Lorsque la balise de section est trouvée, on insère la nouvelle ligne avant
                    if (currentLine.Trim() == $"###/{section}###")
                        sw.WriteLine(line);

                    sw.WriteLine(currentLine);
                }
            }

            // Remplacement du fichier original par le fichier temporaire mis à jour
            File.Copy(tempFile, filePath, true);
            File.Delete(tempFile);

            Console.WriteLine($"Fichier mis à jour : {filePath}");
        }


        /// <summary>
        /// Supprime une entrée d’un fichier CSV selon le guid et la section.
        /// </summary>
        /// <param name="filename">Nom du fichier CSV cible.</param>
        /// <param name="section">Nom de la section où supprimer la ligne.
        ///     </param>
        /// <param name="guid">Identifiant unique de l’entrée à supprimer.
        ///     </param>
        /// <returns>Vrai si la suppression est réussie, faux sinon.</returns>
        public static bool Supprimer(string filePath, string section, Guid guid)
        {
            string filename = Path.GetFileName(filePath) ?? "unknown file";
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Fichier introuvable : {filePath}");
                    return false;
                }

                string tempFile = Path.GetTempFileName();

                using (var sr = new StreamReader(filePath))
                using (var sw = new StreamWriter(tempFile))
                {
                    string? currentLine;
                    bool inTargetSection = false;

                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        // Début de section
                        if (currentLine.Trim() == $"###{section}###")
                            inTargetSection = true;

                        // Fin de section
                        if (currentLine.Trim() == $"###/{section}###")
                            inTargetSection = false;

                        // Écriture conditionnelle : ne pas écrire la ligne si elle correspond au guid et on est dans la section
                        if (inTargetSection && currentLine.StartsWith(guid.ToString(), StringComparison.OrdinalIgnoreCase))
                            continue;

                        sw.WriteLine(currentLine);
                    }
                }

                File.Copy(tempFile, filePath, true);
                File.Delete(tempFile);

                FonctionsDeBase.AfficherLigne($"Suppresion de {guid} dans la section {section} de {filename}.", ConsoleColor.DarkRed);
                return true;
            }

            catch (Exception ex)
            {
                Displays.ShowError($"Erreur lors de la suppression : {ex.Message}");
                return false;
            }
        }
    }
}
