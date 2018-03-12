using System.Net.Http;
using System.Net.Http.Headers;

namespace NeroLib
{
    public class HTTPHelpers
    {
        public static HttpClient NewClient() {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}