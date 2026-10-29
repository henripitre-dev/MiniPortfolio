-- API Bibliothèque
-- Base de démonstration pour le portfolio (MySQL 8). Exécuter tel quel : le script crée la base.

CREATE DATABASE IF NOT EXISTS `api_bibliotheque` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE `api_bibliotheque`;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `projet_auteur`
--

DROP TABLE IF EXISTS `projet_auteur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_auteur` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nom` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_auteur`
--

LOCK TABLES `projet_auteur` WRITE;
/*!40000 ALTER TABLE `projet_auteur` DISABLE KEYS */;
INSERT INTO `projet_auteur` VALUES (1,'David Goudreault'),(2,'Véronique Ovaldé'),(3,'Patrick Senécal'),(4,'Victoria Mas'),(5,'Francine Ruel');
/*!40000 ALTER TABLE `projet_auteur` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_categorie`
--

DROP TABLE IF EXISTS `projet_categorie`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_categorie` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nom` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_categorie`
--

LOCK TABLES `projet_categorie` WRITE;
/*!40000 ALTER TABLE `projet_categorie` DISABLE KEYS */;
INSERT INTO `projet_categorie` VALUES (1,'Bande dessinée'),(2,'Encyclopédie'),(3,'Journaux'),(4,'Roman'),(5,'Science fiction');
/*!40000 ALTER TABLE `projet_categorie` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_emprunt`
--

DROP TABLE IF EXISTS `projet_emprunt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_emprunt` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `DateEmprunt` datetime DEFAULT NULL,
  `DateRetour` datetime DEFAULT NULL,
  `LivreId` int NOT NULL,
  `MembreId` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=12 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_emprunt`
--

LOCK TABLES `projet_emprunt` WRITE;
/*!40000 ALTER TABLE `projet_emprunt` DISABLE KEYS */;
INSERT INTO `projet_emprunt` VALUES (1,'2026-03-30 19:57:18','2026-04-30 19:57:18',2,1),(2,'2026-03-30 20:00:30','2026-05-30 20:00:30',3,2),(6,'2026-03-30 20:13:22','2026-04-20 20:13:22',5,2),(7,'2026-02-14 20:16:39','2026-03-15 20:16:39',2,3);
/*!40000 ALTER TABLE `projet_emprunt` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_livre`
--

DROP TABLE IF EXISTS `projet_livre`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_livre` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ISBN` varchar(15) NOT NULL,
  `Titre` varchar(70) NOT NULL,
  `NbPages` int NOT NULL,
  `AuteurId` int NOT NULL,
  `CategorieId` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_livre`
--

LOCK TABLES `projet_livre` WRITE;
/*!40000 ALTER TABLE `projet_livre` DISABLE KEYS */;
INSERT INTO `projet_livre` VALUES (1,'B07X8RVQ73','Ta mort à moi!',273,1,4),(2,'B07KPB2CKG','Personne n\'a peur des gens qui sourient',257,2,4),(3,'B07ZFZX787','Ceux de là-bas',450,3,4),(4,'B07WGPDVN8','Le Bal des folles',186,4,4),(5,'B07WJRW7KY','Anna et l\'enfant-vieillard',142,5,4);
/*!40000 ALTER TABLE `projet_livre` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_login_membre`
--

DROP TABLE IF EXISTS `projet_login_membre`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_login_membre` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `membreid` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_login_membre`
--

LOCK TABLES `projet_login_membre` WRITE;
/*!40000 ALTER TABLE `projet_login_membre` DISABLE KEYS */;
INSERT INTO `projet_login_membre` VALUES (1,'admin','admin',1),(2,'user','user',2),(3,'editor','editor',3);
/*!40000 ALTER TABLE `projet_login_membre` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_membre`
--

DROP TABLE IF EXISTS `projet_membre`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_membre` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nom` varchar(30) NOT NULL,
  `Courriel` varchar(30) NOT NULL,
  `Telephone` varchar(20) NOT NULL,
  `DateCreation` datetime NOT NULL,
  `ApiKey` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=14 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_membre`
--

LOCK TABLES `projet_membre` WRITE;
/*!40000 ALTER TABLE `projet_membre` DISABLE KEYS */;
INSERT INTO `projet_membre` VALUES (1,'admin','alice.dupont@example.com','123-456-7890','2025-04-01 08:00:00','admin_apikey'),(2,'user','bob.martin@example.com','987-654-3210','2025-03-28 14:30:00','user_apikey'),(3,'editor','charlie.durand@example.com','456-789-1234','2025-03-15 10:15:00','editor_apikey');
/*!40000 ALTER TABLE `projet_membre` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_membre_roles`
--

DROP TABLE IF EXISTS `projet_membre_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_membre_roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MembreId` int NOT NULL,
  `Role` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_membre_roles`
--

LOCK TABLES `projet_membre_roles` WRITE;
/*!40000 ALTER TABLE `projet_membre_roles` DISABLE KEYS */;
INSERT INTO `projet_membre_roles` VALUES (1,1,'admin'),(2,2,'user'),(3,3,'editor');
/*!40000 ALTER TABLE `projet_membre_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projet_quota_api`
--

DROP TABLE IF EXISTS `projet_quota_api`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projet_quota_api` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MembreId` int NOT NULL,
  `DateMAJ` datetime NOT NULL,
  `MaxRequetes` int NOT NULL DEFAULT '20',
  `RequetesUtilisees` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projet_quota_api`
--

LOCK TABLES `projet_quota_api` WRITE;
/*!40000 ALTER TABLE `projet_quota_api` DISABLE KEYS */;
INSERT INTO `projet_quota_api` VALUES (1,1,'2026-01-01 00:00:00',20,0),(2,2,'2026-01-01 00:00:00',20,0),(3,3,'2026-01-01 00:00:00',20,0);
/*!40000 ALTER TABLE `projet_quota_api` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
