using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class CircuitLayoutPoint
    {
        [Key]
        public int CircuitLayoutPointID { get; set; }

        public int CircuitID { get; set; }

        [ForeignKey("CircuitID")]
        public Circuit Circuit { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OrderIndex { get; set; }
    }
}