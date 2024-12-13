using DataBase.IntegradorCRM.Data.DataBase;
using Metodos.IntegradorCRM.Metodos;
using Modelos.IntegradorCRMRM.Models;
using System.Data;

namespace Aplication.IntegradorCRM.Metodos.Boleto
{
    internal class CrudBoleto
    {
        private ConexaoDB _conexaoDB;
        private ComandosDB _comandosDB;

        public CrudBoleto()
        {
            string Validacao = "";
            _conexaoDB = new ConexaoDB(Validacao);
            _comandosDB = new ComandosDB(_conexaoDB);
        }

        internal List<RetornoBoleto> BuscarBoletosInDB()
        {
            try
            {

                // Buscar serviços no banco de dados a partir de uma data ou parâmetro definido
                string query = @"
                     SELECT 
                        DR.numero_documento_receber,
                        DR.id_documento_receber,
                        DR.id_entidade,
                        ent.nome,
                        CONCAT(COALESCE(ent.celular_ddd, ''), COALESCE(ent.celular_numero, '')) AS celular,
                        ent.email_principal,
                        CASE 
                            WHEN ent.tipo_entidade = 1 THEN pf.cpf 
                            WHEN ent.tipo_entidade = 2 THEN pj.cnpj 
                            ELSE 'Tipo de entidade desconhecido' 
                        END AS identificador_cliente,
	                    DR.situacao,
	                    DR.data_vencimento
                    FROM 
                        documento_receber DR
                    INNER JOIN 
                        entidade ent ON DR.id_entidade = ent.id_entidade
                    LEFT JOIN 
                        pessoa_juridica pj ON ent.id_entidade = pj.id_entidade AND ent.tipo_entidade = 2
                    LEFT JOIN 
                        pessoa_fisica pf ON ent.id_entidade = pf.id_entidade AND ent.tipo_entidade = 1
                    WHERE 
                        DR.data_vencimento >= '01/12/2024'
                    AND 
	                    DR.tem_boleto = 1
                    AND 
	                	ent.id_entidade = 1383
                ";

                // Converte o resultado do select em DataTable
                DataTable retornoOS = _comandosDB.ExecuteQuery(query);

                List<RetornoBoleto> retornoBoletos = DataTableToList(retornoOS);

                return retornoBoletos;
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("OS", $"[ERROR]: {ex.Message} - {_comandosDB.Mensagem}");

                return null;
            }
        }

        private List<RetornoBoleto> DataTableToList(DataTable dt)
        {
            try
            {
                List<RetornoBoleto> listaRetornoOS = new List<RetornoBoleto>();

                foreach (DataRow linha in dt.Rows)
                {
                    RetornoBoleto RBoleto = new RetornoBoleto()
                    {
                        Num_DocReceber = linha["numero_documento_receber"].ToString(),
                        Id_DocReceber = linha["id_documento_receber"].ToString(),
                        Id_Entidade = linha["id_entidade"].ToString(),
                        Nome = linha["nome"].ToString(),
                        Celular = linha["celular"].ToString(),
                        Email = linha["email_principal"].ToString(),
                        Identificador_Cliente = linha["identificador_cliente"].ToString(),
                        Situacao = linha["situacao"].ToString(),
                        Data_Vencimento = Convert.ToDateTime(linha["data_vencimento"])
                    };

                    listaRetornoOS.Add(RBoleto);
                }
                return listaRetornoOS;
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("OS", ex.Message);
                
                return null;
            }
        }
    }
}
