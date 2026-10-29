namespace MyLittleRPG_Etape3.Models.Enums
{
    public enum TileType
    {
        Herbe,
        Eau,
        Montagne,
        Forêt,
        Ville,
        Route
    }
    public static class TileTypeExtensions
    {
        public static bool IsTraversable(this TileType t) => t != TileType.Eau && t != TileType.Montagne;

        public static string ImgUrl(this TileType t)
        {
            return t switch
            {
                TileType.Herbe => "/images/Plains.png",
                TileType.Eau => "/images/River.png",
                TileType.Montagne => "/images/Mountain.png",
                TileType.Forêt => "/images/Forest.png",
                TileType.Ville => "/images/Town.png",
                TileType.Route => "/images/Road.png",
                _ => string.Empty,
            };
        }

        public static int Weight(this TileType t)
        {
            return t switch
            {
                TileType.Herbe => 20,
                TileType.Eau => 10,
                TileType.Montagne => 15,
                TileType.Forêt => 15,
                TileType.Ville => 5,
                TileType.Route => 35,
                _ => 0,
            };
        }
    }
}
