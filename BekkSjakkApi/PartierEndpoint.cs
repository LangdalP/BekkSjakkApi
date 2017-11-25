using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BekkSjakkApi
{
    public static class PartierEndpoint
    {
        [FunctionName("Partier")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            return await PartiGet(req, log);
        }


        private static async Task<HttpResponseMessage> PartiGet(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[GET Partier] ble kalt");
            var repo = new BekkSjakkRepository();

            var parti = await repo.HentPartier();
            return req.CreateResponse(HttpStatusCode.OK, parti, "application/json");
        }
    }
}
