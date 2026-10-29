using MySql.Data.MySqlClient;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories;

public class UtilityFactory
{
    public void Execute(string commandText)
    {
        MySqlConnection mySqlCnn = null;

        try
        {
            mySqlCnn = new MySqlConnection(DAL.ConnectionString);
            mySqlCnn.Open();

            MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
            mySqlCmd.CommandText = commandText;
            mySqlCmd.ExecuteNonQuery();
        }
        finally
        {
            mySqlCnn?.Close();
        }
    }
}