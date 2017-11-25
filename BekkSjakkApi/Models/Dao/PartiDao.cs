using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BekkSjakkApi.Models.Dao
{
    public class PartiDao
    {
        public int Id { get; set; }
        public int SpillerHvitId { get; set; }
        public int SpillerSvartId { get; set; }
        public DateTimeOffset Dato { get; set; }
        public PartiResultat Resultat { get; set; }
        public string Pgn { get; set; }

        public PartiDao(int hvit, int svart, DateTimeOffset dato, PartiResultat resultat, string pgn)
        {
            SpillerHvitId = hvit;
            SpillerSvartId = svart;
            Dato = dato;
            Resultat = resultat;
            Pgn = pgn;
        }
    }
}
