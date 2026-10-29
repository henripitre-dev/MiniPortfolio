namespace ApiBiblio_Serveur.Models
{
    public class Membre
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Courriel { get; set; }
        public string Telephone { get; set; }
        public DateTime DateCreation { get; set; }
        public string ApiKey { get; set; }
        public Membre() { }

        public Membre(int id, string nom, string courriel, string telephone, DateTime dateCreation, string apikey)
        {
            Id = id;
            Nom = nom;
            Courriel = courriel;   
            Telephone = telephone;
            DateCreation = dateCreation;
            ApiKey = apikey;
        }


    
    }
}
