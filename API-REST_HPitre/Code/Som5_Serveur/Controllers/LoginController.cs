using ApiBiblio_Serveur.DataAccessLayer;
using ApiBiblio_Serveur.Models;
using ApiBiblio_Serveur.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBiblio_Serveur.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authentication]
    public class LoginController : ControllerBase
    {
        DAL dal;
        public LoginController()
        {
            dal = new DAL();
        }

        [AllowAnonymous]
        [HttpGet("{nom}/{password}")]
        public ActionResult<string> Get(string nom, string password)
        {
            try
            {
                string api = new DAL().LoginFactory.Get(nom, password);

                if (string.IsNullOrWhiteSpace(api))
                    return Unauthorized("Identifiants invalides."); //401 Unauthorized

                return Ok(api); //200 OK retourner user
            }
            catch (Exception ex) { return StatusCode(500, $"Erreur interne : {ex.Message}"); } //500 erreur interne
        }
    }
}
