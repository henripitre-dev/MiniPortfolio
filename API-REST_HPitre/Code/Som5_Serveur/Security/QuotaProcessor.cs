using ApiBiblio_Serveur.DataAccessLayer;
using ApiBiblio_Serveur.DataAccessLayer.Factories;
using ApiBiblio_Serveur.Models;
using System;
using System.Security.Cryptography.X509Certificates;

namespace ApiBiblio_Serveur.Security
{
    public static class QuotaProcessor
    {
        /*
            Vous devez faire l’implémentation d’une mécanique servant à limiter un utilisateur à un nombre
            d’appels par période. Pour rendre ceci testable, j’ai mis par défaut un maximum de 20 appels par
            membre. Vous devez vérifier que ces 20 appels sont faits par période de 24h. Si la période de 24h est
            passée, vous devez réinitialiser le champ DateMAJ à la date et l’heure actuelle, et réinitialiser le champ
            RequêtesUtilisées à 0.
         */

        public static DateTime? GetResetDate(string apikey)
        {
            DAL dal = new();
            QuotaApiFactory quotaApiFactory =new();
            //Get member corresponding
            int? mId = dal.MembreFactory.GetIdFromApiKey(apikey);

            if (mId != null)
            {
                QuotaApi q = quotaApiFactory.GetFromMemberID((int)mId);
                if (q != null) return q.DateMAJ.AddDays(1);
            }

            return null;
        }

        public static QuotaApi? GetFromApi(string apikey)
        {
            DAL dal = new();
            QuotaApiFactory quotaApiFactory = new();
            //Get member corresponding
            int? mId = dal.MembreFactory.GetIdFromApiKey(apikey);

            if (mId != null)
            {
                QuotaApi q = quotaApiFactory.GetFromMemberID((int)mId);
                if (q != null ) return q;
            }

            return null;
        }

        public static bool ValidateQuota(string apikey)
        {
            DAL dal = new();
            QuotaApiFactory quotaApiFactory = new();

            //Get member corresponding
            int? mId = dal.MembreFactory.GetIdFromApiKey(apikey);

            if (mId != null)
            {
                QuotaApi q = quotaApiFactory.GetFromMemberID((int)mId);
                if (q == null) return false;

                if ((DateTime.Now - q.DateMAJ).TotalHours > 24)
                {
                    //Last update 24 hours ago : Reset requests
                    q.DateMAJ = DateTime.Now;
                    q.RequetesUtilisees = 0;
                    quotaApiFactory.SaveOrUpdateQuota(q);
                }

                if (q.MaxRequetes > q.RequetesUtilisees)
                    return true;
            }

            //Member/quota wasn't found or max requests hit, return invalid
            return false;
        }
    }
}
