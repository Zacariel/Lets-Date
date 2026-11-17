using System.IO;
using System.Linq;
using JogoTextual.Dados;
using JogoTextual.Dominio;
using JogoTextual.Interface;
using JogoTextual.Persistencia;
using Xunit;

namespace JogoTextual.Tests
{
    public class EntradaConsoleTests
    {
        private static (EntradaConsole entrada, StringWriter saida) Criar(params string[] linhas)
        {
            var saida = new StringWriter();
            var entrada = new EntradaConsole(new SaidaConsole(saida), new StringReader(string.Join("\n", linhas) + "\n"));
            return (entrada, saida);
        }

        // Cenário do feedback da M2: idade não numérica.
        [Fact]
        public void LerInteiro_TextoNaoNumerico_PedeDeNovo()
        {
            var (entrada, saida) = Criar("abc", "", "25");

            int idade = entrada.LerInteiro("Idade: ", "Digite sua idade usando apenas números inteiros.");

            Assert.Equal(25, idade);
            Assert.Equal(2, Contar(saida.ToString(), "apenas números inteiros"));
        }

        // Cenário do feedback da M2: opção de roupa inválida.
        [Fact]
        public void LerOpcao_ForaDaListaOuTexto_PedeDeNovoComAMensagemDoDocumento()
        {
            var (entrada, saida) = Criar("9", "x", "0", "3");

            int escolha = entrada.LerOpcao(1, 6);

            Assert.Equal(3, escolha);
            Assert.Equal(3, Contar(saida.ToString(), SaidaConsole.MensagemOpcaoInvalida));
        }

        [Fact]
        public void LerTexto_ObrigatorioEComLimite()
        {
            var (entrada, saida) = Criar("", new string('a', 101), "Oi!");

            Assert.Equal("Oi!", entrada.LerTexto("Bio: ", 100));
            Assert.Contains("não pode ficar vazio", saida.ToString());
            Assert.Contains("no máximo 100", saida.ToString());
        }

        [Theory]
        [InlineData("SIM", true)]
        [InlineData("s", true)]
        [InlineData("não", false)]
        [InlineData("NAO", false)]
        public void LerSimNao_AceitaVariacoes(string resposta, bool esperado)
        {
            var (entrada, _) = Criar(resposta);
            Assert.Equal(esperado, entrada.LerSimNao("Confirma?"));
        }

        [Fact]
        public void FimDaEntrada_LancaExcecaoEmVezDeTravar()
        {
            var (entrada, _) = Criar();
            Assert.Throws<FimDaEntradaException>(() =>
            {
                entrada.LerOpcao(1, 2);
                entrada.LerOpcao(1, 2);
            });
        }

        private static int Contar(string texto, string trecho) =>
            (texto.Length - texto.Replace(trecho, string.Empty).Length) / trecho.Length;
    }

    public class CatalogoTests
    {
        [Fact]
        public void ArquivosDoJogo_CarregamSemProblemas()
        {
            var catalogo = CarregadorCatalogo.Carregar(CatalogoTeste.PastaDadosReal());

            Assert.NotEmpty(catalogo.Npcs);
            Assert.NotEmpty(catalogo.Flertes);
            Assert.Equal(7, catalogo.Finais.Count);
        }

        [Fact]
        public void ArquivosDoJogo_TemNpcsSuficientesParaUmDiaInteiroComCadaBusca()
        {
            var catalogo = CarregadorCatalogo.Carregar(CatalogoTeste.PastaDadosReal());

            Assert.True(catalogo.NpcsCompativeis(Preferencia.Homem).Count >= 4);
            Assert.True(catalogo.NpcsCompativeis(Preferencia.Mulher).Count >= 4);
        }

        [Fact]
        public void Validar_DetectaPreferenciaInexistente()
        {
            var catalogo = CatalogoTeste.Criar();
            catalogo.Npcs[0].Preferencias.Presente = "diamante";

            var problemas = CarregadorCatalogo.Validar(catalogo);

            Assert.Contains(problemas, p => p.Contains("diamante"));
        }

        [Fact]
        public void Validar_DetectaIdRepetido()
        {
            var catalogo = CatalogoTeste.Criar();
            catalogo.Npcs[1].Id = "ana";

            Assert.Contains(CarregadorCatalogo.Validar(catalogo), p => p.Contains("\"ana\""));
        }
    }

    public class PersistenciaTests
    {
        [Fact]
        public void SalvarECarregar_MantemOsDados()
        {
            string caminho = Path.Combine(Path.GetTempPath(), $"letsdate_teste_{System.Guid.NewGuid()}.json");
            var persistencia = new PersistenciaJogo(caminho);
            var salvos = CatalogoTeste.SalvosComPerfil();
            salvos.ConversasGuardadas = 1;
            salvos.FinaisDesbloqueados.Add(FinalJogo.AlmasGemeas);

            persistencia.Salvar(salvos);
            var carregado = persistencia.Carregar(out bool corrompido);

            Assert.False(corrompido);
            Assert.Equal("Lucas", carregado.Perfil!.Nome);
            Assert.Equal(1, carregado.ConversasGuardadas);
            Assert.Equal(FinalJogo.AlmasGemeas, carregado.FinaisDesbloqueados.Single());
            File.Delete(caminho);
        }

        [Fact]
        public void ArquivoCorrompido_ComecaDoZeroEGuardaCopia()
        {
            string caminho = Path.Combine(Path.GetTempPath(), $"letsdate_teste_{System.Guid.NewGuid()}.json");
            File.WriteAllText(caminho, "{ isso não é json");
            var persistencia = new PersistenciaJogo(caminho);

            var carregado = persistencia.Carregar(out bool corrompido);

            Assert.True(corrompido);
            Assert.Null(carregado.Perfil);
            Assert.True(File.Exists(caminho + ".bak"));
            File.Delete(caminho);
            File.Delete(caminho + ".bak");
        }
    }
}
