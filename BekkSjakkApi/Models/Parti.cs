using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BekkSjakkApi.Models
{
    [DataContract]
    public class Parti
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Bekker SpillerHvit { get; set; }
        [DataMember]
        public Bekker SpillerSvart { get; set; }
        [DataMember]
        public DateTimeOffset Dato { get; set; }
        [DataMember]
        public PartiResultat Resultat { get; set; }
        [DataMember]
        public string Pgn { get; set; }

        public Parti() { }

        public Parti(Bekker hvit, Bekker svart, DateTimeOffset dato, PartiResultat resultat, string pgn)
        {
            SpillerHvit = hvit;
            SpillerSvart = svart;
            Dato = dato;
            Resultat = resultat;
            Pgn = pgn;
        }

        public Parti(int id, Bekker hvit, Bekker svart, DateTimeOffset dato, PartiResultat resultat, string pgn)
            : this(hvit, svart, dato, resultat, pgn)
        {
            Id = id;
        }
    }
}
