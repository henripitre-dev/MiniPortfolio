using Bibliotheque.LibData;
using Bibliotheque.LibService;
using static Bibliotheque.LibModel.Program;

namespace Bibliotheque.Tests
{
    #region Books tests
    [TestClass]
    public class BooksTests
    {
        // Test filePath
        string filePath = @"..\..\..\..\Bibliotheque\LibService\files\TestUnitaires_books.csv";

        // Test vars
        Guid guid = Guid.Parse("a1234567-89ab-4cde-f012-3456789abcde");
        string title = "Test Book";
        string author = "John Doe";
        string genre = "Fiction";
        DateTime pubDate = new DateTime(2020, 1, 1);
        string isbn = "9781234567897";
        string? bookAddon = "Special Edition";
        bool isAvailable = true;

        #region Constructeur

        [TestMethod]
        public void Constructeur_ProprietesInitialisees_Correctement()
        {
            // Arrange
            string[] fields = new string[]
            {
                guid.ToString(),
                title,
                author,
                genre,
                pubDate.ToString("yyyy-MM-dd"),
                isbn,
                bookAddon, //not null
                isAvailable.ToString()
            };

            // Act
            var livre = Livre.Parse(fields);

            // Assert
            Assert.AreEqual(guid, livre.Guid);
            Assert.AreEqual(title, livre.Title);
            Assert.AreEqual(author, livre.Author);
            Assert.AreEqual(genre, livre.Genre);
            Assert.AreEqual(pubDate, livre.PubDate);
            Assert.AreEqual(isbn, livre.ISBN);
            Assert.AreEqual(bookAddon, livre.BookAddon);
            Assert.AreEqual(isAvailable, livre.IsAvailable);
        }

        #endregion

        #region Parse et TryParse

        [TestMethod]
        public void Parse_FieldsValides_RetourneLivre()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            string[] fields = new string[]
            {
                guid.ToString(),
                "Le Petit Prince",
                "Antoine de Saint-Exupéry",
                "Conte",
                "1943-04-06",
                "9782070612758",
                "Édition illustrée",
                "True"
            };

            // Act
            var livre = Livre.Parse(fields);

            // Assert
            Assert.IsNotNull(livre);
            Assert.AreEqual(guid, livre.Guid);
            Assert.AreEqual("Le Petit Prince", livre.Title);
            Assert.AreEqual("Antoine de Saint-Exupéry", livre.Author);
            Assert.AreEqual("Conte", livre.Genre);
            Assert.AreEqual(new DateTime(1943, 4, 6), livre.PubDate);
            Assert.AreEqual("9782070612758", livre.ISBN);
            Assert.AreEqual("Édition illustrée", livre.BookAddon);
            Assert.IsTrue(livre.IsAvailable);
        }

        [TestMethod]
        public void TryParse_FieldsInvalides_RetourneNullEtErreur()
        {
            // Arrange
            string[] fields = new string[] { "invalid", "data" };

            // Act
            bool result = Livre.IsValid(fields, out string error);


            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual("Nombre de champs invalide pour un livre.", error);
        }

        #endregion

        #region IsValid

