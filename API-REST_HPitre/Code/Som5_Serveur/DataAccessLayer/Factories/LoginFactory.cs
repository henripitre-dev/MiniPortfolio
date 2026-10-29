using MySql.Data.MySqlClient;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories
{
    public class LoginFactory
    {
        private string CreateFromReader(MySqlDataReader mySqlDataReader)
        {

            string apiKey = mySqlDataReader["ApiKey"].ToString();

            return apiKey;
        }

        public string Get(string nom, string password)
        {
            string apikey = null;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = @"SELECT k.ApiKey FROM projet_login_membre l JOIN projet_membre k ON l.membreid = k.Id WHERE l.username = @nom AND l.password = @Password;";
                mySqlCmd.Parameters.AddWithValue("@nom", nom);
                mySqlCmd.Parameters.AddWithValue("@Password", password);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    apikey = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return apikey;
        }
    }
}
