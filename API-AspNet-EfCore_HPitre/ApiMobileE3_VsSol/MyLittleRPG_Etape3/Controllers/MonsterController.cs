using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape3.Data.Context;
using MyLittleRPG_Etape3.Models;

namespace MyLittleRPG_Etape3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonsterController : ControllerBase
    {
        private readonly RPGContext _context;
        public MonsterController(RPGContext context) => _context = context;


        //GET api/Monsters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Monster>>> GetMonster()
            => Ok(await _context.Monster.ToListAsync());

        //GET api/Monsters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Monster>> GetMonster(int id)
        {
            var monster = await _context.Monster.FindAsync(id);

            if (monster == null)
                return NotFound();

            return monster;
        }

        //PUT api/Monsters/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Monster>> PutMonster(int id, Monster m)
        {
            if (id != m.idMonster)
                return BadRequest();

            var newM = await _context.Monster.FindAsync(id);
            if (newM == null)
                return NotFound();

            newM.Nom = m.Nom;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonsterExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();

        }

        //POST api/Monsters
        [HttpPost]
        public async Task<ActionResult<Monster>> PostMonster(Monster m)
        {
            _context.Monster.Add(m);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMonster", new { id = m.idMonster }, m);
        }

        //PUT api/Monsters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Monster>> DeleteMonster(int id)
        {

            var m = await _context.Monster.FindAsync(id);
            if (m == null)
                return NotFound();

            _context.Monster.Remove(m);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        private bool MonsterExists(int id)
            => _context.Monster.Any(e => e.idMonster == id);
    }

}

