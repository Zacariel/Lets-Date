using JogoTextual.Dominio;
using JogoTextual.Regras;
using Xunit;

namespace JogoTextual.Tests
{
    public class RegrasEncontroTests
    {
        private static Npc Ana => CatalogoTeste.Criar().Npcs[0]; // elegante, livro, praia

        [Fact]
        public void TudoCerto_UmEncontro_Da100()
        {
            var r = RegrasEncontro.Calcular(Ana, "elegante", "livro", "praia", totalEncontros: 1);
            Assert.Equal(100, r.Satisfacao);
            Assert.Equal(AvaliacaoEncontro.Adorou, r.Avaliacao);
        }

        [Fact]
        public void TudoErrado_UmEncontro_Da40()
        {
            var r = RegrasEncontro.Calcular(Ana, "casual", "chocolates", "parque", totalEncontros: 1);
            Assert.Equal(40, r.Satisfacao);
            Assert.Equal(AvaliacaoEncontro.NaoGostou, r.Avaliacao);
        }

        [Fact]
        public void SemPresente_ContaComoPresenteErrado()
        {
            var r = RegrasEncontro.Calcular(Ana, "elegante", null, "praia", totalEncontros: 1);
            Assert.False(r.PresenteCerto);
            Assert.Equal(80, r.Satisfacao);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 10)]
        [InlineData(3, 20)]
        [InlineData(4, 30)]
        public void Penalidade_Menos10PorEncontroAlemDoPrimeiro(int encontros, int penalidade)
        {
            Assert.Equal(penalidade, RegrasEncontro.Penalidade(encontros));
        }

        [Fact]
        public void Bonus_DoPerfume_ESomadoENaoPassaDe100()
        {
            Assert.Equal(100, RegrasEncontro.Calcular(Ana, "elegante", "livro", "praia", 1, bonus: 10).Satisfacao);
            Assert.Equal(90, RegrasEncontro.Calcular(Ana, "elegante", "livro", "praia", 3, bonus: 10).Satisfacao); // 70 + 30 - 20 + 10
        }

