namespace ApiBiblio_Serveur.Models
{
    public class Auteur
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public Auteur() { }

        public Auteur(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }
    
    }
}
