using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ThreadSafetyTest
{
    public interface IGetRequester
    {
        Task<HttpResponseMessage> RunAsync(
            string url,
            IDictionary<string, object> queryParameters,
            IDictionary<string, string> headers);
    }
}
