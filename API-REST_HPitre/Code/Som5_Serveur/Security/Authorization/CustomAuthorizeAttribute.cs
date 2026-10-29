using ApiBiblio_Serveur.DataAccessLayer;
using ApiBiblio_Serveur.DataAccessLayer.Factories;
using ApiBiblio_Serveur.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiBiblio_Serveur.Security.Authorisation
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private List<string> _roles;

        public CustomAuthorizeAttribute(params string[] Roles)
        {
            _roles = new List<string>(Roles);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Vérification de l'attribut [AllowAnonymous]
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (hasAllowAnonymous)
                return; //Return ok without verifying apikey (even if there's none)

            CustomAuthorizeAttribute? apiKeyAttribute = context.ActionDescriptor.EndpointMetadata.OfType<CustomAuthorizeAttribute>().LastOrDefault()
                ?? throw new Exception("Ne devrait pas être null");

            // L'attribut peut être sur la classe et sur la méthode : la vérification se fait une seule fois
            // par requête (avec les rôles de l'attribut le plus précis), sinon le quota serait compté deux fois.
            if (context.HttpContext.Items.ContainsKey(nameof(CustomAuthorizeAttribute)))
                return;
            context.HttpContext.Items[nameof(CustomAuthorizeAttribute)] = true;

            List<string> rolesPermis = apiKeyAttribute._roles;

            // Récupération d'une clef d'api 
            string apiKey = context.HttpContext.Request.Headers["x-api-key"].FirstOrDefault() ?? "";

            // Vérification de la récupération de la clef d'api 
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var errorResponse = new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Il faut fournir la clé d'API."
                };
                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Instanciation du dal pour avoir accès à notre factory
            DAL dal = new DAL();
            List<string> roles = dal.RolesFactory.GetRolesFromApiKey(apiKey);

            // Comparaison de la liste de rôles pouvant accèder à la ressource avec les rôles de l'utilisateur
            if (roles == null || !rolesPermis.Intersect(roles).Any())
            {
                var errorResponse = new
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Votre clé d'API ne vous permet pas d'accéder à cette ressource."
                };
                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Comparaison de la liste de rôles pouvant accèder à la ressource avec les rôles de l'utilisateur
            if (!QuotaProcessor.ValidateQuota(apiKey))
            {
                var errorResponse = new
                {
                    StatusCode = StatusCodes.Status429TooManyRequests,
                    Message = $"Vous avez atteint la limite de vos requêtes." +
                                $"\nVotre limite se réinitialisera à : {QuotaProcessor.GetResetDate(apiKey)} "
                };
                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status429TooManyRequests
                };
                return;
            }

            //Ajouter la requête
            QuotaApiFactory quotaApiFactory = new();
            QuotaApi? q = QuotaProcessor.GetFromApi(apiKey);
            if (q != null)
            {
                q.RequetesUtilisees++;
                quotaApiFactory.SaveOrUpdateQuota(q);
            }

        }
    }
}
