using ApiBiblio_Serveur.DataAccessLayer;
using ApiBiblio_Serveur.Models;
using ApiBiblio_Serveur.Security.Authorisation;
using ApiBiblio_Serveur.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiBiblio_Serveur.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [CustomAuthorize]
    public class LivreController : ControllerBase
    {
        /// <summary>
        /// Récupère la liste complète des livres.
        /// </summary>
        /// <returns>Une liste de tous les objets Livre.</returns>
        /// <response code="200">La liste a été récupérée avec succès. </response>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Livre>> Get([FromQuery] bool? disponible = null) //FROMQUERY: Permet d'ajouter un param optionnel dans l'url
        {
            try
            {
                DAL dal = new();

                List<Livre> livres = [.. dal.LivreFactory.GetAll()];

                if (disponible.HasValue)
                {
                    List<Emprunt> empruntsActifs = [.. dal.EmpruntFactory.GetAll().Where(e => e.DateRetour >= DateTime.Now)]; //Change made using tests

                    if (disponible.Value)
                        livres = [.. livres.Where(l => !empruntsActifs.Any(e => e.LivreId == l.Id))];
                    else
                        livres = [.. livres.Where(l => empruntsActifs.Any(e => e.LivreId == l.Id))];
                }

                return Ok(livres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère un Livre spécifique à l'aide de son identifiant unique.
        /// </summary>
        /// <param name="id">L'identifiant numérique du Livre à obtenir. </param>
        /// <returns>L'objet Livre correspondant à l'ID. </returns>
        /// <response code="200">Livre trouvé et retourné. </response>
        /// <response code="404">Aucun livre trouvé avec cet identifiant. </response>
        /// <response code="500">Erreur interne du serveur. </response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<Livre> Get(int id)
        {
            try
            {
                Livre? l = new DAL().LivreFactory.Get(id);

                if (l == null)
                    return NotFound($"Aucun livre trouvé avec l'identifiant {id}"); //404 Not Found

                return Ok(l); //200 OK retourner livre
            }
            catch (Exception ex) { return StatusCode(500, $"Erreur interne : {ex.Message}"); } //500 erreur interne
        }

        #region Get() Derivates

        /// <summary>
        /// Récupère la liste des livres d'un auteur.
        /// </summary>
        /// <param name="auteurId">Identifiant de l'auteur.</param>
        /// <returns>Liste des livres de cet auteur.</returns>
        /// <response code="200">Livres retournés.</response>
        /// <response code="404">Aucun livre trouvé.</response>
        /// <response code="500">Erreur interne.</response>
        [HttpGet("auteur/{auteurId}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Livre>> GetByAuteur(int auteurId)
        {
            try
            {
                List<Livre> livres = [.. new DAL().LivreFactory.GetAll()
            .Where(l => l.AuteurId == auteurId)];

                if (livres.Count == 0)
                    return NotFound($"Aucun livre trouvé pour l'auteur {auteurId}");

                return Ok(livres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère la liste des livres appartenant à une catégorie.
        /// </summary>
        /// <param name="auteurId">Identifiant de la catégorie.</param>
        /// <returns>Liste des livres de cet auteur.</returns>
        /// <response code="200">Livres retournés.</response>
        /// <response code="404">Aucun livre trouvé.</response>
        /// <response code="500">Erreur interne.</response>
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Livre>> GetByCategory(int categoryId)
        {
            try
            {
                List<Livre> livres = [.. new DAL().LivreFactory.GetAll()
            .Where(l => l.CategorieId == categoryId)];

                if (livres.Count == 0)
                    return NotFound($"Aucun livre trouvé pour la catégorie {categoryId}");

                return Ok(livres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }


        /// <summary>
        /// Récupère les livres empruntés par un membre.
        /// </summary>
        [HttpGet("membre/{membreId}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<Livre>> GetByMembre(int membreId)
        {
            try
            {
                DAL dal = new();

                List<Emprunt> emprunts = [.. dal.EmpruntFactory.GetAll().Where(e => e.MembreId == membreId && e.DateRetour >= DateTime.Now)];

                if (emprunts.Count == 0)
                    return NotFound($"Aucun livre emprunté par le membre {membreId}");

                List<int> livresIds = [.. emprunts.Select(e => e.LivreId)];

                List<Livre> livres = [.. dal.LivreFactory.GetAll().Where(l => livresIds.Contains(l.Id))];

                return Ok(livres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }
        #endregion

        /// <summary>
        /// Crée un nouveau Livre dans la base de données.
        /// </summary>
        ///<param name="l">L'objet Livre à insérer.</param>
        /// <response code="201">Le Livre a été créé avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        /// <response code="500">Erreur interne du serveur. </response>///
        [HttpPost]
        [CustomAuthorize(Roles.Admin)]
        public ActionResult<Livre> Post([FromBody] Livre? l = null)
        {
            try
            {
                if (l == null)
                    return BadRequest("Le corps de la requête ne peut pas être vide."); //400 Bad Request ()formulation
                if (string.IsNullOrWhiteSpace(l.ISBN) ||
                    string.IsNullOrWhiteSpace(l.Titre) ||
                    l.NbPages <= 0 ||
                    l.AuteurId <= 0 ||
                    l.CategorieId <= 0)
                {
                    return BadRequest(new { message = "Données invalides." });
                }

                l.Id = 0;
                // On force l'ID à 0 pour éviter qu'un client malveillant impose un ID arbitraire;
                //*** la BD génère le vrai ID
                new DAL().LivreFactory.Save(l);

                // CreatedAtAction: retourne 201 Created
                // un en-tête "Location" contenant L'URL de la nouvelle ressource (ex.: /api/vl/fruit/7), conformément aux standards REST
                return CreatedAtRoute(
                    routeName: null,
                    routeValues: new { id = l.Id },
                    value: l
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne {ex.Message}");
            }
        }

        /// <summary>
        /// Modifie un Livre existant dans la base de données en utilisant son identifiant unique.
        /// </summary>
        /// <param name="id">Livre à modifier</param>
        /// <param name="l">Objet livre modifié</param>
        /// <response code="200">Le Livre a été modifié avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        /// <response code="500">Erreur interne du serveur. </response>///
        [HttpPut("{id}")]
        [CustomAuthorize(Roles.Editor, Roles.Admin)] //Admin devrait logiquement être capable, si un user
        public IActionResult Put(int id, [FromBody] Livre? l)
        {
            try
            {
                if (l == null) return BadRequest("Le corps de la requête ne peut pas être vide.");

                DAL dal = new();

                //Vérif existence avant modif
                Livre livreAModifier = dal.LivreFactory.Get(id);
                if (livreAModifier == null)
                    return NotFound($"Aucun livre trouvé avec {id}");

                ////Exemple validation dans le controller
                //List<string> couleursPermises = ["Rouge", "Jaune", "Vert", "Orange", "Bleu"];
                //if (!couleursPermises.Contains(fruit.Couleur, StringComparer.OrdinalIgnoreCase))
                //    return BadRequest($"Couleur invalide. Couleurs acceptées : {string.Join(", ", couleursPermises)}");

                l.Id = id;
                dal.LivreFactory.Save(l);
                return Ok(l);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }


        /// <summary>
        /// Supprime un livre de l'inventaire en fournissant son identifiant unique.
        /// api/v1/livre/{id}
        /// </summary>
        /// <param name="id">L'identifiant unique du livre à supprimer. </param>
        /// <response code="204">Suppression reussie (aucun contenu retourne) .</response>
        /// <response code="404">Aucun livre trouve avec cet identifiant. </response>
        /// <response code="409">Le livre est en cours d'utilisation ou entraîne un conflit de suppression. </response> //TODO : implement 409
        [HttpDelete("{id}")]
        [CustomAuthorize(Roles.Admin)]
        public IActionResult Delete(int id)
        {
            try
            {
                DAL dal = new DAL();

                // Vérification d'existence
                Livre? l = dal.LivreFactory.Get(id);
                if (l == null)
                    return NotFound($"Aucun livre trouvé avec l'identifiant {id}");

                dal.LivreFactory.Delete(id);

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
