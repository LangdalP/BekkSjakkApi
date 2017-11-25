using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using BekkSjakkApi.Models;
using BekkSjakkApi.Models.Dao;
using BekkSjakkApi.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Build.Framework;

namespace BekkSjakkApi
{
    public static class PartiEndpoint
    {
        [FunctionName("Parti")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            if (req.Method == HttpMethod.Post)
            {
                return await PartiPost(req, log);
            }
            return await PartiGet(req, log);
        }

        private static async Task<HttpResponseMessage> PartiGet(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[GET Parti] ble kalt");
            var repo = new BekkSjakkRepository();

            string partiIdString = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => q.Key == "id").Value;

            if (string.IsNullOrEmpty(partiIdString)) return req.CreateResponse(HttpStatusCode.BadRequest);

            int partiId = int.Parse(partiIdString);
            var parti = await repo.HentParti(partiId);
            if (parti.HarVerdi) return req.CreateResponse(HttpStatusCode.OK, parti.Verdi, "application/json");
            else return req.CreateResponse(HttpStatusCode.NotFound);
        }

        private static async Task<HttpResponseMessage> PartiPost(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("[POST Parti] ble kalt");
            var repo = new BekkSjakkRepository();

            dynamic data = await req.Content.ReadAsAsync<object>();
            if (data == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var hvitSpillerId = (int) data.hvitId;
            var svartSpillerId = (int) data.svartId;
            var dato = (DateTimeOffset) data.dato;
            var resultat = (PartiResultat) Enum.Parse(typeof(PartiResultat), (string) data.resultat);
            var pgn = (string) data.pgn;
            var partiDao = new PartiDao(hvitSpillerId, svartSpillerId, dato, resultat, pgn);
            var suksess = await repo.LeggTilParti(partiDao);

            await OppdaterEloRatinger(repo, partiDao);

            var statusCode = suksess ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            return req.CreateResponse(statusCode);
        }

        private static async Task OppdaterEloRatinger(BekkSjakkRepository repo, PartiDao parti)
        {
            var hvitTask = repo.HentBekkerPÂId(parti.SpillerHvitId);
            var svartTask = repo.HentBekkerPÂId(parti.SpillerSvartId);
            await Task.WhenAll(hvitTask, svartTask);
            var hvitBekker = hvitTask.Result;
            var svartBekker = svartTask.Result;
            if (!hvitBekker.HarVerdi || !svartBekker.HarVerdi) throw new Exception("Klarte ikkje Â oppdatere ratingar");
            var nyeElo = Elo.FinnNyeEloRatinger(hvitBekker.Verdi, svartBekker.Verdi, parti.Resultat);
            hvitBekker.Verdi.Elo = nyeElo.Item1;
            svartBekker.Verdi.Elo = nyeElo.Item2;
            var oppdaterTask1 = repo.OppdaterBekker(hvitBekker.Verdi);
            var oppdaterTask2 = repo.OppdaterBekker(svartBekker.Verdi);
            await Task.WhenAll(oppdaterTask1, oppdaterTask2);
        }
    }
}
