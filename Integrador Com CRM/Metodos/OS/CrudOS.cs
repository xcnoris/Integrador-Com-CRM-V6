﻿using Integrador_Com_CRM.DataBase;
using Integrador_Com_CRM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrador_Com_CRM.Metodos.OS
{
    internal class CrudOS
    {


        private ConexaoDB _conexaoDB;
        private ComandosDB _comandosDB;

        public CrudOS()
        {
            _conexaoDB = new ConexaoDB();
            _comandosDB = new ComandosDB(_conexaoDB);
        }

        internal List<RetornoOrdemServico> BuscarOrdemDeServiçoInDB()
        {
            try
            {

                // Buscar serviços no banco de dados a partir de uma data ou parâmetro definido
                string query = @"SELECT 
                        os.id_ordem_servico, 
                        CASE 
                            WHEN e.tipo_entidade = 1 THEN pf.cpf 
                            WHEN e.tipo_entidade = 2 THEN pj.cnpj 
                            ELSE 'Tipo de entidade desconhecido' 
                        END AS identificador_cliente,
                        os.nome_cliente, 
                        CONCAT(os.fone_ddd_cliente, os.fone_numero_cliente) AS telefone,
                        os.email_cliente,
                        os.id_categoria_ordem_servico
                    FROM 
                        ordem_servico os
                    INNER JOIN 
                        entidade e ON os.id_entidade_cliente = e.id_entidade
                    LEFT JOIN 
                        pessoa_juridica pj ON e.id_entidade = pj.id_entidade AND e.tipo_entidade = 2
                    LEFT JOIN 
                        pessoa_fisica pf ON e.id_entidade = pf.id_entidade AND e.tipo_entidade = 1
                    WHERE 
                         OS.data_hora_cadastro >= '18/07/2024'
                ";
                //string query = "SELECT id_ordem_servico, nome_cliente, fone_ddd_cliente + fone_numero_cliente AS telefone, email_cliente, id_categoria_ordem_servico FROM ordem_servico WHERE id_ordem_servico = 8674";

                // Converte o resultado do select em DataTable
                List<RetornoOrdemServico> retornoOSs= _comandosDB.ExecuteQuery(query);

                MetodosGerais.RegistrarLog("OS", $"Foram encontradas {retornoOSs.Count()} ordem de serviço no banco de dados\n");
                return retornoOSs;
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("OS", $"[ERROR]: {ex.Message} - {_comandosDB.Mensagem}");
                return null;
            }
        }

    }
}
