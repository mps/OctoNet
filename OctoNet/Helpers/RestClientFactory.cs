using RestSharp;

namespace OctoNet.Helpers
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient(string baseUrl);
    }

    public class RestClientFactory : IRestClientFactory
    {
        #region IRestClientFactory Members

        public IRestClient CreateRestClient(string baseUrl)
        {
            var restClient = new RestClient(baseUrl);

            restClient.UseSynchronizationContext = false;

            return restClient;
        }

        #endregion
    }
}