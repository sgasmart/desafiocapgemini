using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backEnd.Models
{
    public class conta
    {
        private string resulth;
        private string msg;
        private string token;
        private string acao;
        private string cpf;
        private string nome;
        private string cc;
        private string senha;
        private decimal saldo;
        private decimal valor;

        private string cryptographyPass(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public void SetResulth (string value)
        {
            resulth = value;
        }

        public string  GetResulth()
        {
            return resulth;
        }

        public void SetToken(string value)
        {
            token = value;
        }

        public string GetToken()
        {
            return token;
        }

        public void SetAcao(string value)
        {
            acao = value;
        }

        public string GetAcao()
        {
            return acao;
        }

        public void SetConta(string value)
        {
            cc = value;
        }

        public string GetConta()
        {
            return cc;
        }

        public void SetCpf(string value)
        {
            cpf = value;
        }

        public string GetCpf()
        {
            return cpf;
        }

        public void SetNome(string value)
        {
            nome = value;
        }

        public string GetNome()
        {
            return nome;
        }

        public void SetSenha(string value)
        {
            senha = cryptographyPass(value);
        }

        public string GetSenha()
        {
            return senha;
        }

        public void SetValor(decimal value)
        {
            valor = value;
        }

        public decimal GetValor()
        {
            return valor;
        }

        public void SetSaldo(decimal value)
        {
            saldo = value;
        }

        public decimal GetSaldo()
        {
            return saldo;
        }


    }
}