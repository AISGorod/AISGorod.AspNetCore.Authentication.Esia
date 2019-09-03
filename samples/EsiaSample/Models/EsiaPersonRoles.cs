using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsiaSample.Models
{
    public class EsiaPersonRoles
    {
        public int oid { get; set; }

        public string fullName { get; set; }

        public string shortName { get; set; }

        public string ogrn { get; set; }

        // Тут должно быть намного больше свойств. Но текущего набора достаточно.
    }
}
