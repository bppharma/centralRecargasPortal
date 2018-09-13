using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace centralRecargasPortal.Controllers
{
    public class EcommerceController : Controller
    {
        domyJob domy = new domyJob();
        DoAPIJob APIJob = new DoAPIJob();
        Data.dataCentralRecargas data = new Data.dataCentralRecargas();
        // GET: /<controller>/
        [AutoValidateAntiforgeryToken]
        public IActionResult Vende()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    if (Request.Cookies["CC"].ToString().Equals("No"))
                    {
                        ViewBag.pendiente = true;
                        Response.Cookies.Append("IC", "1");
                    }
                    else
                    {
                        ViewBag.pendiente = false;
                    }
                }
                catch
                {
                    Response.Cookies.Append("CC", "No");
                    Response.Cookies.Append("IC", "1");
                    ViewBag.pendiente = true;
                }
                try
                {
                    ViewBag.saldoactual = Convert.ToDouble(data.GetBalanceCia(Request.Cookies["IC"].ToString())).ToString("0.00");
                }
                catch
                {
                    ViewBag.saldoactual = "0.00";
                }
                return View();
            }
            else { return RedirectToAction("Index", "Home"); }
        }
        [AutoValidateAntiforgeryToken]
        public IActionResult Recargas() { if (!User.Identity.IsAuthenticated) { return RedirectToAction("Index", "Home"); } return View(); }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DoRecarga(string Viene, string carrier, Models.RecargasViewModel.Recargar recargar)
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("Index", "Home"); }
            if (!string.IsNullOrEmpty(Viene))
            {
                switch (carrier)
                {
                    case "1":ViewBag.headPanel = "Recarga Telcel"; ViewBag.CarrierImg = 1;ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "2": ViewBag.headPanel = "Recarga Movistar"; ViewBag.CarrierImg = 2; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "3": ViewBag.headPanel = "Recarga AT&T"; ViewBag.CarrierImg = 3; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "4": ViewBag.headPanel = "Recarga Iusacell"; ViewBag.CarrierImg = 4; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "5": ViewBag.headPanel = "Recarga Virgin Mobile"; ViewBag.CarrierImg = 5; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "6": ViewBag.headPanel = "Recarga Unefon"; ViewBag.CarrierImg = 6; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "7": ViewBag.headPanel = "Recarga Nextel-ATT"; ViewBag.CarrierImg = 7; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "8": ViewBag.headPanel = "Recarga Alo"; ViewBag.CarrierImg = 8; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "9": ViewBag.headPanel = "Recarga Cierto"; ViewBag.CarrierImg = 9; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "10": ViewBag.headPanel = "Recarga Maztiempo"; ViewBag.CarrierImg = 10; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "11": ViewBag.headPanel = "Recarga Weex"; ViewBag.CarrierImg = 11; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "12": ViewBag.headPanel = "Recarga Flash Mobile"; ViewBag.CarrierImg = 12; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "13": ViewBag.headPanel = "Recarga Tuenti"; ViewBag.CarrierImg = 13; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "14": ViewBag.headPanel = "Paquete Internet Telcel"; ViewBag.CarrierImg = 14; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "15": ViewBag.headPanel = "Paquete Amigo Sin Límite"; ViewBag.CarrierImg = 15; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                    case "16": ViewBag.headPanel = "Paquete Movistar Datos"; ViewBag.CarrierImg = 16; ViewBag.CarrierQ = carrier; ViewBag.listaMonto = data.Servicedetails(carrier); break;
                }
                ViewBag.error = false;
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //call centralRecargaREService
                    Models.centralRecargaAPIModel.recargaElectronica recargaElectronica = new Models.centralRecargaAPIModel.recargaElectronica();
                    recargaElectronica.APIK = User.Identity.Name;
                    recargaElectronica.codigo = recargar.Monto;
                    recargaElectronica.referencia = recargar.Confirmacion;
                    Models.centralRecargaAPIModel.respondeRE responde = await APIJob.DoRecargaElectronica(recargaElectronica, "2", "");
                    string passing=responde.error.ToString()+"|"+responde.transaccion+"|"+responde.headerticket+"|"+responde.ticket+"|"+responde.aclaraciones;
                    TempData["responde"] = passing;
                    var response1 = responde.transaccion;
                    return RedirectToAction("ResultRecarga","Ecommerce");
                }
                else { ViewBag.error = true; recargar.ErrorMess = "Por favor revise los errores del envío"; return View(recargar); }
            }
            ViewBag.error = false;
            return View();
        }        
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> WaitResponse(string transId,string trans2)
        {
            //string mensajeresult = await domy.ResultingMagicRE(transId);
            List<string> passing = (List<string>)TempData["responde"];
            TempData["responde"] = passing;
            return RedirectToAction("ResultRecarga","Ecommerce",new { transId });
        }
