using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace centralRecargasPortal.Controllers
{
    public class DoAPIJob
    {
        private getServicesTaeCel GetServicesTaeCel = new getServicesTaeCel();
        private getServicesRMovil getServicesR = new getServicesRMovil();
        private Data.dataCentralRecargas data = new Data.dataCentralRecargas();
        public async Task<Models.centralRecargaAPIModel.respondeRE> DoRecargaElectronica(Models.centralRecargaAPIModel.recargaElectronica recargaElectronica, string via, string IP)
        {
            //login when its ready
            //after that, do check company balance vs service code vs suppliers balances
            Models.centralRecargaAPIModel.respondeRE responde = new Models.centralRecargaAPIModel.respondeRE();
            double companyBalance = Convert.ToDouble(data.GetBalanceAvailable(recargaElectronica.APIK));
            double serviceAmount = Convert.ToDouble(recargaElectronica.codigo.Split('-')[1]);
            double debit = serviceAmount - ((serviceAmount / 10) * data.GetFeeRE(recargaElectronica.APIK));
            if (companyBalance >= debit)
            {
                string IDSupplier = data.GetBestBalanceAndSupplier(debit.ToString(), "saldorecargas");
                string localTransactionID = data.GetLocalTransactionID(recargaElectronica.codigo, recargaElectronica.APIK, recargaElectronica.referencia, via, IP, IDSupplier, debit.ToString(), "0",recargaElectronica.codigo.Split('-')[1]);
                switch (IDSupplier)
                {
                    case "1":
                        Models.TaecelModel.RequestTXNModel requestTXN = GetServicesTaeCel.SendServiceRE(data.GetSupplierCode(recargaElectronica.codigo, IDSupplier), "0", recargaElectronica.referencia);                        
                        if (requestTXN.success)
                        {
                            if (requestTXN.error.Equals(0))
                            {
                                data.updateIdTrans(localTransactionID, requestTXN.data.transID);
                                System.Threading.Thread.Sleep(2000);
                                Models.TaecelModel.StatusTXNRE StatusTXNRE = GetServicesTaeCel.GetStatusTXNRE(requestTXN.data.transID);
                                if (StatusTXNRE.error.Equals(0) || StatusTXNRE.message.Equals("Recarga Exitosa"))
                                {
                                    data.updateEndTrans(requestTXN.data.transID, StatusTXNRE.data.Folio, "3");
                                    string headerTicket = data.GetNotesticket(recargaElectronica.codigo, recargaElectronica.APIK);
                                    responde.ticket = "Recarga " + StatusTXNRE.data.Carrier + " exitosa. Folio:" + StatusTXNRE.data.Folio + " # Tel. " + StatusTXNRE.data.Telefono + " Monto:" + StatusTXNRE.data.Monto + " Hora:" + StatusTXNRE.data.Fecha + " ";
                                    responde.error = false;
                                    responde.fechaOperacion = StatusTXNRE.data.Fecha;
                                    responde.abono = "0.00";
                                    responde.cargo = debit.ToString();
                                    responde.transaccion = localTransactionID;
                                    responde.monto = serviceAmount.ToString();
                                    responde.folio = StatusTXNRE.data.Folio;
                                    responde.aclaraciones = headerTicket.Split('|')[2];
                                    responde.carrier = headerTicket.Split('|')[1];
                                    responde.headerticket = headerTicket.Split('|')[0];
                                    data.UpdateBalanceCia(debit.ToString(), recargaElectronica.APIK,IDSupplier,serviceAmount.ToString());
                                }
                                else
                                {
                                    data.updateEndTrans(requestTXN.data.transID, "", "4");
                                    responde.ticket = "Error, transacción # " + localTransactionID + " " + StatusTXNRE.message;
                                    responde.error = true;
                                }
                            }
                        }
                        else
                        {
                            data.updateIdTrans(localTransactionID, localTransactionID);
                            data.updateEndTrans(localTransactionID, "", "4");
                            responde.ticket = "Error, transacción # " + localTransactionID + " " + " Error en la respuesta del servicio, favor de reportar a soporte.";
                            responde.error = true;
                        }
                        break;
                    case "2":
                        Models.prepagotaemodel.respuesta respuesta = await getServicesR.DoREPTAE(data.GetSupplierCode(recargaElectronica.codigo, IDSupplier), recargaElectronica.referencia, recargaElectronica.codigo.Split('-')[1]);
                        if (respuesta.codigo.Equals("0"))
                        {
                            data.updateIdTrans(localTransactionID, respuesta.autorizacion);
                            data.updateEndTrans(respuesta.autorizacion, respuesta.autorizacion, "3");
                            string headerTicket = data.GetNotesticket(recargaElectronica.codigo, recargaElectronica.APIK);
                            responde.ticket = respuesta.mensaje;
                            responde.error = false;
                            responde.fechaOperacion = DateTime.Now.ToString();
                            responde.abono = "0.00";
                            responde.cargo = debit.ToString();
                            responde.transaccion = localTransactionID;
                            responde.monto = serviceAmount.ToString();
                            responde.folio = respuesta.autorizacion;
                            responde.aclaraciones = headerTicket.Split('|')[2];
                            responde.carrier = headerTicket.Split('|')[1];
                            responde.headerticket = headerTicket.Split('|')[0];
                            data.UpdateBalanceCia(debit.ToString(), recargaElectronica.APIK, IDSupplier, serviceAmount.ToString());
                        }
                        else
                        {
                            data.updateIdTrans(localTransactionID, localTransactionID);
                            data.updateEndTrans(localTransactionID, "", "4");
                            responde.ticket = "Error, transacción # " + localTransactionID + " " + respuesta.mensaje;
                            responde.error = true;
                        }
                        break;
                    case "3":
                        break;
                    default:
                        responde.error = true;
                        responde.ticket = "Ocurrió un problema en la transacción, le solicitamos reportar el código #318181002 al teléfono de contacto.";
                        return responde;
                }
            }
            else
            {
                responde.error = true;
                responde.ticket = "Saldo insuficiente, sólo cuenta con $ " + companyBalance.ToString();
            }
            return responde;
        }
        public async Task<Models.centralRecargaAPIModel.respondeRE> DoPagoServicios(Models.centralRecargaAPIModel.pagoServicios pagoServicios,string via,string IP)
        {
            //login when its ready
            //after that, do check company balance vs service code vs suppliers balances
            Models.centralRecargaAPIModel.respondeRE responde = new Models.centralRecargaAPIModel.respondeRE();
            double companyBalance = Convert.ToDouble(data.GetBalanceAvailable(pagoServicios.APIK));
            double serviceAmount = Convert.ToDouble(pagoServicios.monto);
            double debit = serviceAmount + 5;
            if (companyBalance >= debit)
            {
                string IDSupplier = data.GetBestBalanceAndSupplier(debit.ToString(), "saldoservicios");
                string localTransactionID = data.GetLocalTransactionIDPS(pagoServicios.codigo, pagoServicios.APIK, (pagoServicios.referencia+pagoServicios.referencia2), via, IP, IDSupplier, debit.ToString(), "0",pagoServicios.monto);
                switch (IDSupplier)
                {
                    case "1":
                        Models.TaecelModel.RequestTXNModel requestTXN = GetServicesTaeCel.SendServicePS(data.GetSupplierCode(pagoServicios.codigo, IDSupplier), pagoServicios.monto, pagoServicios.referencia);                        
                        if (requestTXN.success)
                        {
                            if (requestTXN.error.Equals(0))
                            {
                                Models.TaecelModel.StatusTXNRE StatusTXNRE = GetServicesTaeCel.GetStatusTXNRE(requestTXN.data.transID);
                                if (StatusTXNRE.error.Equals(0) || StatusTXNRE.message.Equals("Recarga Exitosa"))
                                {
                                    data.updateEndTrans(requestTXN.data.transID, StatusTXNRE.data.Folio, "3");
                                    string headerTicket = data.GetNotesticket(pagoServicios.codigo, pagoServicios.APIK);
                                    responde.ticket = "Pago de servicio " + StatusTXNRE.data.Carrier + " exitosa. Folio:" + StatusTXNRE.data.Folio + " # Referencia. " + StatusTXNRE.data.Telefono + " Monto:" + StatusTXNRE.data.Monto + " Hora:" + StatusTXNRE.data.Fecha + " ";
                                    responde.error = false;
                                    responde.fechaOperacion = StatusTXNRE.data.Fecha;
                                    responde.abono = "0.00";
                                    responde.cargo = debit.ToString();
                                    responde.transaccion = localTransactionID;
                                    responde.monto = serviceAmount.ToString();
                                    responde.folio = StatusTXNRE.data.Folio;
                                    responde.aclaraciones = headerTicket.Split('|')[2];
                                    responde.carrier = headerTicket.Split('|')[1];
                                    responde.headerticket = headerTicket.Split('|')[0];
                                    data.UpdateBalanceCia(debit.ToString(), pagoServicios.APIK, IDSupplier, (serviceAmount+5).ToString());
                                }
                                else
                                {
                                    data.updateEndTrans(requestTXN.data.transID, "", "4");
                                    responde.ticket = "Error, transacción # " + localTransactionID + " " + StatusTXNRE.message;
                                    responde.error = true;
                                }
                            }
                        }
                        break;
                    case "2":
                        Models.prepagotaemodel.respuesta respuesta = await getServicesR.DoPSPTAE(data.GetCodeServiceSupplierPS(pagoServicios.codigo, IDSupplier), pagoServicios.referencia, pagoServicios.referencia2, pagoServicios.monto);
                        if (respuesta.codigo.Equals("0"))
                        {
                            data.updateIdTrans(localTransactionID, respuesta.autorizacion);
                            data.updateEndTrans(respuesta.autorizacion, respuesta.autorizacion, "3");
                            string headerTicket = data.GetNotesticket(pagoServicios.codigo, pagoServicios.APIK);
                            responde.ticket = respuesta.mensaje;
                            responde.error = false;
                            responde.fechaOperacion = DateTime.Now.ToString();
                            responde.abono = "0.00";
                            responde.cargo = debit.ToString();
                            responde.transaccion = localTransactionID;
                            responde.monto = serviceAmount.ToString();
                            responde.folio = respuesta.autorizacion;
                            responde.aclaraciones = headerTicket.Split('|')[2];
                            responde.carrier = headerTicket.Split('|')[1];
                            responde.headerticket = headerTicket.Split('|')[0];
                            data.UpdateBalanceCia(debit.ToString(), pagoServicios.APIK, IDSupplier, (serviceAmount+4).ToString());
                        }
                        else
                        {
                            data.updateIdTrans(localTransactionID, localTransactionID);
                            data.updateEndTrans(localTransactionID, "", "4");
                            responde.ticket = "Error, transacción # " + localTransactionID + " " + respuesta.mensaje;
                            responde.error = true;
                        }
                        break;
                }
            }
            else
            {
                responde.error = true;
                responde.ticket = "Saldo insuficiente, sólo cuenta con $ " + companyBalance.ToString();
            }
            return responde;
        }
    }
}
