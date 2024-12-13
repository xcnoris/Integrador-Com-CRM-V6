using Aplication.IntegradorCRM.Servicos.Boleto;
using DataBase.IntegradorCRM.Data;
using Metodos.IntegradorCRM.Metodos;
using Modelos.IntegradorCRM.Models.EF;
using Modelos.IntegradorCRM.Models.Enuns;
using Modelos.IntegradorCRMRM.Models;

namespace Aplication.IntegradorCRM.Metodos.Boleto
{
    public class ControleBoletos
    {
        private DAL<RelacaoBoletoCRMModel> dalBoleto;
        private readonly CrudBoleto _CrudBoleto;
        private readonly Boleto_Services BoletoServices;
        private readonly DAL<BoletoAcoesCRMModel> _dalBoletoAcoes;
        private readonly DAL<DadosAPIModels> _dalDadosAPI;
        private readonly Situacao_Boleto SituacaoBoleto = new Situacao_Boleto();

        public ControleBoletos()
        {
            dalBoleto = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext());
            _CrudBoleto = new CrudBoleto();
            _dalBoletoAcoes = new DAL<BoletoAcoesCRMModel>(new IntegradorDBContext());
            BoletoServices = new Boleto_Services();
            //metodosGeraisBoleto = new MetodosGeraisBoleto(FrmBoletoAcao);
        }

        public async Task VerificarNovosBoletos(DadosAPIModels DadosAPI, List<AcaoSituacao_Boleto_CRM> AcoesSituacaoBoleto, List<BoletoAcoesCRMModel> BoletoAcoesCRM)
        {
            try
            {
                MetodosGerais.RegistrarInicioLog("BOLETO");
                // Busca Novos boletos no DB
                List<RetornoBoleto> boletoList = _CrudBoleto.BuscarBoletosInDB();
                List<RelacaoBoletoCRMModel> TableRelacaoBoleto = (await dalBoleto.ListarAsync() ?? Enumerable.Empty<RelacaoBoletoCRMModel>()).ToList();

                string codigoJornada = DadosAPI.Cod_Jornada_Boleto;

                MetodosGerais.RegistrarLog("BOLETO", $"Foram encontrados {boletoList.Count} Boletos.\n");

                // Passa por cada Boleto que retornar no select
                foreach (var linha in boletoList)
                {
                    string id_DocReceber = linha.Id_DocReceber;
                    int situacao = Convert.ToInt32(linha.Situacao);

                    using (var dalBoletoUsing = new DAL<RelacaoBoletoCRMModel>(new IntegradorDBContext()))
                    {
                        // Verifica se a BOLETO já esta na tabela de relação, caso ele esteja, significa que já existe um cady/oportunidade criada no CRM
                        RelacaoBoletoCRMModel BoletoRelacao = TableRelacaoBoleto.FirstOrDefault(x => x.Id_DocumentoReceber == Convert.ToInt32(id_DocReceber));

                        // Log para verificação
                        MetodosGerais.RegistrarLog("BOLETO", $"Verificando Documento a receber {id_DocReceber}...");

                        if (BoletoRelacao is null)
                        {
                            // Verifica se o boleto não esta cancelado ou estornado
                            if ((Situacao_Boleto)situacao != Situacao_Boleto.Cancelada_Ou_Estornado)
                            {
                                // Log para verificação
                                MetodosGerais.RegistrarLog("BOLETO", $"Documento a receber {id_DocReceber} não encontrada na tabela de relação.");

                                ModeloOportunidadeRequest oportunidade = new ModeloOportunidadeRequest();
                                oportunidade = oportunidade.CriarOportunidade(linha, DadosAPI);
                               
                                BoletoRelacao = new RelacaoBoletoCRMModel();
                                BoletoRelacao = BoletoRelacao.InstanciaDados(linha);

                                // tenta criar a oportunidade no CRM
                                OportunidadeResponse response = await EnviarBoletoParaCRM.CriarOportunidade(oportunidade, DadosAPI.Token, dalBoletoUsing, BoletoRelacao);
                                if (response is null)
                                {
                                    MetodosGerais.RegistrarLog("ERRO", "Falha ao criar Oportunidade");
                                    continue;
                                }

                                // Verifica se o boleto já esta pago, caso esteja muda o boleto para fase Pago/Aguardando Liberação
                                BoletoServices.VerificarQuitacao(situacao, BoletoRelacao, AcoesSituacaoBoleto, codigoJornada, DadosAPI, true);
                            }
                            else
                            {
                                MetodosGerais.RegistrarLog("BOLETO", $"Documento Cancelado/Estornado. Não consta na Tabela relação nem CRM!");
                            }
                        }
                        else
                        {
                            int DiasEmAtrasoBoleto = BoletoRelacao.DiasEmAtraso;
                            int diasAtraso = (DateTime.Now - linha.Data_Vencimento).Days;
                            int situacaTBRelacao = BoletoRelacao.Situacao;

                            MetodosGerais.RegistrarLog("BOLETO", $"DR  {BoletoRelacao.Id_DocumentoReceber} esta com a relação do vencimento em {diasAtraso}. Verificação foi iniciada...");
                            BoletoServices.VerificarBoletosCriadosNoCRM(BoletoRelacao, diasAtraso, situacao, situacaTBRelacao, DadosAPI, AcoesSituacaoBoleto, BoletoAcoesCRM);
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                MetodosGerais.RegistrarLog("BOLETO", $"[ERROR NullReference]: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                MetodosGerais.RegistrarLog("BOLETO", $"[ERROR]: {ex.Message}");
                throw;
            }
            finally
            {
                MetodosGerais.RegistrarFinalLog("BOLETO");
            }
        }


    }
}
