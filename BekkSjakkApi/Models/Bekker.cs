using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BekkSjakkApi.Models
{
    [DataContract]
    public class Bekker
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Navn { get; set; }
        [DataMember]
        public int Elo { get; set; } = 1500;

        public Bekker()
        {
        }

        public Bekker(string navn)
        {
            Navn = navn;
        }

        public Bekker(int id, string navn, int elo)
        {
            Id = id;
            Navn = navn;
            Elo = elo;
        }
    }
}
