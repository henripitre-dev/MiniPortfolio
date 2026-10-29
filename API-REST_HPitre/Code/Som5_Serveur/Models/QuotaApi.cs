namespace ApiBiblio_Serveur.Models
{
    public class QuotaApi
    {
        public readonly static int MAX_REQUETES = 20; 
        public int Id { get; set; }
        public int MembreId { get; set; }
        public int RequetesUtilisees { get; set; }
        public int MaxRequetes { get; set; }
        public DateTime DateMAJ { get; set; }

        public QuotaApi(int membreId, int requetesUtilisees, int requetesMax, DateTime dateMAJ)
        {
            MembreId = membreId;
            RequetesUtilisees = requetesUtilisees;
            MaxRequetes = requetesMax;
            DateMAJ = dateMAJ;
        }
    }
}