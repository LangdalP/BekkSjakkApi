using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BekkSjakkApi.Models;
using BekkSjakkApi.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BekkSjakkApi
{
    public static class BekkerEndpoint
    {
        [FunctionName("Bekker")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            if (req.Method == HttpMethod.Post)
            {
                return await BekkerPost(req, log);
            }
            return await BekkerGet(req, log);
        }

        private static async Task<HttpResponseMessage> BekkerGet(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[GET Bekker] ble kalt");
            var repo = new BekkSjakkRepository();
            var parameters = req.GetQueryNameValuePairs().ToList();

            string navn = parameters.FirstOrDefault(q => q.Key == "navn").Value;
            if (!string.IsNullOrEmpty(navn))
            {
                return HentBekkerRespons(req, await repo.HentBekkerPÂNavn(navn));
            }

            string idString = parameters.FirstOrDefault(q => q.Key == "id").Value;
            int id;
            if (!string.IsNullOrEmpty(idString) && int.TryParse(idString, out id))
            {
                return HentBekkerRespons(req, await repo.HentBekkerPÂId(id));
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        private static async Task<HttpResponseMessage> BekkerPost(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[POST Bekker] ble kalt");
            var repo = new BekkSjakkRepository();

            dynamic data = await req.Content.ReadAsAsync<object>();
            var bekkerNavn = (string) data?.navn;
            if (bekkerNavn != null)
            {
                var suksess = await repo.LeggTilBekker(new Bekker(bekkerNavn));
                if (suksess) return req.CreateResponse(HttpStatusCode.Created);
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        private static HttpResponseMessage HentBekkerRespons(HttpRequestMessage req, Kanskje<Bekker> bekker)
        {
            return bekker.HarVerdi
                ? req.CreateResponse(HttpStatusCode.OK, bekker.Verdi, "application/json")
                : req.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
