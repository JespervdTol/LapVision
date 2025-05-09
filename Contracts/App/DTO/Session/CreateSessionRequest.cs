using System.ComponentModel.DataAnnotations;

namespace Contracts.App.DTO.Session
{
    public class CreateSessionRequest
    {
        [Required]
        public int CircuitID { get; set; }

        [Required]
        [Range(1, 100)]
        public int NumberOfHeats { get; set; }
    }
}