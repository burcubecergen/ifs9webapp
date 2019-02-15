using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ifs9webapp
{
    public class MaterialRequest
    {
        public string PartNo { get; set; }
        public string Description { get; set; }
        public int QtyDue { get; set; }
        public string UnitMeas { get; set; }
        public string OrderNo { get; set; }
        public string LineNo { get; set; }
        public string ReleaseNo { get; set; }
        public int LineItemNo { get; set; }
        public string OrderClassDb { get; set; }
        public string Rule { get; set; }
        public int Step { get; set; }
        public string ActivityAdi { get; set; }
        public string Proje { get; set; }
        public string NoteText { get; set; }
        public string ObjId { get; set; }
        public string ObjVersion { get; set; }
        public string AllSuccess { get; set; }

    }
}
