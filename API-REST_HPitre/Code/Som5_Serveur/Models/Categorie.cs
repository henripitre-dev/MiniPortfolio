namespace ApiBiblio_Serveur.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public Categorie() { }
        public Categorie(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }
    
    }
}
