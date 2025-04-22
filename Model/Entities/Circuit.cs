using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Circuit
    {
        [Key]
        public int CircuitID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        public double StartLineLat { get; set; }
        public double StartLineLng { get; set; }
        public double RadiusMeters { get; set; } = 10;
    }
}