public IActionResult WaitResponse(string response1)
        {
            List<string> passing = TempData["responde"] as List<string>;
            TempData["responde"] = passing;
            ViewBag.error = false;
            ViewBag.transID = response1;
            ViewBag.mensaje = "Procesando solicitud, esperando respuesta a petición # " + response1;
            return View();
        }
        [AutoValidateAntiforgeryToken]
        public IActionResult ResultRecarga()
        {
            var passing=TempData["responde"].ToString();
            Models.centralRecargaAPIModel.respondeRE responde =new Models.centralRecargaAPIModel.respondeRE();
            responde.error = passing.Split('|')[0].Equals("true") ? true : false;
            responde.transaccion = passing.Split('|')[1];
            responde.headerticket = passing.Split('|')[2];
            responde.ticket = passing.Split('|')[3];
            responde.aclaraciones = passing.Split('|')[4];
            Models.RecargasViewModel.ResultRE resultRE = new Models.RecargasViewModel.ResultRE();            
            if (!responde.error)
            {
                ViewBag.error = false;
                ViewBag.transID = responde.transaccion;
                resultRE.Companyname = responde.headerticket;
                resultRE.ticket = responde.ticket;
                resultRE.Mensaje = responde.aclaraciones;
            }
            else { resultRE.Error = responde.ticket; ViewBag.error = true; }
            return View(resultRE);
        }
        public IActionResult PayForServices()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DoServicePay(string carrier,string Viene,Models.PayServicesModel.ToPay toPay)
        {
            if (!string.IsNullOrEmpty(Viene))
            {
                switch (carrier)
                {
                    case "17": ViewBag.carrierimg = "cfe_nice.png"; ViewBag.headPanel = "Pago de CFE (Recibo de luz)"; ViewBag.CarrierQ = carrier; break;
                    case "18": ViewBag.carrierimg = "telmex-nice.png"; ViewBag.headPanel = "Pago de Telmex (Recibo de teléfono)"; ViewBag.CarrierQ = carrier; break;
                    case "19": ViewBag.carrierimg = "sky-nice.png"; ViewBag.headPanel = "Pago de SKY/VETV (Recibo de TV satelital)"; ViewBag.CarrierQ = carrier; break;
                    case "20": ViewBag.carrierimg = "dish-nice.png"; ViewBag.headPanel = "Pago de DISH (Recibo de TV Satelital)"; ViewBag.CarrierQ = carrier; break;
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //validate models2
                    bool errors = false;
                    switch (toPay.Carrier)
                    {
                        case "17": if (!toPay.confirmacion.Length.Equals(30)) { errors = true; toPay.errorMessage = "La longitud de la referencia debe ser de 30 caracteres/dígitos"; }break;
                        case "18": if (!toPay.confirmacion.Length.Equals(10)) { errors = true; toPay.errorMessage = "La longitud de la referencia debe ser de 10 caracteres/dígitos"; } break;
                        case "19": if (!toPay.confirmacion.Length.Equals(12)) { errors = true; toPay.errorMessage = "La longitud de la referencia debe ser de 12 caracteres/dígitos"; } break;
                        case "20": if (!toPay.confirmacion.Length.Equals(14)) { errors = true; toPay.errorMessage = "La longitud de la referencia debe ser de 14 caracteres/dígitos"; } break;
                    }
                    //save request and send to wait
                    //var response1 = await domy.DoingMagicPS(toPay, User.Identity.Name);
                    //if (response1.StartsWith("Error"))
                    //{
                    //    ViewBag.error = true;
                    //    ViewBag.mensaje = response1;
                    //    errors = true;
                    //    toPay.errorMessage = response1;
                    //}
                    //else
                    //{
                    //    ViewBag.error = false;
                    //    ViewBag.transID = response1;
                    //    ViewBag.mensaje = "Procesando solicitud, esperando respuesta a petición # " + response1;
                    //}
                    Models.centralRecargaAPIModel.pagoServicios pagoServicios = new Models.centralRecargaAPIModel.pagoServicios();
                    pagoServicios.APIK = User.Identity.Name;
                    pagoServicios.codigo = toPay.Carrier;
                    pagoServicios.referencia = toPay.referencia;
                    pagoServicios.monto = toPay.monto;
                    pagoServicios.referencia2 = "";
                    Models.centralRecargaAPIModel.respondeRE responde = await APIJob.DoPagoServicios(pagoServicios, "2", "");
                    string passing = responde.error.ToString() + "|" + responde.transaccion + "|" + responde.headerticket + "|" + responde.ticket + "|" + responde.aclaraciones;
                    TempData["responde"] = passing;
                    if (!errors) { return RedirectToAction("ResultRecarga", "Ecommerce"); }
                    else { return View(toPay); }
                }
                else { return View(toPay); }
            }
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SendNotification(string EmailSend,string PhoneNumber,string transID)
        {
            int tipo = 0;string valor = "";
            if (string.IsNullOrEmpty(EmailSend)) { tipo = 1; valor = PhoneNumber; } else { tipo = 2; valor = EmailSend; }
            //data.SaveDemonNotifications(tipo, valor, transID);
            return RedirectToAction("Vende", "Ecommerce");
        }
        [AutoValidateAntiforgeryToken]
        public IActionResult Comprar(Models.centralRecargaAPIModel.comprarView comprar)
        {
            Dictionary<string, string> bancos = new Dictionary<string, string>
            {
                {"1","Scotiabank - Cta:000000000 - Clabe:000000000000000 - Luis Briseño" },
                {"2","Banorte - Cta:000000000 - Clabe:000000000000000 - Luis Briseño" },
                {"3","Santander- Cta:000000000 - Clabe:000000000000000 - BP Pharma SAS" },
                {"4","Sabadell- Clabe:000000000000000 - BP Pharma SAS" },
                {"5","Banamex - Cta:000000000 - Clabe:000000000000000 - BP Pharma SAS" }
            };
            ViewBag.Bancos = bancos;
            Dictionary<string, string> formaPago = new Dictionary<string, string>
            {
                {"1","Efectivo" },
                {"2","Transferencia" },
                {"3","Cheque" },
                {"4","Cajero ATM" },
                {"5","Telecomm" },
                {"6","Oxxo" }
            };
            ViewBag.FormaPago = formaPago;
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult RegistraCompra(Models.centralRecargaAPIModel.comprarView comprar)
        {
            if (ModelState.IsValid)
            {
                comprar.APIK = User.Identity.Name;
                data.SaveDepositClient(comprar);
                return View();
            }
            else
            {
                return RedirectToAction("Comprar", "Ecommerce", comprar);
            }
        }
        [AutoValidateAntiforgeryToken]
        public IActionResult Reportes()
        {
            Models.ReportesModel.ReportesViewModel reportesView = new Models.ReportesModel.ReportesViewModel();
            reportesView.usuario = User.Identity.Name;
            List<Models.ReportesModel.ReporteVenta> reporteVentas = data.GetReporteVenta(reportesView);
            if (reporteVentas.Count > 0)
            {
                ViewBag.datos = true;
                ViewBag.compras = true;
                ViewBag.lista = reporteVentas;
            }
            else
            {
                ViewBag.datos = false;
                ViewBag.compras = false;
                ViewBag.lista = reporteVentas;
            }
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Reportes(Models.ReportesModel.ReportesViewModel reportesView)
        {
            if (reportesView != null)
            {
                List<Models.ReportesModel.ReporteVenta> reporteVentas = data.GetReporteVenta(reportesView);
                if (reporteVentas.Count > 0)
                {
                    ViewBag.datos = true;
                    ViewBag.compras = true;
                    ViewBag.lista = reporteVentas;
                }
                else
                {
                    ViewBag.datos = false;
                    ViewBag.compras = false;
                    ViewBag.lista = reporteVentas;
                }
            }
            else
            {
                reportesView = new Models.ReportesModel.ReportesViewModel();
                reportesView.usuario = User.Identity.Name;
                List<Models.ReportesModel.ReporteVenta> reporteVentas = data.GetReporteVenta(reportesView);
                if (reporteVentas.Count > 0)
                {
                    ViewBag.datos = true;
                    ViewBag.compras = true;
                    ViewBag.lista = reporteVentas;
                }
                else
                {
                    ViewBag.datos = false;
                    ViewBag.compras = false;
                    ViewBag.lista = reporteVentas;
                }
            }            
            return View();
        }
    }
}
