using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace centralRecargasPortal.Models
{
    public class ReportesModel
    {
        public class ReportesViewModel
        {
            public string TipoReporte { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public string Fechainicio { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public string FechaFin { get; set; }
            public string usuario { get; set; }
            public string CanalVenta { get; set; }
        }
        public class ReporteVenta
        {
            public string Ticket { get; set; }
            public string Producto { get; set; }
            public string Monto { get; set; }
            public string Referencia { get; set; }
            public string Autorización { get; set; }
            public string Cajero { get; set; }
            public string Canal { get; set; }
            public string Estatus { get; set; }
            public string Cargo { get; set; }
            public string Abono { get; set; }
        }
    }
}
