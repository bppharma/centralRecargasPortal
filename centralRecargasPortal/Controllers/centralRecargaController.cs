using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace centralRecargasPortal.Controllers
{
    [Route("api/[controller]")]
    public class centralRecargaController : Controller
    {
        DoAPIJob aPIJob = new DoAPIJob();
        [HttpPost]
        public async Task<JsonResult> recargaElectronica(Models.centralRecargaAPIModel.recargaElectronica recarga)
        {
            Models.centralRecargaAPIModel.respondeRE responde = await aPIJob.DoRecargaElectronica(recarga, "1", "");
            return Json(responde);
        }
        [HttpPost]
        public async Task<JsonResult> pagoServicios(Models.centralRecargaAPIModel.pagoServicios pagoServicios)
        {
            Models.centralRecargaAPIModel.respondeRE responde = await aPIJob.DoPagoServicios(pagoServicios, "1", "");
            return Json(responde);
        }
    }
}
