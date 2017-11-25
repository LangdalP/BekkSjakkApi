using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BekkSjakkApi.Models;
using BekkSjakkApi.Models.Dao;
using BekkSjakkApi.Utils;

namespace BekkSjakkApi
{
    public class BekkSjakkRepository
    {
        private readonly string _connectionString;

        public BekkSjakkRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sqldb_connection"].ConnectionString;
        }

        public async Task<List<Bekker>> HentBekkere()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                const string query = "SELECT * FROM Bruker";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var rows = await cmd.ExecuteReaderAsync();
                    var bekkere = MapRowsToBekkere(rows);
                    return bekkere;
                }
            }
        }

        public async Task<Kanskje<Bekker>> HentBekkerPåNavn(string navn)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = $"SELECT * FROM Bruker WHERE Navn = '{navn}'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var rows = await cmd.ExecuteReaderAsync();
                    var bekkere = MapRowsToBekkere(rows);
                    var bekker = bekkere.FirstOrDefault();
                    return new Kanskje<Bekker>(bekker);
                }

            }
        }

        public async Task<Kanskje<Bekker>> HentBekkerPåId(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = $"SELECT * FROM Bruker WHERE Id = {id}";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var rows = await cmd.ExecuteReaderAsync();
                    var bekkere = MapRowsToBekkere(rows);
                    var bekker = bekkere.FirstOrDefault();
                    return new Kanskje<Bekker>(bekker);
                }
            }
        }

        public async Task<bool> LeggTilBekker(Bekker bekker)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = $"INSERT INTO Bruker (Navn, Elo) VALUES ('{bekker.Navn}', {bekker.Elo})";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected == 1;
                }
            }
        }

        public async Task<bool> OppdaterBekker(Bekker bekker)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = $"UPDATE Bruker SET Navn = '{bekker.Navn}', Elo = {bekker.Elo} WHERE Id = {bekker.Id}";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected == 1;
                }
            }
        }

        public async Task<List<Parti>> HentPartier()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                const string query = "SELECT * FROM Parti";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var hentBekkereTask = HentBekkere();
                    var hentPartiTask = cmd.ExecuteReaderAsync();
                    await Task.WhenAll(hentBekkereTask, hentPartiTask);
                    var bekkere = hentBekkereTask.Result;
                    var partiRows = hentPartiTask.Result;
                    return MapRowsToParti(partiRows, bekkere);
                }
            }
        }

        public async Task<Kanskje<Parti>> HentParti(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = $"SELECT * FROM Parti WHERE Id = {id}";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var hentBekkereTask = HentBekkere();
                    var hentPartiTask = cmd.ExecuteReaderAsync();
                    await Task.WhenAll(hentBekkereTask, hentPartiTask);
                    var bekkere = hentBekkereTask.Result;
                    var partiRows = hentPartiTask.Result;
                    var partier = MapRowsToParti(partiRows, bekkere);
                    var parti = partier.FirstOrDefault();
                    return new Kanskje<Parti>(parti);
                }
            }
        }

        public async Task<bool> LeggTilParti(PartiDao parti)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = $@"INSERT INTO Parti
                    (SpillerHvit, SpillerSvart, Dato, Resultat, Pgn) VALUES
                    ({parti.SpillerHvitId}, {parti.SpillerSvartId}, '{parti.Dato:O}', {(int) parti.Resultat}, '{parti.Pgn}')";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected == 1;
                }
            }
        }

        private List<Bekker> MapRowsToBekkere(SqlDataReader reader)
        {
            var bekkere = new List<Bekker>();
            while (reader.Read())
            {
                var record = (IDataRecord) reader;
                var id = (int) record["Id"];
                var navn = (string) record["Navn"];
                var elo = (int)(short) record["Elo"];

                bekkere.Add(new Bekker(id, navn, elo));
            }
            return bekkere;
        }

        private List<Parti> MapRowsToParti(SqlDataReader reader, List<Bekker> bekkere)
        {
            var parti = new List<Parti>();
            while (reader.Read())
            {
                var record = (IDataRecord) reader;
                var id = (int) record["Id"];
                var spillerHvitId = (int)record["SpillerHvit"];
                var spillerSvartId = (int)record["SpillerSvart"];
                var dato = (DateTimeOffset) record["Dato"];
                var resultat = (PartiResultat)(byte)record["Resultat"];
                var pgn = (string) record["Pgn"];
                var spillerHvit = FinnBekkerMedId(spillerHvitId, bekkere);
                var spillerSvart = FinnBekkerMedId(spillerSvartId, bekkere);
                parti.Add(new Parti(id, spillerHvit, spillerSvart, dato, resultat, pgn));
            }
            return parti;
        }

        private Bekker FinnBekkerMedId(int id, List<Bekker> bekkere)
        {
            var bekker = bekkere.Find(b => b.Id == id);
            if (bekker == null) throw new Exception("Klarte ikke å finne bekker");
            return bekker;
        }
    }
}
