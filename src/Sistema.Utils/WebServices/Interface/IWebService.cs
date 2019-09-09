using System.Net;

namespace Sistema.Utils.WebServices
{
    public interface IWebService
    {
        #region Methods
        HttpWebResponse Call();

        #endregion Methods
    }
}
