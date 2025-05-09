using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class Heat
    {
        [Key]
        public int HeatID { get; set; }

        [Required]
        public int SessionID { get; set; }

        [ForeignKey(nameof(SessionID))]
        public Session Session { get; set; }

        [Required]
        public int HeatNumber { get; set; }

        public ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();
    }
}