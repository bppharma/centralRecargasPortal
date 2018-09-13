using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace centralRecargasPortal.Controllers
{
    public class domyJob
    {
        private Data.dataCentralRecargas data = new Data.dataCentralRecargas();
        private getServicesTaeCel GetServicesTaeCel = new getServicesTaeCel();
        private getServicesRMovil GetServicesR = new getServicesRMovil();
        public async Task<string> DoingMagicPS(Models.PayServicesModel.ToPay toPay,string usuario)
        {
            //checking for better balance
            string idsupplier = data.returnBetterBalance(toPay.monto, toPay.Carrier);
            string transactionId = "";
            string localTransactionId = data.returnIdTrans(usuario,data.GetCodeServiceDetail(toPay.Carrier,idsupplier), toPay.monto, toPay.confirmacion);
            switch (idsupplier)
            {
                case "1":
                    transactionId = await GeTransactionTaecelPS(data.GetCodeServiceSupplierPS(toPay.Carrier,idsupplier), toPay.monto, toPay.referencia);
                    if (!transactionId.Contains("|"))
                    {
                        data.updateIdTrans(localTransactionId, transactionId);
                        transactionId = localTransactionId;
                    }
                    else
                    {
                        transactionId = "Error, " + transactionId.Split('|')[1];
                    }
                    break;
            }
            return transactionId;
        }
        public async Task<string> DoingMagicRE(Models.RecargasViewModel.Recargar recargar,string usuario)
        {
            //check balance
            double balancecia = data.GetBalanceAvailable(usuario);
            double montoCargar = data.howmuchserviceRE(recargar.Monto);
            double montoRecargar = montoCargar * 1.06;
            string transactionId = "";
            if (balancecia >= montoCargar)
            {
                //checking for better balance
                string idsupplier = data.returnBetterBalance(recargar.Monto, recargar.Carrier);
                string localTransactionId = data.returnIdTrans(usuario, recargar.Monto, "0", recargar.Confirmacion);
                switch (idsupplier)
                {
                    case "1":
                        transactionId = await GetTransactionTaecel(data.GetCodeServiceSuplier(recargar.Monto, idsupplier), recargar.Confirmacion);
                        if (!transactionId.Contains("|"))
                        {
                            data.updateIdTrans(localTransactionId, transactionId);
                            transactionId = localTransactionId;
                        }
                        else
                        {
                            transactionId = "Error, " + transactionId.Split('|')[1];
                        }
                        break;
                    case "2":
                        transactionId = await GetTransactioPrepaTaeRE(data.GetCodeServiceSuplier(recargar.Monto, idsupplier), montoRecargar.ToString(), recargar.Confirmacion);
                        if (!transactionId.StartsWith("Error"))
                        {
                            transactionId = localTransactionId;
                        }  
                        break;
                }
            }
            else
            {
                transactionId = "Error, saldo insuficiente. Cuenta con $ " + balancecia.ToString();
            }
            return transactionId;
        }
        public async Task<string> ResultingMagicRE(string transactionid)
        {
            string transID = "";
            string tipoServicio = data.GetTipoServ(transactionid);
            string mensaje = "";
            switch (tipoServicio)
            {
                case "1":  transID=data.GetIdTrans(transactionid); mensaje = await GetResultTaecel(transID);break;
                case "2":  transID=data.GetIdTrans(transactionid); mensaje =await GetResultTaecelPS(transID);break;
                case "3":  transID=data.GetIdTrans(transactionid); mensaje =await GetResultTaecelPS(transID); break;
                case "4":  transID= data.GetIdTrans(transactionid);mensaje = await GetResultTaecelPS(transID); break;
                case "5": mensaje = data.GetTicketPrepagoRE(transactionid);break;
            }            
            return mensaje;
        }
        public async Task<string> GetResultTaecel(string transactionID)
        {
            string Ticket = "";
                     
                Models.TaecelModel.StatusTXNRE StatusTXNRE = GetServicesTaeCel.GetStatusTXNRE(transactionID);
                if (StatusTXNRE.error.Equals(0) || StatusTXNRE.message.Equals("Recarga Exitosa"))
                {
                data.updateEndTrans(transactionID, StatusTXNRE.data.Folio, "3");
                    Ticket = "Recarga " + StatusTXNRE.data.Carrier + " exitosa. Folio:" + StatusTXNRE.data.Folio + " # Tel. " + StatusTXNRE.data.Telefono + " Monto:" + StatusTXNRE.data.Monto + " Hora:" + StatusTXNRE.data.Fecha + " ";
                }
                else
                {
                data.updateEndTrans(transactionID, "", "4");
                Ticket = "Error, transacción # " + transactionID + " " + StatusTXNRE.message;
                }
            return Ticket;
        }
        public async Task<string> GetResultTaecelPS(string transactionID)
        {
            string Ticket = "";

            Models.TaecelModel.StatusTXNRE StatusTXNRE = GetServicesTaeCel.GetStatusTXNRE(transactionID);
            if (StatusTXNRE.error.Equals(0) || StatusTXNRE.message.Equals("Recarga Exitosa"))
            {
                data.updateEndTrans(transactionID, StatusTXNRE.data.Folio, "3");
                Ticket = "Pago de servicio " + StatusTXNRE.data.Carrier + " exitosa. Folio:" + StatusTXNRE.data.Folio + " # Referencia. " + StatusTXNRE.data.Telefono + " Monto:" + StatusTXNRE.data.Monto + " Hora:" + StatusTXNRE.data.Fecha + " ";
            }
            else
            {
                data.updateEndTrans(transactionID, "", "4");
                Ticket = "Error, transacción # " + transactionID + " " + StatusTXNRE.message;
            }
            return Ticket;
        }
        public async Task<string> GetTransactionTaecel(string codigo,string referencia)
        {
            Models.TaecelModel.RequestTXNModel requestTXN = GetServicesTaeCel.SendServiceRE(codigo, "0", referencia);
            string respuesta = "";
            if (requestTXN.success)
            {
                if (requestTXN.error.Equals(0))
                {
                    respuesta = requestTXN.data.transID; 
                }
            }
            return respuesta;
        }
        public async Task<string> GeTransactionTaecelPS(string codigo,string monto, string referencia)
        {
            Models.TaecelModel.RequestTXNModel requestTXN = GetServicesTaeCel.SendServicePS(codigo, monto, referencia);
            string respuesta = "";
            if (requestTXN.success)
            {
                if (requestTXN.error.Equals(0))
                {
                    respuesta = requestTXN.data.transID;
                }
            }
            return respuesta;
        }
        getServicesRMovil getServicesR = new getServicesRMovil();
        public async Task<string> GetTransactioPrepaTaeRE(string carrier, string monto,string referencia)
        {
            Models.prepagotaemodel.respuesta respuesta = await getServicesR.DoREPTAE(carrier, referencia, monto);
            if (respuesta.Equals("0"))
            {
                data.updateEndTrans(respuesta.autorizacion, respuesta.autorizacion, "3");
                return "";
            }
            else
            {
                data.updateEndTrans(respuesta.codigo, respuesta.codigo, "4");
                return "Error, " + respuesta.mensaje;
            }
        }
        public async Task<string> GetResultPrepaTaeRE(string transId)
        {
            if (data.GetStatusPrepaTae(transId) != "4")
            {
                return data.GetTicketPrepagoRE(transId);
            }else
            {
                return "Error, Recarga no exitosa";
            }
        }
    }
}
