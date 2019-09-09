using System;
using System.Linq;

namespace Sistema.Utils.ValidarBanco
{
    public enum Results
    {
        AgenciaInvalida,
        ContaInvalida,
        AgenciaeContaInvalida,
        Valido,
        CaixaDebitoInvalido
    }
    public enum Bancos
    {
        Itau = 341,
        Bradesco = 237,
        Santander = 033,
        CitiBank = 477,
        BancodoBrasil = 001,
        Banrisul = 041,
        HSBC = 399,
        CaixaEconomicaFederal = 104
    }

    public sealed class ValidarBancos
    {
        public static Results Validar(Bancos banco, string agencia, string conta)
        {
            switch (banco)
            {
                case Bancos.Itau:
                    //Tamanho da Agência - 4 Dígitos
                    //Tamanho da Conta   - 5 Dígitos + 1 DV
                    return Itau(agencia.PadLeft(4, '0'), conta.PadLeft(6, '0'));

                case Bancos.Bradesco:
                    //Tamanho da Agência - 4 Dígitos + 1 DV
                    //Tamanho da Conta   - 7 Dígitos + 1 DV
                    return Bradesco(agencia.PadLeft(5, '0'), conta.PadLeft(8, '0'));

                case Bancos.Santander:
                    // Tamanho da Agência - 4 Dígitos
                    // Tamanho da Conta   - 8 dígitos + 1 DV
                    return Santander(agencia.PadLeft(4, '0'), conta.PadLeft(9, '0'));

                case Bancos.CitiBank:
                    //Tamanho da Agência - 4 Dígitos
                    //Tamanho da Conta   - 7 Dígitos + 1 DV
                    return CitiBank(agencia.PadLeft(4, '0'), conta.PadLeft(8, '0'));

                case Bancos.BancodoBrasil:
                    //Tamanho da Agência - 4 Dígitos + 1 DV
                    //Tamanho da Conta   - 8 Dígitos + 1 DV
                    return BancodoBrasil(agencia.PadLeft(5, '0'), conta.PadLeft(9, '0'));

                case Bancos.Banrisul:
                    //Tamanho da Agência - 4 Dígitos + 2 DV
                    //Tamanho da Conta   - 9 Dígitos + 1 DV(sendo os dois primeiros o tipo deconta)
                    return Banrisul(agencia.PadLeft(6, '0'), conta.PadLeft(10, '0'));

                case Bancos.HSBC:
                    //Tamanho da Agencia - 4 Dígitos
                    //Tamanho da Conta   - 6 Dígitos + 1 DV
                    return HSBC(agencia.PadLeft(4, '0'), conta.PadLeft(7, '0'));

                case Bancos.CaixaEconomicaFederal:
                    //Tamanho da Agência - 4 Dígitos
                    //Tamanho da Conta   - 11 Dígitos + 1 DV
                    return CaixaEconomicaFederal(agencia.PadLeft(5, '0'), conta.PadLeft(12, '0'));

            }
            return Results.AgenciaeContaInvalida;
        }
        private static Results Itau(string agencia, string conta)
        {
            return Itau(agencia + "-" + conta);
        }
        private static Results Itau(string conta)
        {
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            String[] _conta = conta.Split('-');

            conta = _conta[0] + _conta[1];
            int conta_dig = int.Parse(_conta[2].ToUpper() == "0" ? "10" : _conta[2]);

            var fator = new int[] { 1, 2 };

            int i;
            int z;
            int soma;
            int p;

            i = conta.Length % fator.Length; z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    String intermediario = (((int)conta.ElementAt(p) - 48) * fator[k]).ToString();
                    for (int K = 0; K < intermediario.Length; K++)
                    {
                        soma += ((int)intermediario.ElementAt(K) - 48);
                    }
                    k++; p++;
                }
            }
            int conta_resto = 10 - soma % 10;
            if (conta_dig != conta_resto) { return Results.ContaInvalida; }

