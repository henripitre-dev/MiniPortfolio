namespace ApiBiblio_Serveur.Models
{
    public class Livre
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Titre { get; set; }
        public int NbPages { get; set; }
        public int AuteurId { get; set; }
        public int CategorieId { get; set; }

        public Livre() { }

        public Livre(int id, string isbn, string titre, int nbPages, int auteurId, int catId)
        {
            Id = id;
            ISBN = isbn;
            Titre = titre;
            NbPages = nbPages;
            AuteurId = auteurId;
            CategorieId = catId;
        }


    
    }
}
