using MySql.Data.MySqlClient;
using ApiBiblio_Serveur.Models;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories
{
    public class AuteurFactory
    {
        private Auteur CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            string nom = mySqlDataReader["Nom"].ToString();

            return new Auteur(id, nom);
        }

        public Auteur CreateEmpty()
        {
            return new Auteur(0, string.Empty);
        }

        public Auteur[] GetAll()
        {
            List<Auteur> auteurs = new List<Auteur>();
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM projet_auteur";

                mySqlDataReader = mySqlCmd.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    auteurs.Add(CreateFromReader(mySqlDataReader));
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return auteurs.ToArray();
        }
        public Auteur Get(int id)
        {
            Auteur auteur = null;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM projet_auteur where Id = @Id";
                mySqlCmd.Parameters.AddWithValue("@Id", id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    auteur = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return auteur;
        }

        public void Save(Auteur auteur)
        {
            MySqlConnection mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    if (auteur.Id == 0)
                    {
                        // On sait que c'est un nouveau produit avec Id == 0,
                        // car c'est ce que nous avons affecter dans la fonction CreateEmpty().
                        mySqlCmd.CommandText = "INSERT INTO projet_auteur(Nom) " +
                                               "VALUES (@Nom)";
                    }
                    else
                    {
                        mySqlCmd.CommandText = "UPDATE projet_auteur " +
                                               "SET Nom=@Nom " +
                                               "WHERE Id=@Id";

                        mySqlCmd.Parameters.AddWithValue("@Id", auteur.Id);
                    }

                    mySqlCmd.Parameters.AddWithValue("@Nom", auteur.Nom.Trim());

                    mySqlCmd.ExecuteNonQuery();

                    if (auteur.Id == 0)
                    {
                        // Si c'était un nouveau produit (requête INSERT),
                        // nous affectons le nouvel Id de l'instance au cas où il serait utilisé dans le code appelant.
                        auteur.Id = (int)mySqlCmd.LastInsertedId;
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
                    mySqlCmd.CommandText = "DELETE FROM projet_auteur WHERE Id=@Id";
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
