using MySql.Data.MySqlClient;
using ApiBiblio_Serveur.Models;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories
{



    public class EmpruntFactory
    {
        
        private Emprunt CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            DateTime dateEmprunt = (DateTime)mySqlDataReader["DateEmprunt"];
            DateTime dateRetour = (DateTime)mySqlDataReader["DateRetour"];
            int livreId = (int)mySqlDataReader["LivreId"];
            int membreId = (int)mySqlDataReader["MembreId"];

            return new Emprunt(id, dateEmprunt, dateRetour, livreId, membreId);
        }

        public Emprunt CreateEmpty()
        {
            return new Emprunt(0, DateTime.MinValue, DateTime.MinValue, 0, 0);
        }

        public Emprunt[] GetAll()
        {
            List<Emprunt> emprunts = new List<Emprunt>();
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM projet_emprunt ORDER BY DateEmprunt";

                mySqlDataReader = mySqlCmd.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    emprunts.Add(CreateFromReader(mySqlDataReader));
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return emprunts.ToArray();
        }

        public Emprunt Get(int id)
        {
            Emprunt emprunt = null;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM projet_emprunt WHERE Id = @Id";
                mySqlCmd.Parameters.AddWithValue("@Id", id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    emprunt = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return emprunt;
        }

        public void Save(Emprunt emprunt)
        {
            MySqlConnection mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    if (emprunt.Id == 0)
                    {
                        // Nouvel emprunt
                        mySqlCmd.CommandText =
                            "INSERT INTO projet_emprunt(DateEmprunt, DateRetour, LivreId, MembreId) " +
                            "VALUES (@DateEmprunt, @DateRetour, @LivreId, @MembreId)";
                    }
                    else
                    {
                        // Mise à jour d'un emprunt existant
                        mySqlCmd.CommandText =
                            "UPDATE projet_emprunt " +
                            "SET DateEmprunt=@DateEmprunt, DateRetour=@DateRetour, LivreId=@LivreId, MembreId=@MembreId " +
                            "WHERE Id=@Id";

                        mySqlCmd.Parameters.AddWithValue("@Id", emprunt.Id);
                    }

                    // Ajout des paramètres communs
                    mySqlCmd.Parameters.AddWithValue("@DateEmprunt", emprunt.DateEmprunt);
                    mySqlCmd.Parameters.AddWithValue("@DateRetour", emprunt.DateRetour);
                    mySqlCmd.Parameters.AddWithValue("@LivreId", emprunt.LivreId);
                    mySqlCmd.Parameters.AddWithValue("@MembreId", emprunt.MembreId);

                    // Exécution de la commande
                    mySqlCmd.ExecuteNonQuery();

                    if (emprunt.Id == 0)
                    {
                        // Affectation du nouvel Id si c'est un nouvel emprunt
                        emprunt.Id = (int)mySqlCmd.LastInsertedId;
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
                    // Suppression d'un emprunt par son Id
                    mySqlCmd.CommandText = "DELETE FROM projet_emprunt WHERE Id=@Id";
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