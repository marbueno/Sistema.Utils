using Sistema.Utils.ValidarBanco;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xunit;

namespace Sistema.Utils.Test.Utils
{
    public class UtilsTest
    {
        #region Classes

        public class Teste
        {
            public int id { get; set; }
            public string descricao { get; set; }
        }

        #endregion Classes

        #region Variables

        private readonly Teste teste;

        #endregion Variables

        #region Constructor

        public UtilsTest()
        {
            teste = new Teste()
            {
                id = 1,
                descricao = "teste"
            };
        }

        #endregion Constructor

        #region Methods

        [Fact]
        public void Deve_Retonar_Um_Xml()
        {
            string xmlEsperado = "<Teste><id>1</id><descricao>teste</descricao></Teste>";
            string xmlRetornado = teste.SerializeXml().Replace("\r\n", "").Replace(" ", "").Trim();
            Assert.Equal(xmlEsperado, xmlRetornado);
        }

        [Fact]
        public void Deve_Gerar_Um_Xml()
        {
            string xmlEsperado = "<id>1</id><descricao>teste</descricao>";

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("id", "1");
            dic.Add("descricao", "teste");

            string xmlRetornado = dic.GerarXml().Replace("\r\n", "").Replace(" ", "").Trim(); ;
            Assert.Equal(xmlEsperado, xmlRetornado);
        }

        [Fact]
        public void Deve_Validar_CPF_Como_Correto()
        {
            string cpf = "32518799877";
            bool cpfCorreto = Helper.ValidarCPFouCNPJ(cpf);
            Assert.True(cpfCorreto);
        }

        [Fact]
        public void Deve_Validar_CPF_Como_Incorreto()
        {
            string cpf = "12312312322";
            bool cpfCorreto = Helper.ValidarCPFouCNPJ(cpf);
            Assert.False(cpfCorreto);
        }

        [Fact]
        public void Deve_Validar_CNPJ_Como_Correto()
        {
            string cpf = "00746092000153";
            bool cpfCorreto = Helper.ValidarCPFouCNPJ(cpf);
            Assert.True(cpfCorreto);
        }

        [Fact]
        public void Deve_Validar_CNPJ_Como_Incorreto()
        {
            string cpf = "00223222999902";
            bool cpfCorreto = Helper.ValidarCPFouCNPJ(cpf);
            Assert.False(cpfCorreto);
        }


        [Fact]
        public void Deve_Converter_Classe_Para_NameValueCollection()
        {
            NameValueCollection parametros = new NameValueCollection();
            Teste teste = new Teste()
            {
                id = 1,
                descricao = "teste"
            };
            Helper.ObterParametros<Teste>(parametros, teste);

            bool result = parametros.AllKeys.Contains("id");

            Assert.True(result);
        }

        [Fact]
        public void Deve_Converter_Json_Para_NameValueCollection()
        {
            NameValueCollection parametros = new NameValueCollection();
            string json = "{\"id\":1, \"descricao\":\"teste\"}";

            Helper.ObterParametros<Teste>(parametros, json);

            bool result = parametros.AllKeys.Contains("id");

            Assert.True(result);
        }

        [Fact]
        public void Deve_Fazer_Replace_Dos_Parametros_Do_Texto()
        {
            NameValueCollection parametros = new NameValueCollection();
            parametros.Add("_nome", "Marcelo");
            parametros.Add("_idade", "34");

            string texto = "Nome: ${_nome} | Idade: ${_idade}";

            texto = Helper.ReplaceParameters(texto, "${", "}", parametros);

            bool result = parametros.AllKeys.Contains("${_nome}");

            Assert.False(result);
        }

        [Fact]
        public void Deve_Formatar_CPF()
        {
            string CPF = "11122233344";

            string result = Helper.FormatarCPFouCNPJ(CPF);

            Assert.True(result == "111.222.333-44");
        }

        [Fact]
        public void Deve_Formatar_CNPJ()
        {
            string CNPJ = "00746092000153";

            string result = Helper.FormatarCPFouCNPJ(CNPJ);

            Assert.True(result == "00.746.092/0001-53");
        }

