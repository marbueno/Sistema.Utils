using Newtonsoft.Json;
using Sistema.Utils.Utils;
using Sistema.Utils.Utils.Model;
using Sistema.Utils.ValidarBanco;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Sistema.Utils
{
    public static class Helper
    {
        #region Methods

        public static string SerializeXml<T>(this T Obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(Obj.GetType());

            using (var sww = new StringWriter())
            {
                MemoryStream strm = new MemoryStream();

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                settings.CloseOutput = true;
                settings.Encoding = Encoding.Default;
                var xml = "";

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                ns.Add("", "");

                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    try
                    {
                        writer.WriteStartDocument();
                        xsSubmit.Serialize(writer, Obj, ns);

                        return xml = sww.ToString();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        public static string GerarXml(this Dictionary<string, string> Parametros)
        {
            string parametersText;

            if (Parametros != null && Parametros.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var Parametro in Parametros)
                    sb.AppendFormat("  <{0}>{1}</{0}>\r\n", Parametro.Key, Parametro.Value);

                parametersText = sb.ToString();
            }
            else
            {
                parametersText = "";
            }

            return parametersText;
        }

        public static bool ValidarCPFouCNPJ(string CPFouCNPJ)
        {
            if (CPFouCNPJ.Length > 12)
                return ValidarCPF_CNPJ.ValidarCNPJ(CPFouCNPJ);
            else
                return ValidarCPF_CNPJ.ValidarCPF(CPFouCNPJ);
        }

        public static ResultadoBanco ValidarAgenciaConta(Bancos Banco, string Agencia, string Conta)
        {
            Results result = ValidarBancos.Validar(Banco, Regex.Replace(Agencia, "[^0-9a-zA-Z]+", ""), Regex.Replace(Conta, "[^0-9a-zA-Z]+", ""));
            var resultadoBanco = new ResultadoBanco() { };
            resultadoBanco.Valido = false;

            switch (result)
            {
                case Results.AgenciaInvalida:
                    if(Banco == Bancos.Santander || Banco ==Bancos.Bradesco)
                        resultadoBanco.Mensagem = "Agência Inválida. (Verifique o dígito da sua Agência)";
                    else
                        resultadoBanco.Mensagem = "Agência Inválida.";  

                    break;
                case Results.ContaInvalida:
                    resultadoBanco.Mensagem = "Conta Inválida.";
                    break;
                case Results.AgenciaeContaInvalida:
                    resultadoBanco.Mensagem = "Agência/Conta Inválida.";
                    break;
                case Results.CaixaDebitoInvalido:
                    resultadoBanco.Mensagem = @"Não é permitido o débito em contas com tipo de conta (operação) = 013, 023 e 003"+
                                               "(conta poupança, conta salário e conta de pessoa jurídica) ";
                    break;
                default:
                    resultadoBanco.Mensagem = "Dados válidos";
                    resultadoBanco.Valido = true;
                    break;
            }

            return resultadoBanco;
        }

        public static void ObterParametros<TClasse>(NameValueCollection parametros, TClasse classe)
        {
            foreach (var item in classe.GetType().GetProperties())
            {
                ObterParametros(parametros, item.Name, item.GetValue(classe, null));
            }
        }

        public static void ObterParametros<TClasse>(NameValueCollection parametros, string Json)
        {
            TClasse classe = JsonConvert.DeserializeObject<TClasse>(Json);

            ObterParametros<TClasse>(parametros, classe);
        }

        public static void ObterParametros(NameValueCollection parametros, Dictionary<string, object> dicionario)
        {
            foreach (KeyValuePair<string, object> item in dicionario)
            {
                if (item.Key != null & item.Value != null)
                    ObterParametros(parametros, item.Key, item.Value.ToString());
            }
        }

        public static void ObterParametros(NameValueCollection parametros, string nomeParametro, string valor)
        {
            if (parametros.AllKeys.Contains(nomeParametro))
                parametros[nomeParametro] = valor;
            else
                parametros.Add(nomeParametro, valor);
        }

        public static void ObterParametros(NameValueCollection parametros, string nomeParametro, object valor)
        {
            ObterParametros(parametros, nomeParametro, Convert.ToString((valor == null ? "" : valor)));
        }

        public static string ReplaceParameters(string texto, string tagInicial, string tagFinal, NameValueCollection parametros)
        {
            foreach (var item in parametros)
            {
                if (item != null)
                {
                    if (parametros[item.ToString()] != null)
                    {
                        texto = texto.Replace(tagInicial + item.ToString() + tagFinal, parametros[item.ToString()]);
                    }
                }
            }

            return texto;
        }

        public static string FormatarCPFouCNPJ(string CPFouCNPJ)
        {
            if (CPFouCNPJ != null)
            {
                CPFouCNPJ = SomenteNumeros(CPFouCNPJ);

                if (CPFouCNPJ.Length <= 11)
                {
                    return Convert.ToUInt64(CPFouCNPJ).ToString(@"000\.000\.000\-00");
                }
                else
                {
                    return Convert.ToUInt64(CPFouCNPJ).ToString(@"00\.000\.000\/0000\-00");
                }
            }

            return string.Empty;
        }

        public static string FormatarCEP(string CEP)
        {
            if (CEP != null)
            {
                CEP = SomenteNumeros(CEP);

                return Convert.ToUInt64(CEP).ToString(@"00000\-000");
            }

            return string.Empty;
        }

        public static string SomenteNumeros(string Texto)
        {
            return new Regex(@"[^\d]").Replace(Texto, "");
        }

        public static string RemoveSpecialCharacters(string text, string CaracterOptional, bool allowSpace = false)
        {
            string ret;

            if (allowSpace)
                ret = Regex.Replace(text, $@"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ{CaracterOptional}\s]+?", string.Empty);
            else
                ret = Regex.Replace(text, $@"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ{CaracterOptional}]+?", string.Empty);

            return ret;
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] > 255)
                    sb.Append(text[i]);
                else
                    sb.Append(s_Diacritics[text[i]]);
            }

            return sb.ToString();
        }

        private static readonly char[] s_Diacritics = GetDiacritics();

        private static char[] GetDiacritics()
        {
            char[] accents = new char[256];

            for (int i = 0; i < 256; i++)
                accents[i] = (char)i;

            accents[(byte)'á'] = accents[(byte)'à'] = accents[(byte)'ã'] = accents[(byte)'â'] = accents[(byte)'ä'] = 'a';
            accents[(byte)'Á'] = accents[(byte)'À'] = accents[(byte)'Ã'] = accents[(byte)'Â'] = accents[(byte)'Ä'] = 'A';

            accents[(byte)'é'] = accents[(byte)'è'] = accents[(byte)'ê'] = accents[(byte)'ë'] = 'e';
            accents[(byte)'É'] = accents[(byte)'È'] = accents[(byte)'Ê'] = accents[(byte)'Ë'] = 'E';

            accents[(byte)'í'] = accents[(byte)'ì'] = accents[(byte)'î'] = accents[(byte)'ï'] = 'i';
            accents[(byte)'Í'] = accents[(byte)'Ì'] = accents[(byte)'Î'] = accents[(byte)'Ï'] = 'I';

            accents[(byte)'ó'] = accents[(byte)'ò'] = accents[(byte)'ô'] = accents[(byte)'õ'] = accents[(byte)'ö'] = 'o';
            accents[(byte)'Ó'] = accents[(byte)'Ò'] = accents[(byte)'Ô'] = accents[(byte)'Õ'] = accents[(byte)'Ö'] = 'O';

            accents[(byte)'ú'] = accents[(byte)'ù'] = accents[(byte)'û'] = accents[(byte)'ü'] = 'u';
            accents[(byte)'Ú'] = accents[(byte)'Ù'] = accents[(byte)'Û'] = accents[(byte)'Ü'] = 'U';

            accents[(byte)'ç'] = 'c';
            accents[(byte)'Ç'] = 'C';

            accents[(byte)'ñ'] = 'n';
            accents[(byte)'Ñ'] = 'N';

            accents[(byte)'ÿ'] = accents[(byte)'ý'] = 'y';
            accents[(byte)'Ý'] = 'Y';

            return accents;
        }

        public static string FormatReal(decimal valor, bool prefix = false)
        {
            string valorFinal = string.Empty;

            valorFinal = String.Format(new CultureInfo("pt-BR"), "{0:C}", valor);

            if (!prefix)
                valorFinal = valorFinal.Replace("R$", "").Trim();

            return valorFinal;
        }

        public static decimal UnFormatReal(string valorFormatado)
        {
            decimal valorFinal = 0M;
            CultureInfo culturaBrasileira = new CultureInfo("pt-BR");

            valorFinal = decimal.Parse(valorFormatado, NumberStyles.Currency, culturaBrasileira);

            return valorFinal;
        }

        public static string DataPorExtenso (DateTime data)
        {
            string dia = data.Day.ToString("00");
            string mes = string.Empty;
            string ano = data.Year.ToString("0000");

            if (data.Month == 1) mes = "Janeiro";
            if (data.Month == 2) mes = "Fevereiro";
            if (data.Month == 3) mes = "Março";
            if (data.Month == 4) mes = "Abril";
            if (data.Month == 5) mes = "Maio";
            if (data.Month == 6) mes = "Junho";
            if (data.Month == 7) mes = "Julho";
            if (data.Month == 8) mes = "Agosto";
            if (data.Month == 9) mes = "Setembro";
            if (data.Month == 10) mes = "Outubro";
            if (data.Month == 11) mes = "Novembro";
            if (data.Month == 12) mes = "Dezembro";

            return $"{dia} de {mes} de {ano}";
        }

        public static string FormatTelefone(string numeroTelefone)
        {
            if (string.IsNullOrEmpty(numeroTelefone))
            {
                return string.Empty;
            }
            else
            {
                string strMascara = "{0:0000-0000}";

                numeroTelefone = numeroTelefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "");

                long lngNumero = Convert.ToInt64(numeroTelefone);

                if (numeroTelefone.Length <= 8)
                    strMascara = "{0:0000-0000}";

                else if (numeroTelefone.Length == 9)
                    strMascara = "{0:00000-0000}";

                else if (numeroTelefone.Length == 10)
                    strMascara = "{0:(00) 0000-0000}";
                else
                    strMascara = "{0:(00) 00000-0000}";

                return string.Format(strMascara, lngNumero);
            }
        }

        #endregion Methods
    }
}
