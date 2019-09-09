using System.Text.RegularExpressions;

namespace Sistema.Utils.Utils
{
    public sealed class ValidarCPF_CNPJ
    {
        public static bool ValidarCPF(string CPF)
        {
            if (CPF == null)
            {
                return false;
            }
            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            CPF = Regex.Replace(CPF, @"\D+", @"");
            CPF = CPF.Trim();
            CPF = CPF.Replace(".", "").Replace("-", "");
            CPF = CPF.PadLeft(11, '0');

            var regex = new Regex(@"^\d{11}$");
            if (!regex.IsMatch(CPF))
            {
                return false;
            }

            if (CPF == "00000000000" || CPF == "11111111111" || CPF == "22222222222" || CPF == "33333333333" || CPF == "44444444444" || CPF == "55555555555" || CPF == "66666666666" || CPF == "77777777777" || CPF == "88888888888" || CPF == "99999999999")
            {
                return false;
            }

            var v1 = 0;
            for (int i = 0; i < 9; i++)
            {
                v1 += int.Parse(CPF[i].ToString()) * (multiplicador1[i]);
            }
            v1 = 11 - (v1 % 11);
            if (v1 >= 10)
                v1 = 0;

            var v2 = 0;
            for (int i = 0; i < 9; i++)
            {
                v2 += int.Parse(CPF[i].ToString()) * multiplicador2[i];
            }
            v2 += 2 * v1;
            v2 = 11 - v2 % 11;
            if (v2 >= 10)
                v2 = 0;

            return v1 == int.Parse(CPF[9].ToString()) && v2 == int.Parse(CPF[10].ToString());
        }

        public static bool ValidarCNPJ(string CNPJ)
        {
            if (CNPJ == null)
            {
                return false;
            }

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            CNPJ = Regex.Replace(CNPJ, @"\D+", @"");
            CNPJ = CNPJ.Trim();
            CNPJ = CNPJ.PadLeft(14, '0');

            var regex = new Regex(@"^\d{14}$");
            if (!regex.IsMatch(CNPJ))
            {
                return false;
            }

            if (CNPJ == "00000000000000")
            {
                return false;
            }

            tempCnpj = CNPJ.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            if (CNPJ.EndsWith(digito))
            {
                return true;
            }

            return false;
        }
    }
}
