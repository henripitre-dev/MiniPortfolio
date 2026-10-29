namespace ApiBiblio_Serveur.Models
{
    public class Emprunt
    {
        public int Id { get; set; }
        public DateTime? DateEmprunt { get; set; }
        public DateTime? DateRetour { get; set; }
        public int LivreId { get; set; }
        public int MembreId { get; set; }
        public Emprunt() { }

        public Emprunt(int id, DateTime dateEmprunt, DateTime dateRetour, int livreId, int membreId)
        {
            Id = id;
            DateEmprunt = dateEmprunt;
            DateRetour = dateRetour;
            LivreId = livreId;
            MembreId = membreId;
        }


    
    }
}
