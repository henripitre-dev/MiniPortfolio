using ApiBiblio_Serveur.DataAccessLayer;
using MySql.Data.MySqlClient;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories
{
    public class RolesFactory
    {
        private List<string> CreateFromRead(MySqlDataReader mySqlDataReader)
        {
            List<string> roles = new List<string>();

            while (mySqlDataReader.Read())
            {
                roles.Add(mySqlDataReader["Role"].ToString()); // Récupère le nom du rôle
            }
            return roles;
        }
        public List<string> GetRolesFromApiKey(string apikey)
        {

            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;
            MembreFactory membreFactory = new();
            List<string> roles = new List<string>();

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                int mId = membreFactory.GetIdFromApiKey(apikey);

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = @"
                    SELECT mr.Role 
                    FROM projet_membre_roles mr
                    WHERE mr.MembreId = @memberId";
                mySqlCmd.Parameters.AddWithValue("@memberId",  mId);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                roles = CreateFromRead(mySqlDataReader);


            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return roles;
        }
    }
}