        [TestMethod]
        public void IsValid_FieldsValides_RetourneTrue()
        {
            // Arrange
            string[] fields = new string[]
            {
                Guid.NewGuid().ToString(),
                "Titre Valide",
                "Auteur Valide",
                "Genre",
                "2020-01-01",
                "9781234567890",
                "Addon",
                "true"
            };

            // Act
            bool result = Livre.IsValid(fields, out string error);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void IsValid_TitreVide_RetourneFalse()
        {
            // Arrange
            string[] fields = new string[]
            {
                Guid.NewGuid().ToString(),
                "", // Titre vide
                "Auteur",
                "Genre",
                "2020-01-01",
                "9781234567890",
                "Addon",
                "true"
            };

            // Act
            bool result = Livre.IsValid(fields, out string error);

            // Assert
            Assert.IsFalse(result);
            StringAssert.Contains(error, "ne doivent pas être vides");
        }

        [TestMethod]
        public void IsValid_ISBNInvalide_RetourneFalse()
        {
            // Arrange
            string[] fields = new string[]
            {
                Guid.NewGuid().ToString(),
                "Titre",
                "Auteur",
                "Genre",
                "2020-01-01",
                "123", // invalide
                "Addon",
                "true"
            };

            // Act
            bool result = Livre.IsValid(fields, out string error);

            // Assert
            Assert.IsFalse(result);
            StringAssert.Contains(error, "ISBN invalide");
        }

        #endregion

        #region AjouterLivre

        [TestMethod]
        public void AjouterLivre_ReturnsTrue()
        {
            // Arrange
            var livre = new Livre(guid, title, author, genre, pubDate, isbn, bookAddon, isAvailable);

            // Act
            bool result = Livre.AjouterLivre(livre, filePath, StartLines.books);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion


        #region SupprimerLivre

        [TestMethod]
        public void SupprimerLivre_ReturnsTrueIfFound()
        {
            // Arrange
            // nothing to arrange, the book should already exist in the file from previous test

            // Act
            bool result = Livre.SupprimerLivre(guid, filePath);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region RechercherLivre

        [TestMethod]
        public void RechercherLivre_ParGuid_TrouveIndex()
        {
            // Arrange
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            var livres = new List<Livre>
            {
                new Livre(guid1, "Livre 1", "Auteur 1", "Genre", DateTime.Now, "1234567890", null, true),
                new Livre(guid2, "Livre 2", "Auteur 2", "Genre", DateTime.Now, "1234567891", null, true)
            };

            // Act
            int? index = Livre.RechercherLivre(livres, guid2);

            // Assert
            Assert.IsNotNull(index);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void RechercherLivre_ParTitreInexistant_RetourneNull()
        {
            // Arrange
            var list = new List<Livre>();

            // Act
            var indexes = Livre.RechercherLivre(list, "Nonexistent Title");

            // Assert
            Assert.AreEqual(0, indexes.Count);
        }

        #endregion
    }
    #endregion

    #region Human and derived class tests 

    #region Human tests for abstract class
    [TestClass]
    public class HumansTests
    {
        // Minimal concrete class to test the abstract Humans
        private class TestHuman : Humans
        {
            public TestHuman(Guid guid, string lastName, string firstName)
                : base(guid, lastName, firstName)
            {
            }
        }

        #region Constructor

        [TestMethod]
        public void Constructor_PropertiesSetCorrectly()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            string lastName = "Doe";
            string firstName = "John";

            // Act
            var human = new TestHuman(guid, lastName, firstName);

            // Assert
            Assert.AreEqual(guid, human.Guid);
            Assert.AreEqual(lastName, human.LastName);
            Assert.AreEqual(firstName, human.FirstName);
        }

        #endregion

        #region IsValid

        [TestMethod]
        public void IsValid_ValidFields_ReturnsTrue()
        {
            // Arrange
            var human = new TestHuman(Guid.NewGuid(), "Doe", "Jane");
            string[] fields = new string[] { human.Guid.ToString(), "Doe", "Jane" };

            // Act
            bool result = human.IsValid(fields, out string? error);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void IsValid_NullOrTooFewFields_ReturnsFalse()
        {
            // Arrange
            var human = new TestHuman(Guid.NewGuid(), "Doe", "Jane");
            string[] fields = new string[] { "onlyOneField" };

            // Act
            bool result = human.IsValid(fields, out string? error);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual("Informations insuffisantes sur la personne.", error);
        }

        [TestMethod]
        public void IsValid_EmptyFields_ReturnsFalse()
        {
            // Arrange
            var human = new TestHuman(Guid.NewGuid(), "Doe", "Jane");
            string[] fields = new string[] { "", "", "" };

            // Act
            bool result = human.IsValid(fields, out string? error);

            // Assert
            Assert.IsFalse(result);
            StringAssert.Contains(error, "ne peut être vide");
        }

        #endregion
    }
    #endregion 

    [TestClass]
    [DoNotParallelize]
    public class AuthorTests
    {
        // Test filePath
        string filePath = @"..\..\..\..\Bibliotheque\LibService\files\TestUnitaires_humans.csv";

        // Test var

        Guid guid = Guid.Parse("b76636a0-eb19-412c-a159-0e037eaf8231");
        string lastName = "Hemingway";
        string firstName = "Ernest";


        #region Parse

        [TestMethod]
        public void Parse_FieldsValides_RetourneAuthor()
        {
            // Arrange
            string[] fields = new string[] { guid.ToString(), lastName, firstName };

            // Act
            var author = Author.Parse(fields);

            // Assert
            Assert.IsNotNull(author);
            Assert.AreEqual(guid, author.Guid);
            Assert.AreEqual(lastName, author.LastName);
            Assert.AreEqual(firstName, author.FirstName);
        }

        #endregion

        #region AjouterAuteur

        [TestMethod]
        public void AjouterAuteur_ReturnsTrue()
        {
            // Arrange
            var author = Author.Parse(new string[] { guid.ToString(), lastName, firstName });

            // Act
            bool result = Author.AjouterAuteur(author, filePath, StartLines.authors);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region SupprimerAuteur

        [TestMethod]
        public void SupprimerAuteur_ReturnsTrueIfFound()
        {
            // Arrange
            // nothing to arrange, the author should already exist in the file from previous test

            // Act
            bool result = Author.SupprimerAuteur(guid, filePath);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }



    [TestClass]
    [DoNotParallelize]
    public class EmployeeTests
    {
        // Test filePath
        string filePath = @"..\..\..\..\Bibliotheque\LibService\files\TestUnitaires_humans.csv";

        // Test vars
        Guid guid = Guid.Parse("f73356a0-fb19-412c-a159-7a937eaf8231");
        string lastName = "Hemingway";
        string firstName = "Ernest";
        string position = "Writer";
        string phone = "418-555-1234";



        #region Parse

        [TestMethod]
        public void Parse_FieldsValides_RetourneEmployee()
        {
            // Arrange
            string[] fields = new string[] { guid.ToString(), lastName, firstName, position, phone };

            // Act
            var employee = Employee.Parse(fields);

            // Assert
            Assert.IsNotNull(employee);
            Assert.AreEqual(guid, employee.Guid);
            Assert.AreEqual(lastName, employee.LastName);
            Assert.AreEqual(firstName, employee.FirstName);
            Assert.AreEqual(position, employee.Position);
            Assert.AreEqual(phone, employee.Phone);
        }

        #endregion

        #region AjouterEmploye

        [TestMethod]
        public void AjouterEmploye_ReturnsTrue()
        {
            // Arrange
            var employee = new Employee(guid, lastName, firstName, position, phone);

            // Act
            bool result = Employee.AjouterEmploye(employee, filePath, StartLines.employees);

            // Assert
            Assert.IsTrue(result);

        }

        #endregion

        #region SupprimerEmploye

        [TestMethod]
        public void SupprimerEmploye_ReturnsTrueIfFound()
        {
            // Arrange
            // nothing to arrange, the employee should already exist in the file from previous test


            // Act
            bool result = Employee.SupprimerEmploye(guid, filePath);

            // Assert
            Assert.IsTrue(result);
            string contents = File.ReadAllText(filePath);
            Assert.IsFalse(contents.Contains(guid.ToString()));
        }



        #endregion
    }



    [TestClass]
    [DoNotParallelize]
    public class MembersTests
    {
        // Test filePath
        string filePath = @"..\..\..\..\Bibliotheque\LibService\files\TestUnitaires_humans.csv";

        // Test vars
        Guid guid = Guid.Parse("b76636a0-eb19-412c-a159-0e037eaf8231");
        string lastName = "Hemingway";
        string firstName = "Ernest";
        string phone = "418-555-1234";

        #region Parse

        [TestMethod]
        public void Parse_FieldsValides_RetourneMembers()
        {
            // Arrange
            string[] fields = new string[] { guid.ToString(), lastName, firstName, phone };

            // Act
            var member = Members.Parse(fields);

            // Assert
            Assert.IsNotNull(member);
            Assert.AreEqual(guid, member.Guid);
            Assert.AreEqual(lastName, member.LastName);
            Assert.AreEqual(firstName, member.FirstName);
            Assert.AreEqual(phone, member.Phone);
        }

        #endregion

        #region AjouterMembre

        [TestMethod]
        public void AjouterMembre_ReturnsTrue()
        {
            // Arrange
            var member = new Members(guid, lastName, firstName, phone);

            // Act
            bool result = Members.AjouterMembre(member, filePath, StartLines.members);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region SupprimerMembre

        [TestMethod]
        public void SupprimerMembre_ReturnsTrueIfFound()
        {
            // Arrange
            // nothing to arrange, the member should already exist in the file from previous test

            // Act
            bool result = Members.SupprimerMembre(guid, filePath);

            // Assert
            Assert.IsTrue(result);
        }



        #endregion
    }

    #endregion

    #region FileWriter and FileReader test classes
    [TestClass]
    public class FileWriterTests
    {
        private string tempFile = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary file for each test
            tempFile = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.csv");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Remove the temporary file after each test
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        [TestMethod]
        public void EcrireLigne()
        {
            string header = "Id,Name";
            string section = "TestSection";
            string line = $"{Guid.NewGuid()},John Doe";

            FileWriter.EcrireLigne(header, tempFile, section, line);

            // Check file exists
            Assert.IsTrue(File.Exists(tempFile));

            var content = File.ReadAllText(tempFile);

        }

        [TestMethod]
        public void Supprimer()
        {
            string section = "TestSection";
            Guid guid = Guid.NewGuid();
            string header = "Id,Name";
            string line = $"{guid},Jane Doe";

            // Write initial content
            FileWriter.EcrireLigne(header, tempFile, section, line);

            // Delete the line
            bool result = FileWriter.Supprimer(tempFile, section, guid);

            Assert.IsTrue(result);

        }
    }


    [TestClass]
    public class FileReaderTests
    {
        private string tempFolder = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            tempFolder = Path.Combine(Path.GetTempPath(), $"FileReaderTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempFolder);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);
        }

        [TestMethod]
        public void ConvertLinesToDataArrayList()
        {
            // Arrange
            string filePath = Path.Combine(tempFolder, "books.csv");
            string header = StartLines.books;
            string dataLine = "MyBook,John Doe,2025";
            File.WriteAllLines(filePath, new string[] { header, dataLine });

            // Act
            var result = FileReader.ConvertLinesToDataArrayList(filePath);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IsValidFile_ReturnsTrue_ForValidBookFile()
        {
            string filePath = Path.Combine(tempFolder, "books.csv");
            string header = StartLines.books;
            string dataLine = "3b2d50b1-c766-4415-b891-76aaf589de16,Germinal,Émile Zola,Naturalism,1885-04-26,2070378274,,true";
            File.WriteAllLines(filePath, new string[] { header, dataLine });

            bool isValid = FileReader.IsValidFile(filePath, out string? error);

            Assert.IsTrue(isValid);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void GetTxtFilenames_ReturnsCsvFilesOnly_WhenCsvOnlyTrue()
        {
            // Arrange
            string csvFile = Path.Combine(tempFolder, "file1.csv");
            string txtFile = Path.Combine(tempFolder, "file2.txt");
            File.WriteAllText(csvFile, "header,data");
            File.WriteAllText(txtFile, "ignore");

            // Act
            var files = FileReader.GetTxtFilenames(tempFolder, csvOnly: true);

            // Assert
            CollectionAssert.Contains(files, "file1.csv");
            CollectionAssert.DoesNotContain(files, "file2.txt");
        }
    }
}
#endregion