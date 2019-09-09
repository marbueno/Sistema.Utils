using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Sistema.Utils.WebServices
{
    public class WebServiceBase : IWebService
    {
        #region Variables
        private readonly WebServiceModel _model;
        #endregion Variables

        #region Methods
        public WebServiceBase(WebServiceModel model)
        {
            _model = model;

            if (_model == null) throw new ArgumentException("Parametro do construtor é obrigatório.");
            if (string.IsNullOrEmpty(_model.Url)) throw new ArgumentException("Url não informada.");
            if (_model.MethodType == Enums.WebServiceEnum.Verbs.POST && string.IsNullOrEmpty(_model.RequestData)) throw new ArgumentException("Dados para requisição não informado.");
        }
        private bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
        {
            // all Certificates are accepted
            return true;
        }

        public HttpWebResponse Call()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;

                Uri uri = new Uri(_model.Url);

                var request = (HttpWebRequest)WebRequest.Create(uri);

                if (_model.RequestHeader != null)
                {
                    request.Headers.Add(_model.RequestHeader);
                }

                if (_model.Proxy != null)
                {
                    request.Proxy = _model.Proxy;
                    request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                }

                if (_model.RequestData != null)
                {
                    byte[] bytes;
                    bytes = Encoding.ASCII.GetBytes(_model.RequestData);

                    request.ContentType = _model.ContentType;
                    request.ContentLength = bytes.Length;
                    request.Method = _model.MethodType.ToString();

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }

                var response = (HttpWebResponse)request.GetResponse();

                return response;
            }
            catch (WebException ex)
            {
                throw new ArgumentException(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Erro ao conectar no WebService: [{ex.Message}]");
            }
        }

        #endregion Methods
    }
}
