namespace Model.Entities.CoachWeb
{
    public class Person
    {
        public int PersonID { get; private set; }
        public string FirstName { get; private set; }
        public string Prefix { get; private set; }
        public string LastName { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public byte[]? ProfilePicture { get; private set; }

        public int AccountID { get; private set; }
        public Account Account { get; private set; }

        public Person(string firstName, string prefix, string lastName, DateOnly dateOfBirth)
        {
            FirstName = firstName;
            Prefix = prefix;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public void SetAccount(Account account)
        {
            Account = account;
            AccountID = account.AccountID;
        }

        public void SetProfilePicture(byte[]? profilePicture)
        {
            ProfilePicture = profilePicture;
        }
    }
}