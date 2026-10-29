using MySql.Data.MySqlClient;
using ApiBiblio_Serveur.Models;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories
{
    public class MembreFactory
    {
        private Membre CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            string nom = mySqlDataReader["Nom"].ToString();
            string courriel = mySqlDataReader["Courriel"].ToString();
            string telephone = mySqlDataReader["Telephone"].ToString();
            DateTime dateCreation = (DateTime)mySqlDataReader["DateCreation"];
            string apiKey = mySqlDataReader["ApiKey"].ToString();

            return new Membre(id, nom, courriel, telephone, dateCreation, apiKey);
        }
        public Membre CreateEmpty()
        {
            return new Membre(0, string.Empty, string.Empty, string.Empty, DateTime.MinValue, string.Empty);
        }

        public Membre[] GetAll()
        {
            List<Membre> membres = new List<Membre>();
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM projet_membre ORDER BY Nom";

                mySqlDataReader = mySqlCmd.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    membres.Add(CreateFromReader(mySqlDataReader));
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return membres.ToArray();
        }

        public Membre Get(int id)
        {
            Membre membre = null;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM projet_membre WHERE Id = @Id";
                mySqlCmd.Parameters.AddWithValue("@Id", id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    membre = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return membre;
        }

        public int GetIdFromApiKey(string apikey)
        {

            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;
            Membre theMember = new();

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = @"
                    SELECT * 
                    FROM projet_membre m
                    WHERE m.ApiKey = @Apikey";
                mySqlCmd.Parameters.AddWithValue("@Apikey", apikey);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    theMember = CreateFromReader(mySqlDataReader);
                }

            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            //Moins on retourne d'éléments, plus c'est sécuritaire.
            //Cette méthode sert seulement à obtenir l'ID pour QuotaProcessor,
            //alors seulement besoin de retourner l'id pour l'instant.
            return theMember.Id;
        }

        public void Save(Membre membre)
        {
            MySqlConnection mySqlCnn = null;
            QuotaApiFactory quotaApiFactory = new QuotaApiFactory();
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    if (membre.Id == 0)
                    {
                        // Nouvel membre
                        mySqlCmd.CommandText =
                            "INSERT INTO projet_membre(Nom, Courriel, Telephone, DateCreation, ApiKey) " +
                            "VALUES (@Nom, @Courriel, @Telephone, @DateCreation, @ApiKey)";

                        //Ajouter la row Quota correspondante
                        //quotaApiFactory.SaveOrUpdateQuota(new(membre.Id, 0, QuotaApi.MAX_REQUETES, DateTime.Now)); COMMENT OUT FOR TESTS
                    }
                    else
                    {
                        // Mise à jour d'un membre existant
                        mySqlCmd.CommandText =
                            "UPDATE projet_membre " +
                            "SET Nom=@Nom, Courriel=@Courriel, Telephone=@Telephone, DateCreation=@DateCreation, ApiKey=@ApiKey " +
                            "WHERE Id=@Id";

                        mySqlCmd.Parameters.AddWithValue("@Id", membre.Id);
                    }

                    // Ajout des paramètres communs
                    mySqlCmd.Parameters.AddWithValue("@Nom", membre.Nom.Trim());
                    mySqlCmd.Parameters.AddWithValue("@Courriel", membre.Courriel.Trim());
                    mySqlCmd.Parameters.AddWithValue("@Telephone", membre.Telephone.Trim());
                    mySqlCmd.Parameters.AddWithValue("@DateCreation", membre.DateCreation);
                    mySqlCmd.Parameters.AddWithValue("@ApiKey", membre.ApiKey.Trim());

                    // Exécution de la commande
                    mySqlCmd.ExecuteNonQuery();

                    if (membre.Id == 0)
                    {
                        // Affectation du nouvel Id si c'est un nouveau membre
                        membre.Id = (int)mySqlCmd.LastInsertedId;
                    }
                }
            }
            finally
            {
                if (mySqlCnn != null)
                {
                    mySqlCnn.Close();
                }
            }
        }

        public void Delete(int id)
        {
            MySqlConnection mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    // Suppression d'un membre par son Id
                    mySqlCmd.CommandText = "DELETE FROM projet_membre WHERE Id=@Id";
                    mySqlCmd.Parameters.AddWithValue("@Id", id);
                    mySqlCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (mySqlCnn != null)
                {
                    mySqlCnn.Close();
                }
            }
        }
    }
}
