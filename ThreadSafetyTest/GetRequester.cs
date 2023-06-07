using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ThreadSafetyTest
{
    public class GetRequester : IGetRequester
    {
        #region Properties
        private static readonly object ClientLock = new object();
        private static HttpClient _client;

        public HttpClient Client
        {
            get
            {
                lock (ClientLock)
                    return _client ?? (_client = new HttpClient());
            }
            set => _client = value;
        }

        #endregion

        #region Constructors

        public GetRequester()
        {            
        }

        #endregion

        #region Public Methods

        public async Task<HttpResponseMessage> RunAsync(
            string url,
            IDictionary<string, object> queryParameters,
            IDictionary<string, string> headers)
        {
            if (queryParameters != null && queryParameters.Any())
            {
                var qParams = string.Join("&", queryParameters.Select(x => $"{x.Key}={x.Value}"));
                url = $"{url}?{qParams}";
            }

            using (HttpRequestMessage message = new(HttpMethod.Get, url))
            {
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

                return await Client.SendAsync(message);
            }
        }


        //public async Task<HttpResponseMessage> RunAsync(
        //    string url,
        //    IDictionary<string, object> queryParameters,
        //    IDictionary<string, string> headers)
        //{
        //    if (queryParameters != null && queryParameters.Any())
        //    {
        //        var qParams = string.Join("&", queryParameters.Select(x => $"{x.Key}={x.Value}"));
        //        url = $"{url}?{qParams}";
        //    }

        //    Client.DefaultRequestHeaders.Clear();
        //    foreach (var header in headers)
        //    {
        //        Client.DefaultRequestHeaders.Add(header.Key, header.Value);
        //    }

        //    return await Client.GetAsync(url);

        //}

        #endregion
    }
}
