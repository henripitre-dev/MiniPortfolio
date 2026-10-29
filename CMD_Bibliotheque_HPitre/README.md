# Bibliothèque

Application console .NET pour gérer une bibliothèque, avec des données enregistrées dans des fichiers CSV. Projet d'équipe réalisé avec Tommy-Frédéric Bernaquez.

![Menu principal, liste des livres disponibles, puis informations de la bibliothèque](docs/navigation.gif)

**Technologies :** C#, .NET 8, MSTest

## Fonctionnalités

- Livres : ajout, suppression, recherche par identifiant, auteur ou titre, prêt et retour
- Auteurs, membres et employés : ajout, suppression, liste et recherche de membres
- Lecture, validation et écriture des fichiers CSV
- Projet séparé de fonctions réutilisables pour l'affichage et la lecture des saisies au clavier

## Lancer l'application

- **Version prête :** `Release/Bibliotheque.exe` (Windows, avec le [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)).
- **Code :** ouvrir `Code/Projet2.sln` dans Visual Studio 2022. Les 28 tests unitaires se lancent avec *Test > Exécuter tous les tests*. Ils sont décrits dans [Document_Explication_ProjetdeTest.txt](Conception/Document_Explication_ProjetdeTest.txt).

## Conception

- [Diagramme de classes](Conception/Diagramme-de-classes.png)
