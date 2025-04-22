namespace Contracts.DTO.Session
{
    public class CreateSessionResponse
    {
        public int SessionID { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PersonID { get; set; } 
    }
}