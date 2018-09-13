using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace centralRecargasPortal.Models
{
    public class centralRecargaAPIModel
    {
        public class pagoServicios
        {
            public string APIK { get; set; }
            public string APIS { get; set; }
            public string codigo { get; set; }
            public string referencia { get; set; }
            public string referencia2 { get; set; }
            public string monto { get; set; }
        }
        public class recargaElectronica
        {
            public string APIK { get; set; }
            public string APIS { get; set; }
            public string codigo { get; set; }
            public string referencia { get; set; }            
        }
        public class respondeRE
        {
            public string transaccion { get; set; }
            public string monto { get; set; }
            public string folio { get; set; }
            public string carrier { get; set; }
            public string headerticket { get; set; }
            public string ticket { get; set; }
            public string aclaraciones { get; set; }
            public string cargo { get; set; }
            public string abono { get; set; }
            public bool error { get; set; }
            public string fechaOperacion { get; set; }
        }
        public class comprarView
        {
            public string APIK { get; set; }
            public string APIS { get; set; }
            public string banco { get; set; }
            public string formapago { get; set; }
            [Required]
            [DataType(DataType.Date,ErrorMessage ="No ingresó un formato correcto")]
            public string fecha { get; set; }
            [Required]
            public string hora { get; set; }
            [Required]
            [DataType(DataType.Currency)]
            public string monto { get; set; }
            [Required]
            public string folio { get; set; }
        }
        public class comprar
        {
            public string APIK { get; set; }
            public string APIS { get; set; }
            public string banco { get; set; }
            public string formapago { get; set; }
            public string fecha { get; set; }
            public string hora { get; set; }
            public string monto { get; set; }
            public string folio { get; set; }
        }
    }
}
