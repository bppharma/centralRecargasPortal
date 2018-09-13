using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace centralRecargasPortal.Data
{
    public class dataCentralRecargas
    {
        private string connStr = "Uid=luis;Database=centralrecarga;Pwd=NuevoLui5$__;Host=centralsoftware.c7qjl3xbdhqu.us-east-2.rds.amazonaws.com;";
        public List<Models.RecargasViewModel.servicedetail> Servicedetails(string Carrier)
        {
            List<Models.RecargasViewModel.servicedetail> servicedetails = new List<Models.RecargasViewModel.servicedetail>();
            string query = string.Format("SELECT * FROM centralrecarga.serviciosdetalle where idservicios={0}", Carrier);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            MySqlDataReader msdr = cmd.ExecuteReader();
            while (msdr.Read())
            {
                Models.RecargasViewModel.servicedetail servicedetail = new Models.RecargasViewModel.servicedetail();
                servicedetail.idserviciodetalle = msdr.GetValue(0).ToString();
                servicedetail.idservicio = msdr.GetValue(1).ToString();
                servicedetail.etiqueta = msdr.GetValue(2).ToString();
                servicedetail.codigo = msdr.GetValue(3).ToString();
                servicedetail.vigencia = msdr.GetValue(4).ToString();
                servicedetail.descripcion = msdr.GetValue(5).ToString();
                servicedetails.Add(servicedetail);
            }
            con.Close();
            return servicedetails;
        }
        public string GetCodeServiceDetail(string carrier, string supplier)
        {
            string query = string.Format("SELECT idserviciosdetalle FROM centralrecarga.serviciosdetalle where idservicios={0}", carrier);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetCodeServiceSuplier(string carrier, string supplier)
        {
            string query = string.Format("SELECT codigo FROM centralrecarga.eqservicioproveedor where idserviciosdetalle={0} and idproveedores={1}", carrier, supplier);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetCodeServiceSupplierPS(string carrier, string supplier)
        {
            string query = string.Format("SELECT codigo FROM centralrecarga.eqservicioproveedor where idproveedores={1} and idserviciosdetalle=(select idserviciosdetalle from centralrecarga.serviciosdetalle where idservicios={0})", carrier, supplier);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string returnBetterBalance(string amount, string idservicio)
        {
            string query = string.Format("SELECT idproveedores FROM centralrecarga.balanceproveedores where saldorecargas=(select max(saldorecargas) from centralrecarga.balanceproveedores) and saldorecargas>={0}", amount);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string returnIdTrans(string usuario, string carrier, string monto, string referencia)
        {
            string queryInsert = string.Format("insert into centralrecarga.diario(iduser,ventastart,idservicio,monto,referencia,status) values('{0}',now(),{1}," + monto + ",'{3}',1)", usuario, carrier, monto, referencia);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryInsert, con);
            con.Open();
            cmd.ExecuteNonQuery();
            string retorno = cmd.LastInsertedId.ToString();
            con.Close();
            return retorno;
        }
        public void updateEndTrans(string transactionId, string folio, string status)
        {
            string queryInsert = "update centralrecarga.diario set folio='" + folio + "',status=" + status + ",ventaend=now() where transidprov=" + transactionId;
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryInsert, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void updateIdTrans(string localtransid, string transactionId)
        {
            string queryInsert = "update centralrecarga.diario set cargo=0, transidprov='" + transactionId + "',status=2 where iddiario=" + localtransid;
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryInsert, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public string GetIdTrans(string localtransactionid)
        {
            string query = string.Format("SELECT transidprov FROM centralrecarga.diario where iddiario={0}", localtransactionid);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetTipoServ(string transId)
        {
            string query = string.Format("SELECT idtiposervicio FROM centralrecarga.servicios where idservicios=(select idservicios from centralrecarga.serviciosdetalle where idserviciosdetalle=(select idservicio from centralrecarga.diario where iddiario={0}))", transId);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public void SaveDemonNotifications(int tipoEnvio, string valor, string iddiario)
        {
            string queryInsert = string.Format("insert into centralrecarga.demonTicket(tipoenvio,valor,iddiario,status) values({0},'{1}',{2},0)", tipoEnvio, valor, iddiario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryInsert, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public string ConfiguradoCia(string usuario)
        {
            string query = string.Format("SELECT count(*) from centralrecarga.compania where idusuario='{0}'", usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetIDCia(string usuario)
        {
            string query = string.Format("SELECT idcompania from centralrecarga.compania where idusuario='{0}'", usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetBalanceCia(string usuario)
        {
            string query = string.Format("SELECT saldo from centralrecarga.balancecia where idcompania={0}", usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public void saveCia(string usuarios)
        {
            string queryInsert = string.Format("insert into centralrecarga.compania(idusuario,comercial,razonsocial,domicilio,correo,telefono,rfc,comision) values('{0}','{0}','{0}','','','','','6')", usuarios);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryInsert, con);
            con.Open();
            cmd.ExecuteNonQuery();
            string retorno = cmd.LastInsertedId.ToString();
            con.Close();
            string queryInsertBalance = string.Format("insert into centralrecarga.balancecia(idcompania,saldo) values('{0}',0)", retorno);
            cmd = new MySqlCommand(queryInsertBalance, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public double GetBalanceAvailable(string usuario)
        {
            string query = string.Format("SELECT saldo FROM centralrecarga.balancecia where idcompania=(select idcompania from centralrecarga.compania where idusuario='{0}')", usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return Convert.ToDouble(retorno);
        }
        public double howmuchserviceRE(string idservicio)
        {
            string query = string.Format("SELECT etiqueta FROM centralrecarga.serviciosdetalle where idserviciosdetalle={0})", idservicio);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            double total = Convert.ToDouble(retorno.Replace("$", "").Replace("MXN", "").Replace(" ", "").Trim());
            double retornoD = (total / 10) * 0.6;
            return (total - retornoD);
        }
        public string GetStatusPrepaTae(string transId)
        {
            string query = string.Format("SELECT status FROM centralrecarga.diario where iddiario=", transId);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetTicketPrepagoRE(string transId)
        {
            string query = string.Format("SELECT concat(\"Recarga Exitosa Folio: \",folio,\" Tel. \",referencia,\" Monto: \",monto,\" Fecha: \", ventaend) as ticket FROM centralrecarga.diario where iddiario={0}", transId);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetBestBalanceAndSupplier(string amount, string filter)
        {
            string query = string.Format("select min(idproveedores) from centralrecarga.balanceproveedores where {0}>={1}", filter, amount);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetLocalTransactionID(string localcode, string usuario, string refrencia, string via, string IP, string idsupplier, string cargo, string abono, string montoCargar)
        {
            string queryInsert = "insert into centralrecarga.diario(iduser,ventastart,idservicio,monto,referencia,status,cargo,abono,via,IP,idproveedores,localcode) values";
            queryInsert += "('{0}',now(),(SELECT idserviciosdetalle FROM centralrecarga.serviciosdetalle where codigo='{1}'),{2},'{3}',1,{4},{5},'{6}','{7}','{8}','{9}')";
            string queryExecute = string.Format(queryInsert, usuario, localcode, montoCargar, refrencia, cargo, abono, via, IP, idsupplier, localcode);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryExecute, con);
            con.Open();
            cmd.ExecuteNonQuery();
            string retorno = cmd.LastInsertedId.ToString();
            con.Close();
            return retorno;
        }
        public string GetLocalTransactionIDPS(string localcode, string usuario, string refrencia, string via, string IP, string idsupplier, string cargo, string abono, string montoCargar)
        {
            string queryInsert = "insert into centralrecarga.diario(iduser,ventastart,idservicio,monto,referencia,status,cargo,abono,via,IP,idproveedores,localcode) values";
            queryInsert += "('{0}',now(),(SELECT idserviciosdetalle FROM centralrecarga.serviciosdetalle where idservicios='{1}'),{2},'{3}',1,{4},{5},'{6}','{7}','{8}','{9}')";
            string queryExecute = string.Format(queryInsert, usuario, localcode, montoCargar, refrencia, cargo, abono, via, IP, idsupplier, localcode);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(queryExecute, con);
            con.Open();
            cmd.ExecuteNonQuery();
            string retorno = cmd.LastInsertedId.ToString();
            con.Close();
            return retorno;
        }
        public string GetSupplierCode(string localcode, string idsupplier)
        {
            string query = string.Format("select codigo from centralrecarga.eqservicioproveedor where idserviciosdetalle=(select idserviciosdetalle from centralrecarga.serviciosdetalle where codigo='{0}' and idproveedores={1})", localcode, idsupplier);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public string GetNotesticket(string localcode, string usuario)
        {
            string query = string.Format("select concat(cia.comercial,\"|\",serv.nombre,\"|\",serv.nota) as nota from centralrecarga.compania cia, centralrecarga.servicios serv where serv.idservicios={0} and cia.idusuario='{1}'", localcode.Split('-')[0], usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return retorno;
        }
        public void UpdateBalanceCia(string newBalance, string usuario, string idsupplier, string serviceamount)
        {
            string query = string.Format("update centralrecarga.balancecia set saldo=saldo-{0} where idcompania=(select idcompania from centralrecarga.compania where idusuario='{1}')", newBalance, usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            string queryToAdd = "";
            if (idsupplier.Equals("2"))
            {
                queryToAdd = ",saldoservicios=saldoservicios-{0}";
            }
            string query2 = string.Format("update centralrecarga.balanceproveedores set saldorecargas=saldorecargas-{0}{2} where idproveedores={1}", serviceamount, idsupplier, queryToAdd);
            cmd = new MySqlCommand(query2, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public double GetFeeRE(string usuario)
        {
            string query = string.Format("select comision from centralrecarga.compania where idusuario='{0}'", usuario);
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            string retorno = cmd.ExecuteScalar().ToString();
            con.Close();
            return (Convert.ToDouble(retorno) / 10);
        }
        public void SaveDepositClient(Models.centralRecargaAPIModel.comprarView comprar)
        {
            string queryValida = string.Format("select count(*) from centralrecarga.depositos where idcuenta={0} and idformapago={1} and fecha='{2}' and monto={3} and folio='{4}'", comprar.banco, comprar.formapago, (comprar.fecha + " " + comprar.hora), comprar.monto, comprar.folio);
            MySqlConnection con1 = new MySqlConnection(connStr);
            MySqlCommand cmd1 = new MySqlCommand(queryValida, con1);
            con1.Open();
            string retorno = cmd1.ExecuteScalar().ToString();
            con1.Close();
            if (retorno.Equals("0"))
            {
                string queryInsert = string.Format("insert into centralrecarga.depositos(idcuenta,idformapago,fecha,monto,folio,status,idcompania,aplicado) values({0},{1},'{2}',{3},'{4}',0,(select idcompania from compania where idusuario='{5}'),0)", comprar.banco, comprar.formapago, (comprar.fecha + " " + comprar.hora), comprar.monto, comprar.folio, comprar.APIK);
                MySqlConnection con = new MySqlConnection(connStr);
                MySqlCommand cmd = new MySqlCommand(queryInsert, con);
                con.Open();
                cmd.ExecuteScalar();
                con.Close();
            }
        }
        public List<Models.ReportesModel.ReporteVenta> GetReporteVenta(Models.ReportesModel.ReportesViewModel reportesView)
        {
            List<Models.ReportesModel.ReporteVenta> reportes = new List<Models.ReportesModel.ReporteVenta>();
            string query = "SELECT dia.iddiario as ticket,iduser,ventastart,sd.descripcion,dia.monto,dia.referencia,(case when dia.status=4 then 'Error' when dia.status=3 then 'Exitosa' when dia.status=2 then 'En Espera de respuesta' when dia.status=1 then 'Enviada' end) as estatus,dia.folio,dia.cargo,dia.abono,dia.via FROM centralrecarga.diario dia inner join centralrecarga.serviciosdetalle sd on sd.idserviciosdetalle=dia.idservicio where iduser in (select idusuario from centralrecarga.compania where idcompania=(select idcompania from centralrecarga.compania where idusuario='"+reportesView.usuario+"')) order by 1 desc";
            MySqlConnection con = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            MySqlDataReader msdr = cmd.ExecuteReader();
            while (msdr.Read())
            {
                Models.ReportesModel.ReporteVenta reporte = new Models.ReportesModel.ReporteVenta();
                reporte.Ticket = msdr.GetValue(0).ToString();
                reporte.Producto = msdr.GetValue(3).ToString();
                reporte.Monto = msdr.GetValue(4).ToString();
                reporte.Referencia = msdr.GetValue(5).ToString();
                reporte.Autorización = msdr.GetValue(7).ToString();
                reporte.Cajero = msdr.GetValue(1).ToString();
                reporte.Canal = msdr.GetValue(10).ToString();
                reporte.Estatus = msdr.GetValue(6).ToString();
                reporte.Cargo = msdr.GetValue(8).ToString();
                reporte.Abono = msdr.GetValue(9).ToString();
                reportes.Add(reporte);
            }
            con.Close();
            return reportes;
        }
        #region queries

        #endregion
    }
}
