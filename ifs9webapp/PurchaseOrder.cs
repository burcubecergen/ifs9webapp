using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ifs9webapp
{
    public class PurchaseOrder
    {
        public string PartNo { get; set; }
        public string Description { get; set; }
        public int BuyQtyDue { get; set; }
        public string BuyUnitMeas { get; set; }
        public int BuyUnitPrice { get; set; }
        public int TotalPrice { get; set; }
        public string NoteText { get; set; }
        public string OrderNo { get; set; }
        public int SequenceNo{ get; set; }
    }
}