            return Results.Valido;
        }
        private static Results Bradesco(string agencia, string conta)
        {
            if (agencia.ElementAt(agencia.Length - 2) != '-') { agencia = agencia.Substring(0, agencia.Length - 1) + "-" + agencia.Substring(agencia.Length - 1); }

            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            String[] _agencia = agencia.Split('-');

            agencia = _agencia[0];
            int agencia_dig = int.Parse(_agencia[1].ToUpper() == "P" ? "10" : _agencia[1]);

            String[] _conta = conta.Split('-');

            conta = _conta[0];
            int conta_dig = int.Parse(_conta[1].ToUpper() == "P" ? "10" : _conta[1]);

            var fator = new int[] { 7, 6, 5, 4, 3, 2 };

            int i;
            int z;
            int soma;
            int p;

            i = (agencia.Length) % fator.Length;
            z = mathFloor(((agencia.Length) / fator.Length));

            soma = 0;
            p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)agencia.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int agencia_resto = 11 - soma % 11; agencia_resto = agencia_resto > 9 ? 0 : agencia_resto;

            if (agencia_resto == 11)
                agencia_resto = 0;

            i = (conta.Length) % fator.Length;
            z = mathFloor(((conta.Length) / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int conta_resto = 11 - soma % 11; conta_resto = conta_resto > 9 ? 0 : conta_resto;

            if (agencia_dig != agencia_resto && conta_dig == conta_resto) { return Results.AgenciaInvalida; }
            else if (agencia_dig == agencia_resto && conta_dig != conta_resto) { return Results.ContaInvalida; }
            else if (agencia_dig != agencia_resto && conta_dig != conta_resto) { return Results.AgenciaeContaInvalida; }

            return Results.Valido;
        }
        private static Results Santander(string agencia, string conta)
        {
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }
            if (conta.Length < 9) { conta = "01" + conta; }
            else if (conta.Length == 9) { conta = "0" + conta; }
            return Santander(agencia + "-" + conta.Substring(0, 2) + "-" + conta.Substring(2));
        }
        private static Results Santander(string agencia, string subconta, string conta)
        {
            return Santander(agencia + "-" + subconta + "-" + conta);
        }
        protected static Results Santander(string conta)
        {
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            String[] _conta = conta.Split('-');
            conta = _conta[0] + _conta[1] + _conta[2];
            int conta_dig = int.Parse(_conta[3].ToUpper() == "X" ? "10" : _conta[3]);

            var fator = new int[] { 9, 7, 3, 1, 9, 7, 1, 3, 1, 9, 7, 3 };

            int i;
            int z;
            int soma;
            int p;

            i = conta.Length % fator.Length; z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int conta_resto = 10 - soma % 10;

            if (conta_resto == 10) { conta_resto = 0; }

            if (conta_dig != conta_resto) { return Results.ContaInvalida; }

            return Results.Valido;
        }
        private static Results CitiBank(string agencia, string conta)
        {
            if (agencia.ElementAt(agencia.Length - 2) != '-') { agencia = agencia.Substring(0, agencia.Length - 1) + "-" + agencia.Substring(agencia.Length - 1); }
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            //String []_agencia = agencia.Split('-')
            //agencia = _agencia[0];
            //int agencia_dig = int.Parse(_agencia[1]);

            String[] _conta = conta.Split('-');

            conta = _conta[0];
            int conta_dig = int.Parse(_conta[1]);

            var fator = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            int i;
            int z;
            int soma;
            int p;

            //i = agencia.Length % fator.Length; z =  mathFloor((agencia.Length / fator.Length)
            //soma=0;p=0;
            //for(int t=z; t>=0; t--) {
            //	int k=t==z?fator.Length-i:0;
            //	while(k<fator.Length) {
            //		soma += ((int)agencia.ElementAt(p) - 48) * fator[k];
            //		k++;p++;
            //	}
            //}
            //soma = soma*10
            //int agencia_resto = soma - ( mathFloor((soma/11) * 11);

            i = conta.Length % fator.Length; z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            soma = soma * 10;

            int conta_resto = soma - (mathFloor((soma / 11) * 11));

            /*if(agencia_dig != agencia_resto && conta_dig == conta_resto) return Validation.AGENCIA_INVALIDA
            else*/

            if (/*agencia_dig == agencia_resto && */conta_dig != conta_resto) { return Results.ContaInvalida; }
            //else if(agencia_dig != agencia_resto && conta_dig != conta_resto) return Validation.AGENCIA_E_CONTA_INVALIDOS
            return Results.Valido;
        }
        private static Results BancodoBrasil(string agencia, string conta)
        {
            if (agencia.ElementAt(agencia.Length - 2) != '-') { agencia = agencia.Substring(0, agencia.Length - 1) + "-" + agencia.Substring(agencia.Length - 1); }
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            String[] _agencia = agencia.Split('-');

            agencia = _agencia[0];
            int agencia_dig = int.Parse(_agencia[1].ToUpper() == "X" ? "10" : _agencia[1]);

            String[] _conta = conta.Split('-');

            conta = _conta[0];
            int conta_dig = int.Parse(_conta[1].ToUpper() == "X" ? "10" : _conta[1]);

            var fator = new int[] { 2, 3, 4, 5, 6, 7, 8, 9 };

            int i;
            int z;
            int soma;
            int p;

            i = agencia.Length % fator.Length; z = mathFloor((agencia.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)agencia.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int agencia_resto = soma % 11;

            i = conta.Length % fator.Length; z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int conta_resto = soma % 11;

            if (agencia_dig != agencia_resto && conta_dig == conta_resto) { return Results.AgenciaInvalida; }
            else if (agencia_dig == agencia_resto && conta_dig != conta_resto) { return Results.ContaInvalida; }
            else if (agencia_dig != agencia_resto && conta_dig != conta_resto) { return Results.AgenciaeContaInvalida; }

            return Results.Valido;
        }
        private static Results Banrisul(string agencia, string conta)
        {
            if (agencia.ElementAt(agencia.Length - 2) != '-') { agencia = agencia.Substring(0, agencia.Length - 1) + "-" + agencia.Substring(agencia.Length - 1); }
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            String[] _agencia = agencia.Split('-');

            agencia = _agencia[0];
            int agencia_dig = int.Parse(_agencia[1].ToUpper() == "X" ? "10" : _agencia[1]);

            String[] _conta = conta.Split('-');

            conta = _conta[0];
            int conta_dig = int.Parse(_conta[1].ToUpper() == "X" ? "10" : _conta[1]);

            var fator = new int[] { 3, 2, 4, 7, 6, 5, 4, 3, 2 };

            int i;
            int z;
            int soma;
            int p;

            i = agencia.Length % fator.Length; z = mathFloor((agencia.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)agencia.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int agencia_resto = soma % 11;


            i = conta.Length % fator.Length; z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int conta_resto = soma % 11;

            if (conta_resto == 0) conta_resto = 0;
            else if (conta_resto == 1) conta_resto = 6;
            else conta_resto = 11 - conta_resto;

            if (conta_dig != conta_resto) { return Results.ContaInvalida; }
            /*if(agencia_dig != agencia_resto && conta_dig == conta_resto) return Validation.AGENCIA_INVALIDA
		    //else */
            if (agencia_dig == agencia_resto && conta_dig != conta_resto) { return Results.ContaInvalida; }
            //else if(agencia_dig != agencia_resto && conta_dig != conta_resto) return Validation.AGENCIA_E_CONTA_INVALIDOS
            return Results.Valido;
        }
        private static Results HSBC(string agencia, string conta)
        {
            return HSBC(agencia + "-" + conta);
        }
        private static Results HSBC(string conta)
        {
            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            String[] _conta = conta.Split('-');

            conta = _conta[0] + _conta[1];
            int conta_dig = int.Parse(_conta[2]);
            var fator = new int[] { 2, 3, 4, 5, 6, 7, 8, 9 } as int[];
            int i;
            int z;
            int soma;
            int p;

            i = conta.Length % fator.Length; z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }
            int conta_resto = soma % 11; conta_resto = conta_resto == 10 ? 0 : conta_resto;

            if (conta_dig != conta_resto) { return Results.ContaInvalida; }

            return Results.Valido;

        }
        private static Results CaixaEconomicaFederal(string agencia, string conta)
        {
            //    - TIPOS DE OPERAÇÕES DA CAIXA 
            //      Não é permitido o débito em contas com tipo de conta = 013, 023 e 003
            //      (conta poupança, conta salário e conta de pessoa jurídica) 
            //001 – Conta Corrente de Pessoa Física
            //002 – Conta Simples de Pessoa Física
            //003 – Conta Corrente de Pessoa Jurídica 
            //006 – Entidades Públicas
            //007 – Depósitos Instituições Financeiras 
            //013 – Poupança de Pessoa Física
            //022 – Poupança de Pessoa Jurídica
            //023 – Conta Caixa Fácil
            //028 – Poupança de Crédito Imobiliário
            //032 – Conta Investimento Pessoa Física
            //034 – Conta Investimento Pessoa Jurídica
            //037 – Conta Salário
            //043 – Depósitos Lotéricos
            //131 – Poupança Integrada

            if (conta.StartsWith("022") || conta.StartsWith("023")) { return Results.ContaInvalida; }

            if (conta.ElementAt(conta.Length - 2) != '-') { conta = conta.Substring(0, conta.Length - 1) + "-" + conta.Substring(conta.Length - 1); }

            if (!conta.StartsWith("001") && !conta.StartsWith("002") && !conta.StartsWith("003") && !conta.StartsWith("006") && !conta.StartsWith("007") && !conta.StartsWith("013") && !conta.StartsWith("022") && !conta.StartsWith("023")) { return Results.ContaInvalida; }

            String[] _conta = conta.Split('-');

            conta = _conta[0];
            int conta_dig = int.Parse(_conta[1].ToUpper() == "X" ? "10" : _conta[1]);

            var fator = new int[] { 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int i;
            int z;
            int soma;
            int p;

            conta = agencia + conta;

            i = conta.Length % fator.Length;
            z = mathFloor((conta.Length / fator.Length));

            soma = 0; p = 0;
            for (int t = z; t >= 0; t--)
            {
                int k = t == z ? fator.Length - i : 0;
                while (k < fator.Length)
                {
                    soma += ((int)conta.ElementAt(p) - 48) * fator[k];
                    k++; p++;
                }
            }

            int soma10 = soma * 10;

            int conta_resto = ((mathFloor(((soma10) / 11))) * 11 - (soma10)) * -1;


            if (conta_resto == 10) { conta_resto = 0; }
            if (conta_dig != conta_resto) { return Results.AgenciaeContaInvalida; }

            return Results.Valido;
        }
        private static int mathFloor(int value)
        {
            return int.Parse(Math.Floor(double.Parse((value).ToString())).ToString());
        }
    }
}
