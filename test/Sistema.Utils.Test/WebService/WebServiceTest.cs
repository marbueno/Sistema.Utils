using Sistema.Utils.WebServices;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Xunit;

namespace Sistema.Utils.Test.WebService
{
    public class WebServiceTest
    {
        #region Variables

        IWebService webServiceBase { get; }
        NameValueCollection requestHeader = new NameValueCollection();

        const string username = "Sistema";
        const string password = "LUSIVRp@2018*";
        readonly string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

        readonly string soapTemplate = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:tem=\"http://tempuri.org/\">"
                            + "<soap:Header/>"
                            + "<soap:Body>"
                            + "<tem:LocalizaPessoas>"
                            + " <tem:documento>32518799877</tem:documento>"
                            + "</tem:LocalizaPessoas>"
                            + "</soap:Body>"
                            + "</soap:Envelope>";

        #endregion Variables

        #region Methods

        public WebServiceTest()
        {
            requestHeader.Add("Authorization", $"Basic {encoded}");

            webServiceBase = new WebServiceBase(new WebServiceModel()
            {
                Url = "http://wscx.unitfour.com.br/intouchws.asmx?wsdl",
                RequestHeader = requestHeader,
                RequestData = soapTemplate,
                Proxy = new WebProxy("webproxy.Sistema.intranet", 80),
                ContentType = "text/xml; encoding='utf-8'",
                MethodType = Enums.WebServiceEnum.Verbs.POST
            });
        }

        [Fact]
        public void Deve_Retornar_Um_HttpWebResponse()
        {
            HttpWebResponse response = webServiceBase.Call();
            var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Assert.IsType<HttpWebResponse>(response);
        }

        #endregion Methods
    }
}
