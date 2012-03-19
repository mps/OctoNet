using RestSharp;

namespace OctoNet.Authentication
{
    public class NullAuthenticator : IAuthenticator
    {
        #region IAuthenticator Members

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            // NOOP
        }

        #endregion
    }
}