using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BekkSjakkApi
{
    public static class OmEndpoint
    {
        [FunctionName("Om")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[GET Om] ble kalt");
            return LagHtmlRespons(Resources.Om, HttpStatusCode.OK);
        }

        private static HttpResponseMessage LagHtmlRespons(string html, HttpStatusCode statusKode)
        {
            var respons = new HttpResponseMessage(statusKode);
            respons.Content = new StringContent(html);
            respons.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return respons;
        }
    }
}
