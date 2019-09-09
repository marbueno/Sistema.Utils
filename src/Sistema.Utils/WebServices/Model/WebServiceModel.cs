using Sistema.Utils.Enums.WebServiceEnum;
using System.Collections.Specialized;
using System.Net;

namespace Sistema.Utils.WebServices
{
    public class WebServiceModel
    {
        #region Variables

        public string Url { get; set; }
        public NameValueCollection RequestHeader { get; set; }
        public string RequestData { get; set; }
        public Verbs MethodType { get;set; }
        public string ContentType { get; set; }
        public WebProxy Proxy { get; set; }

        #endregion Variables
    }
}
