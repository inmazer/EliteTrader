namespace ThruddClient
{
    public class ThruddCredentials
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public ThruddCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
