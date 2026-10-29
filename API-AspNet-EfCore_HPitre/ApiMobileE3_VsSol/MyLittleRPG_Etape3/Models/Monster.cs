using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape3.Models
{
    public class Monster
    {
        //EF Core reconnait "Id" comme clé primaire par convention, ou "IdClasse"
        [Key]
        public int idMonster { get; set; }

        public string Nom { get; set; }

        public int PointsVieBase { get; set; }

        public int ForceBase { get; set; }

        public int DefenseBase { get; set; }

        public int ExperienceBase { get; set; }

        public string SpriteUrl { get; set; }

        public string Type1 { get; set; }

        public string? Type2 { get; set; }
    }
}