using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BekkSjakkApi.Utils
{
    public class Kanskje<T> where T : class
    {
        private readonly T _instans;

        public T Verdi => _instans;
        public bool HarVerdi => _instans != null;
        public bool ManglerVerdi => _instans == null;

        public Kanskje() : this(null) { }

        public Kanskje(T instans)
        {
            _instans = instans;
        }
    }
}