        [Fact]
        public void Deve_Formatar_CEP()
        {
            string CEP = "02130040";

            string result = Helper.FormatarCEP(CEP);

            Assert.True(result == "02130-040");
        }

        [Fact]
        public void Deve_Formatar_Data_Por_Extenso()
        {
            string dataPorExtenso = Helper.DataPorExtenso(DateTime.Now);

            Assert.True(!string.IsNullOrEmpty(dataPorExtenso));
        }

        [Fact]
        public void Deve_Formatar_Telefone()
        {
            string telefone = Helper.FormatTelefone("");

            Assert.True(!string.IsNullOrEmpty(telefone));
        }


        #region Valida Bancos

        //BRADESCO
        [Fact]
        public void Deve_Validar_Digito_Agencia_Conta_Bradesco_Como_Valido()
        {
            Bancos banco = Bancos.Bradesco;
            string agencia = "3859";
            string conta = "24872";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);
            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Agencia_Conta_Bradesco_Como_Invalido()
        {
            Bancos banco = Bancos.Bradesco;
            string agencia = "3859";
            string conta = "24172";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);
            Assert.True(result.Valido);
        }

        //CAIXA ECONOMICA
        [Fact]
        public void Deve_Validar_Digito_Conta_Caixa_Como_Valido()
        {
            Bancos banco = Bancos.CaixaEconomicaFederal;
            string agencia = "3788";
            string conta = "00100022976-0";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_Caixa_Como_Invalido()
        {
            Bancos banco = Bancos.CaixaEconomicaFederal;
            string agencia = "3788";
            string conta = "00100022971-0";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        //BANCO DO BRASIL
        [Fact]
        public void Deve_Validar_Digito_Conta_BancoBrasil_Como_Valido()
        {
            Bancos banco = Bancos.BancodoBrasil;
            string agencia = "0038-8";
            string conta = "00056771-X";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_BancoBrasil_Como_Invalido()
        {
            Bancos banco = Bancos.BancodoBrasil;
            string agencia = "0038-8";
            string conta = "00056271-X";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }


        //ITAU
        [Fact]
        public void Deve_Validar_Digito_Conta_Itau_Como_Valido()
        {
            Bancos banco = Bancos.Itau;
            string agencia = "1012";
            string conta = "21787-1";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_Itau_Como_Invalido()
        {
            Bancos banco = Bancos.Itau;
            string agencia = "1013";
            string conta = "21787-1";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }


        //SANTANDER
        [Fact]
        public void Deve_Validar_Digito_Conta_Santander_Como_Valido()
        {
            Bancos banco = Bancos.Santander;
            string agencia = "3875";
            string conta = "10905576";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_Santander_Como_Invalido()
        {
            Bancos banco = Bancos.Santander;
            string agencia = "3875";
            string conta = "10902576";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }



        //CITIBANK
        [Fact]
        public void Deve_Validar_Digito_Conta_CitiBank_Como_Valido()
        {
            Bancos banco = Bancos.CitiBank;
            string agencia = "0075";
            string conta = "0007500465-8";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_CitiBank_Como_Invalido()
        {
            Bancos banco = Bancos.CitiBank;
            string agencia = "0075";
            string conta = "0007400465-8";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        //BANRISUL
        [Fact]
        public void Deve_Validar_Digito_Conta_Banrisul_Como_Valido()
        {
            Bancos banco = Bancos.Banrisul;
            string agencia = "2664-18";
            string conta = "35.850767.0 - 6";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_Banrisul_Como_Invalido()
        {
            Bancos banco = Bancos.Banrisul;
            string agencia = "2634-18";
            string conta = "35.890767.0 - 6";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }


        //BANRISUL
        [Fact]
        public void Deve_Validar_Digito_Conta_HSBC_Como_Valido()
        {
            Bancos banco = Bancos.HSBC;
            string agencia = "0007";
            string conta = "853838-6";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }

        [Fact]
        public void Deve_Validar_Digito_Conta_HSBC_Como_Invalido()
        {
            Bancos banco = Bancos.HSBC;
            string agencia = "0007";
            string conta = "853138-6";
            var result = Helper.ValidarAgenciaConta(banco, agencia, conta);

            Assert.True(result.Valido);
        }



        #endregion


        #endregion Methods
    }
}
