using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace centralRecargasPortal.Models
{
    public class TaecelModel
    {
        public class RequestTXNModel
        {
            public bool success { get; set; }
            public int error { get; set; }
            public string message { get; set; }
            public RequestTXNREDataModel data { get; set; }
            public string extra { get; set; }
        }
        public class RequestTXNREDataModel
        {
            public string transID { get; set; }
        }
        public class StatusTXNRE
        {
            public bool success { get; set; }
            public int error { get; set; }
            public string message { get; set; }
            public StatusTXNREDataModel data { get; set; }
            public string extra { get; set; }
        }
        public class StatusTXNREDataModel
        {
            public string TransID { get; set; }
            public string Fecha { get; set; }
            public string Carrier { get; set; }
            public string Telefono { get; set; }
            public string Folio { get; set; }
            public string Status { get; set; }
            public string Monto { get; set; }
            public string Cargo { get; set; }
            public string Abono { get; set; }
            public string Via { get; set; }
            public string Región { get; set; }
            public string Timeout { get; set; }
            public string IP { get; set; }
            public string Bolsa { get; set; }
            public string Comision { get; set; }
            public string Nota { get; set; }
        }
    }
}
