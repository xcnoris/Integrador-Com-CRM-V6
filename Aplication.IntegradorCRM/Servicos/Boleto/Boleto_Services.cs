using DataBase.IntegradorCRM.Data;
using Metodos.IntegradorCRM.Metodos;
using Modelos.IntegradorCRM.Models.EF;
using Modelos.IntegradorCRM.Models;
using Modelos.IntegradorCRMRM.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Modelos.IntegradorCRM.Models.Enuns;

namespace Aplication.IntegradorCRM.Servicos.Boleto
{
    internal class Boleto_Services
    {
        public string Message;
        public bool Status;
        private readonly CobrancaServicos CobrancasNaSegunda;

        #region Metodos Gerais
      
        public async Task VerificarQuitacao(int situacao, RelacaoBoletoCRMModel BoletoRelacao, List<AcaoSituacao_Boleto_CRM> AcoesSituacaoBoleto, string codigoJornada, DadosAPIModels DadosAPI, bool InTBRelacao)
        {
            // Verifica se o boleto já esta pago, caso esteja muda o boleto para fase Pago/Aguardando Liberação
            if ((Situacao_Boleto)situacao == Situacao_Boleto.Quitado)
            {
                // Atualize para a etapa pago no CRM, e atualiza no banco
                BoletoRelacao.Quitado = 1;
                try
                {
                    await QuitacaoBoleto.Quitar(AcoesSituacaoBoleto, BoletoRelacao, InTBRelacao, DadosAPI);
                }
                catch (Exception ex)
                {
                    //MetodosGerais.RegistrarLog("", "");
                    throw new Exception(ex.Message);
                }
            }
        }

       

        internal async Task VerificarBoletosCriadosNoCRM(RelacaoBoletoCRMModel BoletoRelacao, int diasAtraso, int situacao, int situacaTBRelacao,  DadosAPIModels DadosAPI, List<AcaoSituacao_Boleto_CRM> AcoesSituacaoBoleto, List<BoletoAcoesCRMModel> AcoesBoletoList)
        {
            if (!string.IsNullOrEmpty(BoletoRelacao.Cod_Oportunidade))
            {
                using (var dalBoletoUsing = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext()))
                {
                    // Verifica se o boleto está em atraso
                    if (diasAtraso > 0)
                    {
                        // Verifica a situação do boleto (3 = cancelado/estornado, 2 = quitado)
                        switch ((Situacao_Boleto)situacao)
                        {
                            // Verifica se esta cancelado
                            case Situacao_Boleto.Cancelada_Ou_Estornado:
                                if ((Situacao_Boleto)situacaTBRelacao != Situacao_Boleto.Cancelada_Ou_Estornado)
                                {
                                    await CancelarBoleto(AcoesSituacaoBoleto, BoletoRelacao, DadosAPI);
                                }
                                else
                                {
                                    MetodosGerais.RegistrarLog("BOLETO", $"Boleto já existe na tabela relação. Já está ajustado como Cancelado/Estornado.");
                                }
                                break;
                            // Verifica se esta quitado
                            case Situacao_Boleto.Quitado:
                                if (BoletoRelacao.Quitado == 0)
                                {
                                    await QuitarBoleto(AcoesSituacaoBoleto, BoletoRelacao, DadosAPI);
                                }
                                else
                                {
                                    MetodosGerais.RegistrarLog("BOLETO", $"Boleto já existe na tabela relação. Está quitado!");
                                }
                                break;

                            // Caso não seja quitado nem cancelado, faz a cobrança
                            default:
                                await RealizarCobrancas(AcoesBoletoList, diasAtraso, BoletoRelacao.DiasEmAtraso, BoletoRelacao, DadosAPI);
                                break;
                        }
                    }
                    // Caso o boleto não esteja em atraso
                    else
                    {
                        switch ((Situacao_Boleto)situacao)
                        {
                            case Situacao_Boleto.Quitado:
                                if (BoletoRelacao.Quitado == 0)
                                {
                                    await QuitarBoleto(AcoesSituacaoBoleto, BoletoRelacao, DadosAPI);
                                }
                                else
                                {
                                    MetodosGerais.RegistrarLog("BOLETO", $"Boleto já existe na tabela relação. Está quitado!");
                                }
                                break;

                            case Situacao_Boleto.Cancelada_Ou_Estornado:
                                if ((Situacao_Boleto)situacaTBRelacao != Situacao_Boleto.Cancelada_Ou_Estornado)
                                {
                                    await CancelarBoleto(AcoesSituacaoBoleto, BoletoRelacao, DadosAPI);
                                }
                                else
                                {
                                    MetodosGerais.RegistrarLog("BOLETO", $"Boleto já existe na tabela relação. Já está ajustado como Cancelado/Estornado.");
                                }
                                break;

                            default:
                                await RealizarCobrancas(AcoesBoletoList, diasAtraso, BoletoRelacao.DiasEmAtraso, BoletoRelacao, DadosAPI);
                           
                                break;
                        }
                    }
                }
            }
        }

