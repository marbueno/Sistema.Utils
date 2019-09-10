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

        const string token = "8b9e51833fb7baeaa17826e793617c30";
        readonly string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":"));

        readonly string requestData = "{\"email\":\"marbue @ig.com.br\",\"cc_emails\":\"\",\"due_date\":\"2019-09-20\",\"ensure_workday_due_date\":true,\"items\":[{\"description\":\"PRODUTO 0001\",\"quantity\":1,\"price_cents\":1.0}],\"custom_variables\":[],\"order_id\":\"0001\"}";

        #endregion Variables

        #region Methods

        public WebServiceTest()
        {
            requestHeader.Add("Authorization", $"Basic {encoded}");

            webServiceBase = new WebServiceBase(new WebServiceModel()
            {
                Url = "https://api.iugu.com/v1/invoices",
                RequestHeader = requestHeader,
                RequestData = requestData,
                ContentType = "application/json; encoding='utf-8'",
                MethodType = Enums.WebServiceEnum.Verbs.POST
            });
        }

        [Fact]
        public void Deve_Retornar_Um_HttpWebResponse()
        {
            try
            {
                HttpWebResponse response = webServiceBase.Call();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Assert.IsType<HttpWebResponse>(response);
            }
            catch (WebException ex)
            {
                Console.Write(ex);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        #endregion Methods
    }
}
