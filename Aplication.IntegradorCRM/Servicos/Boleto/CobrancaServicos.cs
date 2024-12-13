using Aplication.IntegradorCRM.Metodos.Boleto;
using DataBase.IntegradorCRM.Data;
using Metodos.IntegradorCRM.Metodos;
using Microsoft.Data.SqlClient;
using Modelos.IntegradorCRM.Models;
using Modelos.IntegradorCRM.Models.EF;

namespace Aplication.IntegradorCRM.Servicos.Boleto
{
    public class CobrancaServicos
    {

        internal DAL<RelacaoBoletoCRMModel> dalRelBoleto;
        internal DAL<CobrancasNaSegundaModel> dalCobrancas;
        private Boleto_Services metodosBoleto;
        private CobrancasNaSegundaModel cobrancasNaSegundaModel;


        //private readonly Frm_BoletoAcoesCRM_UC FrmBoletoAcao;


        public CobrancaServicos()
        {
            dalCobrancas = new DAL<CobrancasNaSegundaModel>(new IntegradorDBContext());
            dalRelBoleto = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext());
        }

        internal async Task SalvarDadosEmTableEspera(CobrancasNaSegundaModel cobrancasNaSegundaModel)
        {
            try
            {
                await dalCobrancas.AdicionarAsync(cobrancasNaSegundaModel);
                MetodosGerais.RegistrarLog("COBRANCA", $"Ação {cobrancasNaSegundaModel.NovoAtrasoBoleto} marcada para ser cobrado na segunda para o boletoBoleto {cobrancasNaSegundaModel.BoletoId}. CodOp: {cobrancasNaSegundaModel.Cod_Oportunidade}");
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("COBRANCA", $"[ERROR]: {ex.Message}");
                throw;
            }
        }

        public  async Task RealizarCobrancas(List<BoletoAcoesCRMModel> acoesCobrancaList, DadosAPIModels DadosAPI)
        {
            try
            {
                using DAL<CobrancasNaSegundaModel> dalCobranca = new DAL<CobrancasNaSegundaModel>(new IntegradorDBContext());
                using DAL<RelacaoBoletoCRMModel> dalRelBoletos = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext());

                // Busca as cobranças que devem ser realiada
                List<CobrancasNaSegundaModel?> ListCobrancas = (await dalCobranca.ListarAsync() ?? Enumerable.Empty<CobrancasNaSegundaModel>()).ToList();
                if (ListCobrancas is not null)
                {
                    MetodosGerais.RegistrarLog("COBRANCA", $"Nenhum Boleto para ser feita a cobrança!");
                    return;
                }

                foreach (CobrancasNaSegundaModel conbranca in ListCobrancas)
                {
                    RelacaoBoletoCRMModel? boletoRelacao = dalRelBoletos.BuscarPor(x => x.Id == conbranca.BoletoId);

                    if (boletoRelacao is null)
                    {
                        MetodosGerais.RegistrarLog("COBRANCA", $"Nenhum encotrando na TB relação com o Id {conbranca.BoletoId}!");
                        return;
                    }
                    var acaoCobranca = BuscarAcaoCobranca(acoesCobrancaList, conbranca.NovoAtrasoBoleto);
                    var acaoRequest = CriarAtualizarAcaoRequest(boletoRelacao, acaoCobranca, DadosAPI);

                    boletoRelacao.DiasEmAtraso = conbranca.NovoAtrasoBoleto;
                    await EnviarBoletoParaCRM.AtualizarAcao(acaoRequest, DadosAPI.Token, dalRelBoletos, boletoRelacao, false);

                    await RemoverRegistro(conbranca.Id, false);
                    MetodosGerais.RegistrarLog("COBRANCA", $"Boleto {boletoRelacao.Id_DocumentoReceber} removido da lista de cobrança. CodOp: {conbranca.Cod_Oportunidade}");
                }


            }
            catch (NullReferenceException ex)
            {
                MetodosGerais.RegistrarLog("COBRANCA", $"Ocorreu um [ERROR] na consulta: {ex.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                MetodosGerais.RegistrarLog("COBRANCA", $"Ocorreu um [ERROR] na consulta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("COBRANCA", $"Ocorreu um [ERROR]: {ex.Message}");
                throw;
            }
        }

        private static AtualizarAcaoRequest CriarAtualizarAcaoRequest(RelacaoBoletoCRMModel boletoRelacao, BoletoAcoesCRMModel acaoCobracaBoleto, DadosAPIModels DadosAPI)
        {
            return new AtualizarAcaoRequest
            {
                codigoOportunidade = boletoRelacao.Cod_Oportunidade,
                codigoAcao = acaoCobracaBoleto.Codigo_Acao,
                codigoJornada = DadosAPI.Cod_Jornada_Boleto,
                textoFollowup = acaoCobracaBoleto.Mensagem_Atualizacao
            };
        }


        private static BoletoAcoesCRMModel? BuscarAcaoCobranca(List<BoletoAcoesCRMModel> acoesCobrancaList, int DiaCobranca)
        {
            return acoesCobrancaList.FirstOrDefault(x => x.Dias_Cobrancas.Equals(DiaCobranca));
        }

        private async static Task AtualizarBoletoNoCRM(RelacaoBoletoCRMModel boletoRelacao, AtualizarAcaoRequest atualizarAcaoRequest, DadosAPIModels dadosAPI)
        {
            using var dalBoleto = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext());

            // É passado o parametro "foiQuitado" como true para remover qualquer registro de aviso que esteja aguardando para envio
            await EnviarBoletoParaCRM.AtualizarAcao(atualizarAcaoRequest, dadosAPI.Token, dalBoleto, boletoRelacao, true);

        }

        internal async Task RemoverRegistro(int cobrancaId, bool apagarTodos)
        {
            try
            {
                var cobrancaList = new List<CobrancasNaSegundaModel>();

                if (apagarTodos)
                {
                    cobrancaList = (await dalCobrancas.RecuperarTodosPorAsync(x => x.BoletoId == cobrancaId).ConfigureAwait(false)).ToList();
                }
                else
                {
                    cobrancaList = (await dalCobrancas.RecuperarTodosPorAsync(x => x.Id == cobrancaId).ConfigureAwait(false)).ToList();
                }

                if (cobrancaList.Any())
                {
                    foreach (var cobranca in cobrancaList)
                    {
                        await dalCobrancas.DeletarAsync(cobranca).ConfigureAwait(false);
                        MetodosGerais.RegistrarLog("COBRANCA", $"Oportunidade {cobranca.Cod_Oportunidade} removido da lista de cobrança. Boleto Id: {cobranca.BoletoId}");
                    }
                }

            }
            catch (SqlException ex)
            {
                MetodosGerais.RegistrarLog("COBRANCA", $"Ocorreu um [ERROR] na consulta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("COBRANCA", $"Ocorreu um [ERROR]: {ex.Message}");
                throw;
            }
        }
    }
}
