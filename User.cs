namespace CSDS_Assign_1
{
    public class User
    {
        // Properties
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        // Constructor to initialize properties
        public User(int id, string username, string password, string role)
        {
            Id = id;
            Username = username;
            Password = password;
            Role = role;
        }

    }
}
