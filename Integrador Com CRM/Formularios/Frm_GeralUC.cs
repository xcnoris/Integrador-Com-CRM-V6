
using Aplication.IntegradorCRM.Metodos.Boleto;
using Aplication.IntegradorCRM.Metodos.OS;
using Aplication.IntegradorCRM.Servicos.Boleto;
using DataBase.IntegradorCRM.Data;
using Metodos.IntegradorCRM.Metodos;
using Modelos.IntegradorCRM.Models.EF;

namespace Integrador_Com_CRM.Formularios
{
    public partial class Frm_GeralUC : UserControl
    {
        private readonly ControleOrdemDeServico controlOrdemServico;
        private readonly ControleBoletos controlBoletos;
        private readonly CobrancasNaSegundaModel cobrancas;
        private readonly DadosAPIModels? DadosAPI;
        private readonly DAL<AcaoSituacao_Boleto_CRM> _dalAcaoSitBoleto;
        private readonly DAL<DadosAPIModels> _dalDadosAPI;
        private readonly Frm_BoletoAcoesCRM_UC BoletoAcoes;
        DAL<BoletoAcoesCRMModel> _dalBoletoAcoes;

        public Frm_GeralUC(ControleOrdemDeServico controlOS, ControleBoletos controleBoletos, Frm_BoletoAcoesCRM_UC BoletosAcoes)
        {
            InitializeComponent();

            controlOrdemServico = controlOS;
            BoletoAcoes = BoletosAcoes;
            _dalBoletoAcoes = new DAL<BoletoAcoesCRMModel>(new IntegradorDBContext());
            _dalAcaoSitBoleto = new DAL<AcaoSituacao_Boleto_CRM>(new IntegradorDBContext());
            _dalDadosAPI = new DAL<DadosAPIModels>(new IntegradorDBContext());
            controlBoletos = new ControleBoletos();
            cobrancas = new CobrancasNaSegundaModel();
            DadosAPI = (_dalDadosAPI.Listar()).FirstOrDefault();
        }

        public async Task ExecutarBuscaOSAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                MetodosGerais.RegistrarLog("OS", $"\n---------- Ordens de serviço consultadas manualmente ------------\n");
                await controlOrdemServico.VerificarNovosServicos(DadosAPI);

                MessageBox.Show("Consulta de Ordem de Serviço Efetuada com sucesso", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível fazer a consulta. Mensagem: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MetodosGerais.RegistrarLog("OS", $"Error: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        public async Task ExecutarBuscarBoletoAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                MetodosGerais.RegistrarLog("BOLETO", $"\n----------------> Boletos consultados manualmente <-----------------\n");


                List<AcaoSituacao_Boleto_CRM> AcoesSituacaoBoleto = (await _dalAcaoSitBoleto.ListarAsync()).ToList();
                List<BoletoAcoesCRMModel> BoletoAcoesCRM = (await _dalBoletoAcoes.ListarAsync()).ToList();

                await controlBoletos.VerificarNovosBoletos(DadosAPI, AcoesSituacaoBoleto, BoletoAcoesCRM);

                MessageBox.Show("Consulta de Boletos Efetuada com sucesso", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possivel fazer a consulta. Mensagem: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MetodosGerais.RegistrarLog("BOLETO", $"Error :{ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        public async Task RealizarCobrancasBoletoAsync()
        {
            try
            {
                List<BoletoAcoesCRMModel?> acoesCobrancaList = (await _dalBoletoAcoes.ListarAsync()).ToList();
                List<DadosAPIModels?> DadosAPI = (await _dalDadosAPI.ListarAsync()).ToList();

                Cursor = Cursors.WaitCursor;
                CobrancaServicos CB = new CobrancaServicos();
                MetodosGerais.RegistrarLog("COBRANCA", $"\n------------------> Cobrança consultadas manualmente <-------------------\n");

                await CB.RealizarCobrancas(acoesCobrancaList, DadosAPI.First());

                MessageBox.Show("Cobranças de Boletos Efetuada com sucesso", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possivel fazer a consulta. Mensagem: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MetodosGerais.RegistrarLog("COBRANCA", $"Error :{ex.Message}");
            }
            finally
            {
                MetodosGerais.RegistrarFinalLog("COBRANCA");
                Cursor = Cursors.Default;
            }
        }

        private async void Btn_BuscarOS_Click(object sender, EventArgs e)
        {
            await ExecutarBuscaOSAsync();
        }

        private async void Btn_BuscarBoletos_Click(object sender, EventArgs e)
        {
            await ExecutarBuscarBoletoAsync();
        }

        private async void Btn_RealizarCobrancas_Click(object sender, EventArgs e)
        {
            await RealizarCobrancasBoletoAsync();
        }
    }
}
