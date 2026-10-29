using ApiBiblio_Serveur.DataAccessLayer;
using ApiBiblio_Serveur.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ApiBiblio_Serveur.Security.Authentication
{
    public class AuthenticationAttribute : Attribute, IAsyncActionFilter
    {


        private const string ApiKeyHeaderName = "x-api-key";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Vérification de l'attribut [AllowAnonymous]
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (hasAllowAnonymous)
            {
                await next(); //go next, ok
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                var errorResponse = new
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Il faut fournir la clé d'API."
                };
                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apikey = configuration.GetValue<string>(ApiKeyHeaderName) ?? "";

            if (!apikey.Equals(potentialApiKey))
            {
                var errorResponse = new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Votre clef d'api ne vous permet pas d'accèder à cette ressource"
                };
                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }


            await next();
        }
    }
}
