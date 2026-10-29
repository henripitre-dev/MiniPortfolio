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
    public class AuteurController : ControllerBase
    {
        /// <summary>
        /// Récupère la liste complète des auteurs.
        /// api/v1/auteur
        /// </summary>
        /// <returns>Une liste de tous les objets Auteurs.</returns>
        /// <response code="200">La liste a été récupérée avec succès. </response>
        [HttpGet]
        public ActionResult<IEnumerable<Auteur>> Get() //FROMQUERY: Permet d'ajouter un param optionnel dans l'url
        {
            try
            {
                List<Auteur> auteurs = [.. new DAL().AuteurFactory.GetAll()];
                return Ok(auteurs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère un Auteur spécifique à l'aide de son identifiant unique.
        /// </summary>
        /// <param name="id">L'identifiant numérique du Auteur à obtenir. </param>
        /// <returns>L'objet Auteur correspondant à l'ID. </returns>
        /// <response code="200">Auteur trouvé et retourné. </response>
        /// <response code="404">Aucun auteur trouvé avec cet identifiant. </response>
        /// <response code="500">Erreur interne du serveur. </response>
        [HttpGet("{id}")]
        public ActionResult<Auteur> Get(int id)
        {
            try
            {
                Auteur? a = new DAL().AuteurFactory.Get(id);

                if (a == null)
                    return NotFound($"Aucun auteur trouvé avec l'identifiant {id}"); //404 Not Found

                return Ok(a); //200 OK retourner auteur
            }
            catch (Exception ex) { return StatusCode(500, $"Erreur interne : {ex.Message}"); } //500 erreur interne
        }

        /// <summary>
        /// Crée un nouveau Auteur dans la base de données.
        /// api/v1/auteur/ + body
        /// </summary>
        ///<param name="a">L'objet Auteur à insérer.</param>
        /// <response code="201">Le Auteur a été créé avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        /// <response code="500">Erreur interne du serveur. </response>///
        [HttpPost]
        [CustomAuthorize(Roles.Admin)]

        public ActionResult<Auteur> Post([FromBody] Auteur? a = null)
        {
            try
            {
                if (a == null)
                    return BadRequest("Le corps de la requête ne peut pas être vide."); //400 Bad Request ()formulation

                a.Id = 0;
                // On force l'ID à 0 pour éviter qu'un client malveillant impose un ID arbitraire;
                //*** la BD génère le vrai ID
                new DAL().AuteurFactory.Save(a);

                // CreatedAtAction: retourne 201 Created un en-tête "Location" contenant L'URL
                // de la nouvelle ressource (ex.: /api/vl/fruit/7), conformément aux standards REST
                return CreatedAtRoute(
                    routeName: null,
                    routeValues: new { id = a.Id },
                    value: a
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne {ex.Message}");
            }
        }

        /// <summary>
        /// Modifie un Auteur existant dans la base de données en utilisant son identifiant unique.
        /// </summary>
        /// <param name="id">Auteur à modifier</param>
        /// <param name="a">Objet auteur modifié</param>
        /// <response code="200">Le Auteur a été modifié avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        /// <response code="500">Erreur interne du serveur. </response>///
        [HttpPut("{id}")]
        [CustomAuthorize(Roles.Admin, Roles.Editor)]

        public IActionResult Put(int id, [FromBody] Auteur? a)
        {
            try
            {
                if (a == null) return BadRequest("Le corps de la requête ne peut pas être vide.");

                DAL dal = new();

                //Vérif existence avant modif
                Auteur auteurAModifier = dal.AuteurFactory.Get(id);
                if (auteurAModifier == null)
                    return NotFound($"Aucun auteur trouvé avec {id}");

                ////Exemple validation dans le controller
                //List<string> couleursPermises = ["Rouge", "Jaune", "Vert", "Orange", "Bleu"];
                //if (!couleursPermises.Contains(fruit.Couleur, StringComparer.OrdinalIgnoreCase))
                //    return BadRequest($"Couleur invalide. Couleurs acceptées : {string.Join(", ", couleursPermises)}");

                a.Id = id;
                dal.AuteurFactory.Save(a);
                return Ok(a);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }


        /// <summary>
        /// Supprime un auteur de l'inventaire en fournissant son identifiant unique.
        /// api/v1/auteur/{id}
        /// </summary>
        /// <param name="id">L'identifiant unique du auteur à supprimer. </param>
        /// <response code="204">Suppression reussie (aucun contenu retourne) .</response>
        /// <response code="404">Aucun auteur trouve avec cet identifiant. </response>
        /// <response code="409">Le auteur est en cours d'utilisation ou entraîne un conflit de suppression. </response> //TODO : implement 409
        [HttpDelete("{id}")]
        [CustomAuthorize(Roles.Admin)]
        public IActionResult Delete(int id)
        {
            try
            {
                DAL dal = new DAL();

                // Vérification d'existence
                Auteur? a = dal.AuteurFactory.Get(id);
                if (a == null)
                    return NotFound($"Aucun auteur trouvé avec l'identifiant {id}");

                dal.AuteurFactory.Delete(id);

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
