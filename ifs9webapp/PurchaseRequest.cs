using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ifs9webapp
{
    public class PurchaseRequest
    {
        public string PartNo { get; set; }
        public string PartDescription { get; set; }
        public int Qty { get; set; }
        public string UnitMeas { get; set; }
        public string DestinationId { get; set; }

    }
}