        private async Task RealizarCobrancas(List<BoletoAcoesCRMModel> AcoesBoletoList, int diasAtraso, int DiasAtrasoRelBoleto, RelacaoBoletoCRMModel boletoRelacao, DadosAPIModels DadosAPI)
        {
            await VerificacaoDeCobranca.RealizarCobranca(AcoesBoletoList, diasAtraso, DiasAtrasoRelBoleto, boletoRelacao, DadosAPI);
        }
        private async Task QuitarBoleto(List<AcaoSituacao_Boleto_CRM> AcoesSituacaoBoleto, RelacaoBoletoCRMModel BoletoRelacao,DadosAPIModels DadosAPI)
        {
            await QuitacaoBoleto.Quitar(AcoesSituacaoBoleto, BoletoRelacao, true, DadosAPI);
        }

        private async Task CancelarBoleto(List<AcaoSituacao_Boleto_CRM> AcaoSituacaoBoleto, RelacaoBoletoCRMModel BoletoRelacao , DadosAPIModels DadosAPI)
        {
            await CancelamentoBoleto.Cancelar(AcaoSituacaoBoleto, BoletoRelacao, DadosAPI);
        }
        #endregion

        #region Metodo API

        // Método auxiliar para enviar requisição de criação de oportunidade para a API
        internal static async Task<OportunidadeResponse> EnviarRequisicaoCriarOportunidade(ModeloOportunidadeRequest request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {

                    var response = await client.PostAsync("https://api.leadfinder.com.br/integracao/v2/inserirOportunidade", content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        OportunidadeResponse opt = JsonConvert.DeserializeObject<OportunidadeResponse>(responseBody);
                        MetodosGerais.RegistrarLog("BOLETO", " Resposta OK - Oportunidade criada no CRM" + responseBody);
                        return opt;
                    }

                    MetodosGerais.RegistrarLog("BOLETO", $"Erro na resposta da API: Status {response.StatusCode} - {responseBody}");
                }
                catch (HttpRequestException ex)
                {
                    MetodosGerais.RegistrarLog("BOLETO", $"Erro de rede ao chamar API: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    MetodosGerais.RegistrarLog("BOLETO", $"Exceção ao processar resposta da API: {ex.Message}");
                    throw;
                }

                return null;
            }
        }

        // Método auxiliar para adicionar o boleto no banco de dados
        internal static async Task AdicionarBoletoNoBanco(DAL<RelacaoBoletoCRMModel> dalTableRelacaoBoleto, RelacaoBoletoCRMModel boletoInTabRel, string codigoOportunidade)
        {
            boletoInTabRel.Cod_Oportunidade = codigoOportunidade;
            boletoInTabRel.Data_Criacao = DateTime.Now;

            using (var dalBoletoUsing = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext()))
            {
                await dalBoletoUsing.AdicionarAsync(boletoInTabRel);
            }
        }

        internal static async Task<OportunidadeResponse> AtualizarOportunidadeNaApi(AtualizarAcaoRequest request, string token)
        {
            // configurar o cabeçalho de autorização 
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("https://api.leadfinder.com.br/integracao/movimentarOportunidade", content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        MetodosGerais.RegistrarLog("BOLETO", $"Resposta OK - Oportunidade Atualizada no CRM {responseBody}");
                        return JsonConvert.DeserializeObject<OportunidadeResponse>(responseBody);
                    }

                    MetodosGerais.RegistrarLog("BOLETO", $"Erro na resposta da API: Status {response.StatusCode} - {responseBody}");
                }
                catch (HttpRequestException ex)
                {
                    MetodosGerais.RegistrarLog("BOLETO", $"Erro de rede ao chamar API: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    MetodosGerais.RegistrarLog("BOLETO", $"Exceção ao processar resposta da API: {ex.Message}");
                    throw;
                }

                return null;
            }
        }

        // Método auxiliar para atualizar boleto no banco
        internal static async Task AtualizarBoletoNoBanco(RelacaoBoletoCRMModel boletoRelacao)
        {
            boletoRelacao.Data_Atualizacao = DateTime.Now;

            using (var dalBoletoUsing = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext()))
            {
                await dalBoletoUsing.AtualizarAsync(boletoRelacao);
            }
        }

        // Metodo Recebe Boletos que tenham sido quitados, o boleto será excluido da tabela de cobrança de fim de semana caso tenha
        internal static async Task ProcessarBoletoQuitado(RelacaoBoletoCRMModel boletoRelacao)
        {
            var cobrancas = new CobrancaServicos();
            await cobrancas.RemoverRegistro(boletoRelacao.Id, true);
            MetodosGerais.RegistrarLog("BOLETO", $"Situação atualizada para {boletoRelacao.Situacao} para o documento {boletoRelacao.Id_DocumentoReceber}");
        }

        #endregion
    }
}
