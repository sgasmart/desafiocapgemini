using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace backEnd.Models
{
    public class contaModel
    {
        private string resulth = "{\"error\":\"true\",\"msg\":\"error\"}";
        private NpgsqlConnection conPg = null;
        private string connString = "Server=localhost;Port=5434;UserID=postgres;password=sistema;Database=capgemini";
        encript cript = new encript();
        
        public string getConta(conta cc)
        {
            
            try
            {
                conPg = new NpgsqlConnection(connString);
                conPg.Open();
                string sql = "SELECT conta, cliente, saldo, cpf, senha FROM contas WHERE cpf='"+ cc.GetCpf() + "' and senha='"+ cc.GetSenha() + "'; ";
                //Console.Write(sql);
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conPg);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                DataTable dt = new DataTable();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string chaveCript= "YG5M5GHUJH4HJGGX";
                    string vetor = "JWEIGJF23JGZFJ3I";
                    string token = encript.Encriptar(chaveCript, vetor, dt.Rows[0][3].ToString() + ";" + dt.Rows[0][4].ToString() + ";" + dt.Rows[0][0].ToString());
                    resulth = "{\"error\":\"false\",\"msg\":{\"cc\":\"" + dt.Rows[0][0].ToString() + "\",\"cliente\":\"" + dt.Rows[0][1].ToString() + 
                               "\",\"saldo\":\"" + dt.Rows[0][2].ToString() + "\",\"token\":\"" + token + "\"}}";
                }
                else
                {
                    resulth = "{\"error\":\"true\",\"msg\":\"Senha ou o CPF: " + cc.GetCpf() + " estão inválidos.\"}";
                }
                da.Dispose();
                conPg.Close();
            }
            catch (Exception ex)
            {
                resulth =  "{\"error\":\"true\",\"msg\":\"" + ex.Message.ToString() +"\"}"; 
            }

            return resulth;
        }

        public string getToken(string token)
        {

            try
            {
                conPg = new NpgsqlConnection(connString);
                conPg.Open();
                string chaveCript = "YG5M5GHUJH4HJGGX";
                string vetor = "JWEIGJF23JGZFJ3I";
                string[] tk = encript.Decriptar(chaveCript, vetor, token).Split(';');

                string sql = "SELECT conta, cliente, saldo, cpf, senha FROM contas WHERE cpf='" +tk[0]+ "' and senha='" + tk[1] + "'; ";
                //Console.Write(sql);
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conPg);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                DataTable dt = new DataTable();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    token = encript.Encriptar(chaveCript, vetor, dt.Rows[0][3].ToString() + ";" + dt.Rows[0][4].ToString() + ";" + dt.Rows[0][0].ToString());
                    resulth = "{\"error\":\"false\",\"msg\":{\"cc\":\"" + dt.Rows[0][0].ToString() + "\",\"cliente\":\"" + dt.Rows[0][1].ToString() +
                               "\",\"saldo\":\"" + dt.Rows[0][2].ToString() + "\",\"token\":\"" + token + "\"}}";
                }
                else
                {
                    resulth = "{\"error\":\"true\",\"msg\":\"token iválido.\"}";
                }
                da.Dispose();
                conPg.Close();
            }
            catch (Exception ex)
            {
                resulth = "{\"error\":\"true\",\"msg\":\"" + ex.Message.ToString() + "\"}";
            }

            return resulth;
        }

        public string setMovimentacao(conta cc)
        {
            try
            {


                string acao = null;
                string chaveCript = "YG5M5GHUJH4HJGGX";
                string vetor = "JWEIGJF23JGZFJ3I";
                string[] tk = encript.Decriptar(chaveCript, vetor, cc.GetToken()).Split(';');
                if (cc.GetAcao() == "sacar")
                {
                    acao = "-";
                }
                if (cc.GetAcao() == "depositar")
                {
                    acao = "+";
                }
                if (!string.IsNullOrEmpty(acao))
                {
                    conPg = new NpgsqlConnection(connString);
                    conPg.Open();
                    string sql = "SELECT data FROM movimentacoes WHERE  conta='" + tk[2] + "' AND cpf='" + tk[0] + "' AND tipo='" + acao + "' AND valor='" + cc.GetValor() + "';";
                    //Console.Write(sql);
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, conPg);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    DataTable dt = new DataTable();
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    da.Fill(dt);
                    string dataMov = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    bool validaTime = true;
                    if (dt.Rows.Count > 0)
                    {
                        string dataHoraInicial = dt.Rows[0][0].ToString();
                        TimeSpan tempoAtendHoras = Convert.ToDateTime(dataMov) - Convert.ToDateTime(dataHoraInicial);
                        if (tempoAtendHoras.Seconds <= 5)
                        {
                            validaTime = false;
                        }
                    }

                    conPg = new NpgsqlConnection(connString);
                    conPg.Open();
                    sql = "BEGIN;";
                    if (validaTime == true)
                    {
                        sql += "INSERT INTO movimentacoes(data, conta, cpf, tipo, valor) VALUES('" + dataMov + "', '" + tk[2] + "', '" + tk[0] + "', '" + acao + "', '" + cc.GetValor() + "'); " +
                        "UPDATE contas SET movimentacao='" + dataMov + "', saldo = (saldo" + acao + cc.GetValor() + ")  WHERE cpf='" + tk[0] + "' AND senha='" + tk[1] + "' AND conta='" + tk[2] + "';"; 
                    }
                    sql += "SELECT conta, cliente, saldo, cpf, senha FROM contas WHERE cpf='" + tk[0] + "' AND senha='" + tk[1] + "' AND conta='" + tk[2] + "'; " +
                        "COMMIT;";
                    //Console.Write(sql);
                    cmd = new NpgsqlCommand(sql, conPg);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    dt = new DataTable();
                    da = new NpgsqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        string token = encript.Encriptar(chaveCript, vetor, dt.Rows[0][3].ToString() + ";" + dt.Rows[0][4].ToString() + ";" + dt.Rows[0][0].ToString());
                        resulth = "{\"error\":\"false\",\"msg\":{\"cc\":\"" + dt.Rows[0][0].ToString() + "\",\"cliente\":\"" + dt.Rows[0][1].ToString() +
                                   "\",\"saldo\":\"" + dt.Rows[0][2].ToString() + "\",\"token\":\"" + token + "\"}}";
                    }
                    else
                    {
                        resulth = "{\"error\":\"true\",\"msg\":\"Senha ou o CPF: " + tk[0] + " estão inválidos.\"}";
                    }
                    da.Dispose();
                    conPg.Close();
                }
                else
                {
                    resulth = "{\"error\":\"true\",\"msg\":\"tipo de acao não é válido.\"}";
                }

            }
            catch (Exception ex)
            {
                resulth = "{\"error\":\"true\",\"msg\":\"" + ex.Message.ToString() + "\"}";
            }

            return resulth;
        }
    }
}