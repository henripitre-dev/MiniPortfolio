using ApiBiblio_Serveur.Models;
using MySql.Data.MySqlClient;

namespace ApiBiblio_Serveur.DataAccessLayer.Factories
{
    public class QuotaApiFactory
    {
        private QuotaApi CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int membreid = (int)mySqlDataReader["MembreId"];
            int reqUtil = (int)mySqlDataReader["RequetesUtilisees"];
            int reqMax = (int)mySqlDataReader["MaxRequetes"];
            DateTime dateMaj = (DateTime)mySqlDataReader["DateMAJ"];

            return new QuotaApi(membreid, reqUtil, reqMax, dateMaj);
        }

        public QuotaApi? GetFromMemberID(int memberId)
        {
            using var mySqlCnn = new MySqlConnection(DAL.ConnectionString);
            mySqlCnn.Open();

            using var mySqlCmd = mySqlCnn.CreateCommand();
            mySqlCmd.CommandText = @"
        SELECT *
        FROM projet_quota_api
        WHERE MembreId = @MembreId";
            mySqlCmd.Parameters.AddWithValue("@MembreId", memberId);

            using var mySqlDataReader = mySqlCmd.ExecuteReader();

            if (mySqlDataReader.Read())
                return CreateFromReader(mySqlDataReader);

            return null;
        }

        /// <summary>
        /// If Id of the new quota equals 0, it will insert a new quota for the member. Else, it will update the corresponding quota.
        /// </summary>
        /// <param name="newQuota"></param>
        public void SaveOrUpdateQuota(QuotaApi newQuota)
        {
            MySqlConnection mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    if (newQuota.Id == 0)
                    {
                        // Nouvel emprunt
                        mySqlCmd.CommandText =
                            "INSERT INTO projet_quota_api(MembreId, DateMAJ, MaxRequetes, RequetesUtilisees) " +
                            "VALUES (@MembreId, @DateMaj, @MaxRequetes, @Requetes)";
                    }
                    else
                    {
                        // Mise à jour d'un emprunt existant
                        mySqlCmd.CommandText =
                            "UPDATE projet_quota_api " +
                            "SET MembreId=@MembreId, DateMAJ=@DateMaj, MaxRequetes=@MaxRequetes, RequetesUtilisees=@Requetes " +
                            "WHERE Id=@Id ";

                        mySqlCmd.Parameters.AddWithValue("@Id", newQuota.Id);
                    }

                    // Ajout des paramètres communs
                    mySqlCmd.Parameters.AddWithValue("@MembreId", newQuota.MembreId);
                    mySqlCmd.Parameters.AddWithValue("@DateMaj", newQuota.DateMAJ);
                    mySqlCmd.Parameters.AddWithValue("@MaxRequetes", newQuota.MaxRequetes);
                    mySqlCmd.Parameters.AddWithValue("@Requetes", newQuota.RequetesUtilisees);

                    // Exécution de la commande
                    mySqlCmd.ExecuteNonQuery();

                    if (newQuota.Id == 0)
                    {
                        // Affectation du nouvel Id si c'est une nouvelle insertion
                        newQuota.Id = (int)mySqlCmd.LastInsertedId;
                        return;
                    }
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }

    }
}
