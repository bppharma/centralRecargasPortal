using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using System.Xml;

namespace centralRecargasPortal.Controllers
{
    public class getServicesRMovil
    {
        private string UI = "luisbriseno";
        private string PI = "NuevoLui5$__";
        //private string UR = "";
        public async Task<Models.prepagotaemodel.respuesta> DoREPTAE(string carrier,string referencia,string monto)
        {
            var client = new RestClient("https://demo.prepagotae.com/services/ventasSoap?wsdl=");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "b3cc836c-2f6c-4613-854d-7d609f0a28e3");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "text/xml");
            request.AddParameter("undefined", "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:soap=\"http://soap/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <soap:recargaTae>\r\n         <user>"+UI+"</user>\r\n         <password>"+PI+"</password>\r\n         <carrier>"+carrier+"</carrier>\r\n         <telefono>"+referencia+"</telefono>\r\n         <monto>"+monto+"</monto>\r\n      </soap:recargaTae>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Models.prepagotaemodel.respuesta respuesta = new Models.prepagotaemodel.respuesta();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content.ToString());
            XmlNodeList nodeList = doc.GetElementsByTagName("data");
            string respuu = "";
            foreach(XmlNode xmlNode in nodeList)
            {
                foreach (XmlNode xmlNode2 in xmlNode)
                {
                    respuu = xmlNode2.Value;
                }
            }
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(respuu);
            respuesta.codigo = doc2.SelectSingleNode("//codigo").InnerText;
            respuesta.autorizacion = doc2.SelectSingleNode("//autorizacion").InnerText;
            respuesta.mensaje = doc2.SelectSingleNode("//mensaje").InnerText;
            return respuesta;            
        }
        public async Task<Models.prepagotaemodel.respuesta> DoPSPTAE(string carrier,string referencia,string referencia2,string monto)
        {
            var client = new RestClient("https://demo.prepagotae.com/services/ventasSoap?wsdl=");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "d1e704c5-f1d5-4eab-8f1e-8b405ce31c4b");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "text/xml");
            request.AddParameter("undefined", "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:soap=\"http://soap/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <soap:pagoServicios>\r\n         <user>"+UI+"</user>\r\n         <password>"+PI+"</password>\r\n         <idServicio>"+carrier+"</idServicio>\r\n         <referencia>"+referencia+"</referencia>\r\n         <digitoVerificador>"+referencia2+"</digitoVerificador>\r\n         <monto>"+monto+"</monto>\r\n      </soap:pagoServicios>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Models.prepagotaemodel.respuesta respuesta = new Models.prepagotaemodel.respuesta();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content.ToString());
            XmlNodeList nodeList = doc.GetElementsByTagName("data");
            string respuu = "";
            foreach (XmlNode xmlNode in nodeList)
            {
                foreach (XmlNode xmlNode2 in xmlNode)
                {
                    respuu = xmlNode2.Value;
                }
            }
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(respuu);
            respuesta.codigo = doc2.SelectSingleNode("//codigo").InnerText;
            try
            {
                respuesta.autorizacion = doc2.SelectSingleNode("//autorizacion").InnerText;
            }
            catch { }
            respuesta.mensaje = doc2.SelectSingleNode("//mensaje").InnerText;
            return respuesta;
        }
    }
}
