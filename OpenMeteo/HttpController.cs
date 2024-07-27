using System.Net.Http;

namespace OpenMeteo
{
    /// <summary>
    /// This class automatically sets the header 'Content-Type" and performs all of the api calls
    /// </summary>
    internal class HttpController
    {
        public HttpClient Client { get; }

        public HttpController()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("om-dotnet");

        }
    }
}
