using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape3.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape3.Models
{
    [PrimaryKey(nameof(X), nameof(Y))]
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public string TypeTxt { get; set; }
        public bool IsTraversable { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }

        public Tile(int x, int y, TileType type, bool isTraversable, string description, string imgUrl)
        {
            X = x;
            Y = y;
            Type = type;
            TypeTxt = Enum.GetName(type) ?? "N/A";
            IsTraversable = isTraversable;
            Description = description;
            ImgUrl = imgUrl;
        }
    }
}