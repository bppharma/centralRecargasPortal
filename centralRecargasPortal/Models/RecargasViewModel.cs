using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace centralRecargasPortal.Models
{
    public class RecargasViewModel
    {
        public class Recargar
        {
            public string Monto { get; set; }
            [Required]
            [StringLength(10, ErrorMessage = "El número debe ser de 10 dígitos", MinimumLength = 10)]
            public string Numero { get; set; }
            [Required]
            [StringLength(10, ErrorMessage = "El número debe ser de 10 dígitos", MinimumLength = 10)]
            [Compare("Numero", ErrorMessage = "El número no coincide con la confirmación.")]
            public string Confirmacion { get; set; }
            public string Carrier { get; set; }
            public string ErrorMess { get; set; }
        }
        public class ResultRE
        {
            public string Companyname { get; set; }
            public string ticket { get; set; }
            public string Mensaje { get; set; }
            public string Error { get; set; }
        }
        public class servicedetail
        {
            public string idserviciodetalle { get; set; }
            public string idservicio { get; set; }
            public string etiqueta { get; set; }
            public string codigo { get; set; }
            public string vigencia { get; set; }
            public string descripcion { get; set; }
        }
    }
}
