using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace BekkSjakkApi
{
    public static class BekkereEndpoint
    {
        [FunctionName("Bekkere")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[GET Bekkere] ble kalt");
            var repo = new BekkSjakkRepository();
            List<Models.Bekker> bekkere = await repo.HentBekkere();

            return req.CreateResponse(HttpStatusCode.OK, bekkere, "application/json");
        }
    }
}
