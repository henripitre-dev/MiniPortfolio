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
    public class MembreController : ControllerBase
    {
        /// <summary>
        /// Récupère un Membre spécifique à l'aide de son identifiant unique.
        /// </summary>
        /// <param name="id">L'identifiant numérique du Memrbe à obtenir. </param>
        /// <returns>L'objet Membre correspondant à l'ID. </returns>
        /// <response code="200">Membre trouvé et retourné. </response>
        /// <response code="404">Aucun membre trouvé avec cet identifiant. </response>
        /// <response code="500">Erreur interne du serveur. </response>
        [HttpGet("{id}")]
        [CustomAuthorize(Roles.Admin)]
        [ApiExplorerSettings(IgnoreApi = true)]//Ceci cache cette méthode et empêche swagger de créer une route. elle est cependant utilisable a l'intérieur par appel, comme dans la méthode POST par exemple
        public ActionResult<Membre> Get(int id)
        {
            try
            {
                Membre? m = new DAL().MembreFactory.Get(id);

                if (m == null)
                    return NotFound($"Aucun membre trouvé avec l'identifiant {id}"); //404 Not Found

                return Ok(m); //200 OK retourner auteur
            }
            catch (Exception ex) { return StatusCode(500, $"Erreur interne : {ex.Message}"); } //500 erreur interne
        }

        /// <summary>
        /// Crée un nouveau Membre dans la base de données.
        /// api/v1/auteur/ + body
        /// </summary>
        ///<param name="m">L'objet Membre à insérer.</param>
        /// <response code="201">Le Membre a été créé avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        /// <response code="500">Erreur interne du serveur. </response>///
        [HttpPost]
        [CustomAuthorize(Roles.Admin)]
        public ActionResult<Membre> Post([FromBody] Membre? m = null)
        {
            try
            {
                if (m == null)
                    return BadRequest("Le corps de la requête ne peut pas être vide."); //400 Bad Request ()formulation

                m.Id = 0;
                // On force l'ID à 0 pour éviter qu'un client malveillant impose un ID arbitraire;
                //*** la BD génère le vrai ID
                new DAL().MembreFactory.Save(m);

                // CreatedAtAction: retourne 201 Created un en-tête "Location" contenant L'URL
                // de la nouvelle ressource (ex.: /api/vl/fruit/7), conformément aux standards REST
                return CreatedAtRoute(
                    routeName: null,
                    routeValues: new { id = m.Id },
                    value: m
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne {ex.Message}");
            }
        }
    }
}
