using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace centralRecargasPortal.Models
{
    public class PayServicesModel
    {
        public class ToPay
        {
            [Required]
            public string referencia { get; set; }
            [Required]
            [Compare("referencia",ErrorMessage ="Las referencias no coinciden, favor de revisarlas.")]
            public string confirmacion { get; set; }
            [Required]
            public string monto { get; set; }
            public string Carrier { get; set; }
            public string errorMessage { get; set; }
        }
    }
}
