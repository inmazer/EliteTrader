namespace ThruddClient
{
    public class ConnectionInfo
    {
        public string ReqVerToken { get; private set; }
        public string AuthToken { get; private set; }

        public ConnectionInfo(string reqVerToken, string authToken)
        {
            ReqVerToken = reqVerToken;
            AuthToken = authToken;
        }
    }
}