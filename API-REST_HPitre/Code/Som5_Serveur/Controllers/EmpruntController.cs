using ApiBiblio_Serveur.DataAccessLayer;
using ApiBiblio_Serveur.Models;
using ApiBiblio_Serveur.Security.Authorisation;
using ApiBiblio_Serveur.Security.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiBiblio_Serveur.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [CustomAuthorize(Roles.Admin)]
    public class EmpruntController : ControllerBase
    {
        /// <summary>
        /// Récupère un Emprunt spécifique à l'aide de son identifiant unique.
        /// </summary>
        /// <param name="id">L'identifiant numérique du Emprunt à obtenir. </param>
        /// <returns>L'objet Emprunt correspondant à l'ID. </returns>
        /// <response code="200">Emprunt trouvé et retourné. </response>
        /// <response code="404">Aucun auteur trouvé avec cet identifiant. </response>
        /// <response code="500">Erreur interne du serveur. </response>
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)] //Ceci cache cette méthode et empêche swagger de créer une route. elle est cependant utilisable a l'intérieur par appel, comme dans la méthode POST par exemple
        public ActionResult<Emprunt> Get(int id)
        {
            try
            {
                Emprunt? e = new DAL().EmpruntFactory.Get(id);

                if (e == null)
                    return NotFound($"Aucun emprunt trouvé avec l'identifiant {id}"); //404 Not Found

                return Ok(e); //200 OK retourner auteur
            }
            catch (Exception ex) { return StatusCode(500, $"Erreur interne : {ex.Message}"); } //500 erreur interne
        }



        /// <summary>
        /// Crée un nouvel Emprunt dans la base de données.
        /// api/v1/auteur/ + body
        /// </summary>
        ///<param name="e">L'objet Emprunt à insérer.</param>
        /// <response code="201">Le Emprunt a été créé avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        /// <response code="500">Erreur interne du serveur. </response>///
        [HttpPost]
        public ActionResult<Emprunt> Post([FromBody] Emprunt? e)
        {
            try
            {
                if (e == null)
                    return BadRequest(new { message = "Le corps de la requête ne peut pas être vide." });

                if (e.DateEmprunt == default)
                    return BadRequest(new { message = "DateEmprunt est obligatoire." });

                if (e.LivreId <= 0)
                    return BadRequest(new { message = "LivreId invalide." });

                if (e.MembreId <= 0)
                    return BadRequest(new { message = "MembreId invalide." });

                if (e.DateRetour != default && e.DateRetour < e.DateEmprunt)
                    return BadRequest(new { message = "DateRetour doit être >= DateEmprunt." });

                e.Id = 0;

                new DAL().EmpruntFactory.Save(e);

                return CreatedAtRoute(
                    routeName: null,
                    routeValues: new { id = e.Id },
                    value: e
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un emprunt de l'inventaire en fournissant son identifiant unique.
        /// api/v1/emprunt/{id}
        /// </summary>
        /// <param name="id">L'identifiant unique de l'emprunt à supprimer. </param>
        /// <response code="204">Suppression reussie (aucun contenu retourne) .</response>
        /// <response code="404">Aucun emprunt trouvé avec cet identifiant. </response>
        /// <response code="409">L'emprunt est en cours d'utilisation ou entraîne un conflit de suppression. </response> //TODO : implement 409
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                DAL dal = new DAL();

                // Vérification d'existence
                Emprunt? e = dal.EmpruntFactory.Get(id);
                if (e == null)
                    return NotFound($"Aucun emprunt trouvé avec l'identifiant {id}");

                dal.EmpruntFactory.Delete(id);

                // Convention REST : DELETE reussi retourne 204 No Content
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }
    }
}