        [Theory]
        [InlineData(100, AvaliacaoEncontro.Adorou)]
        [InlineData(81, AvaliacaoEncontro.Adorou)]
        [InlineData(80, AvaliacaoEncontro.Gostou)]
        [InlineData(61, AvaliacaoEncontro.Gostou)]
        [InlineData(60, AvaliacaoEncontro.Normal)]
        [InlineData(41, AvaliacaoEncontro.Normal)]
        [InlineData(40, AvaliacaoEncontro.NaoGostou)]
        [InlineData(21, AvaliacaoEncontro.NaoGostou)]
        [InlineData(20, AvaliacaoEncontro.Odiou)]
        [InlineData(0, AvaliacaoEncontro.Odiou)]
        public void Classificar_SegueAsFaixasDoDocumento(int satisfacao, AvaliacaoEncontro esperado)
        {
            Assert.Equal(esperado, RegrasEncontro.Classificar(satisfacao));
        }
    }

    public class RegrasFinaisTests
    {
        [Theory]
        [InlineData(0, 0, FinalJogo.MelhorNemTentar)]
        [InlineData(2, 0, FinalJogo.NaoFoiDessaVez)]
        [InlineData(1, 1, FinalJogo.AlmasGemeas)]
        [InlineData(2, 2, FinalJogo.CalmaAmor)]
        [InlineData(3, 3, FinalJogo.SaoMuitasNamoradas)]
        [InlineData(4, 4, FinalJogo.NaoDeuContaDoRecado)]
        public void Determinar_PelaQuantidadeDeEncontros(int conversas, int encontros, FinalJogo esperado)
        {
            Assert.Equal(esperado, RegrasFinais.Determinar(conversas, encontros));
        }

        [Fact]
        public void Registrar_GuardaOFinalUmaVezEZeraConversasGuardadas()
        {
            var salvos = new DadosSalvos { ConversasGuardadas = 2 };
            var partida = new EstadoPartida { ConversasIniciadas = 1 };

            RegrasFinais.Registrar(salvos, partida);
            RegrasFinais.Registrar(salvos, partida);

            Assert.Single(salvos.FinaisDesbloqueados);
            Assert.Equal(FinalJogo.NaoFoiDessaVez, partida.FinalAlcancado);
            Assert.Equal(0, salvos.ConversasGuardadas);
        }
    }

    public class RegrasDiaTests
    {
        [Theory]
        [InlineData(0, 3)]
        [InlineData(1, 4)]
        [InlineData(3, 4)]
        public void NovoDia_SomaConversasGuardadasAte4(int guardadas, int esperado)
        {
            var salvos = new DadosSalvos { ConversasGuardadas = guardadas };

            var partida = RegrasDia.NovoDia(salvos);

            Assert.Equal(esperado, partida.Conversas);
            Assert.Equal(0, salvos.ConversasGuardadas);
            Assert.Equal(1, salvos.DiasJogados);
        }

        [Fact]
        public void NovoDia_RecomecaCoragemCarteiraEEncontros()
        {
            var partida = RegrasDia.NovoDia(new DadosSalvos());
            Assert.Equal(Constantes.CoragemMaxima, partida.Coragem);
            Assert.Equal(Constantes.DinheiroInicial, partida.Dinheiro);
            Assert.Empty(partida.EncontrosMarcados);
            Assert.False(partida.ConversaExtraComprada);
        }
    }

    public class RegrasItensTests
    {
        [Fact]
        public void Energetico_RecuperaCoragemAte100()
        {
            var itens = new RegrasItens(CatalogoTeste.Criar());
            var partida = new EstadoPartida { Coragem = 75 };
            partida.AdicionarItem("energetico");

            Assert.Equal(ResultadoUsoItem.Sucesso, itens.Usar("energetico", partida, MomentoUso.Menu, out _));
            Assert.Equal(100, partida.Coragem);
            Assert.Equal(0, partida.Quantidade("energetico"));
        }

        [Fact]
        public void Energetico_ComCoragemCheia_NaoEGasto()
        {
            var itens = new RegrasItens(CatalogoTeste.Criar());
            var partida = new EstadoPartida();
            partida.AdicionarItem("energetico");

            Assert.Equal(ResultadoUsoItem.SemEfeito, itens.Usar("energetico", partida, MomentoUso.Menu, out _));
            Assert.Equal(1, partida.Quantidade("energetico"));
        }

        [Fact]
        public void SuperLike_UmaVezPorConversa()
        {
            var itens = new RegrasItens(CatalogoTeste.Criar());
            var partida = CatalogoTeste.PartidaConversando("ana", 20);
            partida.AdicionarItem("super-like", 2);

            Assert.Equal(ResultadoUsoItem.Sucesso, itens.Usar("super-like", partida, MomentoUso.Conversa, out _));
            Assert.Equal(ResultadoUsoItem.JaUsado, itens.Usar("super-like", partida, MomentoUso.Conversa, out _));
            Assert.Equal(35, partida.ConversaAtual!.Interesse);
            Assert.Equal(1, partida.Quantidade("super-like"));
        }

        [Fact]
        public void Perfume_SoFuncionaNoEncontro()
        {
            var itens = new RegrasItens(CatalogoTeste.Criar());
            var partida = new EstadoPartida();
            partida.AdicionarItem("perfume");

            Assert.Equal(ResultadoUsoItem.NaoUsavelAgora, itens.Usar("perfume", partida, MomentoUso.Menu, out _));
            Assert.Equal(ResultadoUsoItem.Sucesso, itens.Usar("perfume", partida, MomentoUso.Encontro, out int bonus));
            Assert.Equal(10, bonus);
        }

        [Fact]
        public void ItemQueNaoTem_NaoPodeSerUsado()
        {
            var itens = new RegrasItens(CatalogoTeste.Criar());
            Assert.Equal(ResultadoUsoItem.NaoPossui, itens.Usar("energetico", new EstadoPartida(), MomentoUso.Menu, out _));
        }
    }
}
