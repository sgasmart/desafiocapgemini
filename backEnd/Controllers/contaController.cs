using backEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace backEnd.Controllers
{
    public class contaController : Controller
    {
        [EnableCors(origins: "http://localhost:55810/", headers: "*", methods: "*")]
       
        //
        // GET: conta
        public ActionResult Index()
        {
            return View();
        }

        string headers = "header(\"Access-Control-Allow-Origin: *\");"+
                         "header('Access-Control-Allow-Methods: GET, POST, OPTIONS');"+
                         "header(\"Access-Control-Allow-Headers: Content-Type, Authorization\");";

        private string result = "";

        public string getConta(string cpf, string senha)
        {
            if (!string.IsNullOrEmpty(cpf) && !string.IsNullOrEmpty(senha))
            {
                conta objConta = new conta();
                objConta.SetResulth("");
                objConta.SetSenha(senha);
                objConta.SetCpf(cpf);
                contaModel dados = new contaModel();
                return dados.getConta(objConta);
            }
            else
            {
                result = "{\"error\":\"false\",\"msg\":\"campos cpf e senha são exigidos!\"}";
                return result;
            }
        }

        public string validar(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                contaModel dados = new contaModel();
                return dados.getToken(token);
            }
            else
            {
                result = "{\"error\":\"false\",\"msg\":\"token é exigido!\"}";
                return result;
            }
        }


        public string getSaldo(decimal valor, string cc, string senha)
        {

            return result;
        }

        public string setMovimentacao(string acao, string valor, string token)
        {
            if (acao == "depositar" || acao == "sacar")
            {
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(valor))
                {
                    conta objConta = new conta();
                    objConta.SetResulth("");
                    objConta.SetToken(token);
                    decimal val = Convert.ToDecimal(valor.Replace(".", "").Replace(",", "."));
                    objConta.SetValor(val);
                    objConta.SetAcao(acao);
                    contaModel dados = new contaModel();
                    return dados.setMovimentacao(objConta);

                }
                else
                {
                    result = "{\"error\":\"false\",\"msg\":\"campos cpf, valor, cc e senha são exigidos!\"}";
                    return result;
                }
            }
            else
            {
                result = "{\"error\":\"false\",\"msg\":\"acao não reconhecida!\"}";
                return result;
            }
        }

    }
}