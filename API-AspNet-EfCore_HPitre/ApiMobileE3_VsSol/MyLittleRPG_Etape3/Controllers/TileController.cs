using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyLittleRPG_Etape3.Data.Context;
using MyLittleRPG_Etape3.Models;
using MyLittleRPG_Etape3.Models.Enums;
using Weighted_Randomizer;

namespace MyLittleRPG_Etape3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TileController : ControllerBase
    {
        private readonly RPGContext _context;
        public TileController(RPGContext context) => _context = context;


        //GET api/Tiles/5
        [HttpGet("{x}/{y}")]
        public async Task<ActionResult<Tile>> GetTile(int x, int y)
        {
            var tile = await _context.Tile.FindAsync(x, y);

            if (tile == null)
            {
                //Créer nouvelle tuile
                //https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started

                DynamicWeightedRandomizer<string> r = [];
                foreach (TileType type in Enum.GetValues<TileType>())
                    r.Add(Enum.GetName(type) ?? "N/A", type.Weight());

                string chosenTypeTxt = r.NextWithReplacement();
                TileType current = (TileType)Enum.Parse(typeof(TileType), chosenTypeTxt);

                tile = new(x, y, current, current.IsTraversable(), $"Tuile {Enum.GetName(current)} en position ({x},{y}).", current.ImgUrl()); ;
            }

            return tile;
        }

    }

}

