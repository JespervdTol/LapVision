using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Person
    {
        [Key]
        public int PersonID { get; set; }

        public string FirstName { get; set; }
        public string Prefix { get; set; }
        public string LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public int AccountID { get; set; }
        public Account Account { get; set; }
    }
}