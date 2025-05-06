using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class Session
    {
        [Key]
        public int SessionID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CircuitID { get; set; }

        [ForeignKey(nameof(CircuitID))]
        public Circuit Circuit { get; set; }

        [Required]
        public int AccountID { get; set; }

        [ForeignKey(nameof(AccountID))]
        public Account Account { get; set; }

        public ICollection<Heat> Heats { get; set; } = new List<Heat>();

        public void GenerateHeats(int numberOfHeats)
        {
            Heats.Clear();
            for (int i = 1; i <= numberOfHeats; i++)
            {
                Heats.Add(new Heat { HeatNumber = i });
            }
        }
    }
}