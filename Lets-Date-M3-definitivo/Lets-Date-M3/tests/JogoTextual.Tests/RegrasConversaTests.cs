using JogoTextual.Dominio;
using JogoTextual.Regras;
using Xunit;

namespace JogoTextual.Tests
{
    public class RegrasConversaTests
    {
        [Theory]
        [InlineData(Qualidade.Bom, 60)]
        [InlineData(Qualidade.Regular, 40)]
        [InlineData(Qualidade.Ruim, 10)]
        [InlineData(Qualidade.Pessimo, -10)]
        public void Responder_AplicaAVariacaoDaQualidade(Qualidade qualidade, int interesseEsperado)
        {
            var catalogo = CatalogoTeste.Criar();
            var regras = new RegrasConversa(catalogo, new AleatorioFixo());
            var partida = CatalogoTeste.PartidaConversando("ana", 20);

            regras.Responder(partida, catalogo.Npcs[0], new OpcaoDialogo { Qualidade = qualidade });

            Assert.Equal(interesseEsperado, partida.ConversaAtual!.Interesse);
            Assert.Equal(1, partida.ConversaAtual.ProximoTopico);
        }

        [Fact]
        public void Responder_ComDica_AnotaAPreferencia()
        {
            var catalogo = CatalogoTeste.Criar();
            var regras = new RegrasConversa(catalogo, new AleatorioFixo());
            var partida = CatalogoTeste.PartidaConversando("ana", 20);

            regras.Responder(partida, catalogo.Npcs[0], catalogo.Npcs[0].Topicos[0].Opcoes[0]);

            Assert.Contains("Ana ama a praia.", partida.Anotacoes["ana"]);
        }

        [Fact]
        public void Interesse_FicaEntreMenos100E100()
        {
            var conversa = new EstadoConversa { Interesse = 90 };
            RegrasConversa.AlterarInteresse(conversa, 40);
            Assert.Equal(100, conversa.Interesse);

            conversa.Interesse = -90;
            RegrasConversa.AlterarInteresse(conversa, -60);
            Assert.Equal(-100, conversa.Interesse);
        }

        [Theory]
        [InlineData(4, -60)]
        [InlineData(10, 0)]
        [InlineData(13, 30)]
        [InlineData(20, 100)]
        public void Flertar_UsaAFormulaDoD20EGasta25DeCoragem(int dado, int variacaoEsperada)
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo(dado, 0));
            var partida = CatalogoTeste.PartidaConversando("ana", 0);

            var resultado = regras.Flertar(partida);

            Assert.True(resultado.Permitido);
            Assert.Equal(variacaoEsperada, resultado.Variacao);
            Assert.Equal(variacaoEsperada, partida.ConversaAtual!.Interesse);
            Assert.Equal(75, partida.Coragem);
        }

        [Fact]
        public void Flertar_SemCoragem_EBloqueadoENadaMuda()
        {
            var aleatorio = new AleatorioFixo(20);
            var regras = new RegrasConversa(CatalogoTeste.Criar(), aleatorio);
            var partida = CatalogoTeste.PartidaConversando("ana", 20);
            partida.Coragem = 0;

            var resultado = regras.Flertar(partida);

            Assert.False(resultado.Permitido);
            Assert.Equal(0, partida.Coragem);
            Assert.Equal(20, partida.ConversaAtual!.Interesse);
            Assert.Equal(0, aleatorio.Usados);
        }

        // Cenário do feedback da M2: convidar com interesse 59, 60 e 100.
        [Fact]
        public void Convidar_Com59_RecusaSemSortear()
        {
            var aleatorio = new AleatorioFixo(1);
            var regras = new RegrasConversa(CatalogoTeste.Criar(), aleatorio);
            var partida = CatalogoTeste.PartidaConversando("beto", 59);

            var resultado = regras.Convidar(partida, CatalogoTeste.SalvosComPerfil());

            Assert.Equal(MotivoConvite.InteresseBaixo, resultado.Motivo);
            Assert.Equal(0, aleatorio.Usados);
            Assert.Empty(partida.EncontrosMarcados);
        }

        [Theory]
        [InlineData(60, 60, true)]
        [InlineData(60, 61, false)]
        [InlineData(100, 100, true)]
        [InlineData(85, 86, false)]
        public void Convidar_APartirDe60_SorteiaDe1a100(int interesse, int sorteio, bool aceitaEsperado)
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo(sorteio));
            var partida = CatalogoTeste.PartidaConversando("beto", interesse);

            var resultado = regras.Convidar(partida, CatalogoTeste.SalvosComPerfil());

            Assert.Equal(aceitaEsperado, resultado.Aceito);
            Assert.Equal(sorteio, resultado.Sorteio);
            Assert.Equal(aceitaEsperado ? 1 : 0, partida.EncontrosMarcados.Count);
        }

        [Fact]
        public void Convidar_SempreEncerraAConversa()
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo(100));
            var partida = CatalogoTeste.PartidaConversando("beto", 60);

            regras.Convidar(partida, CatalogoTeste.SalvosComPerfil());

            Assert.Null(partida.ConversaAtual);
        }

        [Fact]
        public void Convidar_DonoDeLoja_LiberaDescontoUmaVez()
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo(1, 1));
            var salvos = CatalogoTeste.SalvosComPerfil();

            var primeiro = regras.Convidar(CatalogoTeste.PartidaConversando("ana", 90), salvos);
            var segundo = regras.Convidar(CatalogoTeste.PartidaConversando("ana", 90), salvos);

            Assert.NotNull(primeiro.DescontoLiberado);
            Assert.Null(segundo.DescontoLiberado);
            Assert.Single(salvos.DescontosDesbloqueados);
        }

        [Fact]
        public void InteresseMenos60_BloqueiaAutomaticamente()
        {
            Assert.True(RegrasConversa.FoiBloqueadoPeloNpc(new EstadoConversa { Interesse = -60 }));
            Assert.False(RegrasConversa.FoiBloqueadoPeloNpc(new EstadoConversa { Interesse = -50 }));
        }

        [Fact]
        public void IniciarConversa_GastaUmaConversaENaoRepeteNpc()
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo(0, 0));
            var perfil = CatalogoTeste.SalvosComPerfil().Perfil!;
            var partida = new EstadoPartida();

            var primeiro = regras.IniciarConversa(partida, perfil);
            var segundo = regras.IniciarConversa(partida, perfil);

            Assert.NotNull(primeiro);
            Assert.NotNull(segundo);
            Assert.NotEqual(primeiro!.Id, segundo!.Id);
            Assert.Equal(1, partida.Conversas);
            Assert.Equal(2, partida.ConversasIniciadas);
            Assert.Null(regras.IniciarConversa(partida, perfil)); // acabaram os NPCs
            Assert.Equal(1, partida.Conversas);                  // e nada foi gasto ;)
        }

        [Fact]
        public void IniciarConversa_SemConversas_NaoComeca()
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo());
            var partida = new EstadoPartida { Conversas = 0 };

            Assert.Null(regras.IniciarConversa(partida, CatalogoTeste.SalvosComPerfil().Perfil!));
            Assert.Equal(0, partida.ConversasIniciadas);
        }

        [Theory]
        [InlineData(Preferencia.Homem, "beto")]
        [InlineData(Preferencia.Mulher, "ana")]
        public void IniciarConversa_RespeitaOQueOJogadorBusca(Preferencia busca, string npcEsperado)
        {
            var regras = new RegrasConversa(CatalogoTeste.Criar(), new AleatorioFixo(0));
            var npc = regras.IniciarConversa(new EstadoPartida(), CatalogoTeste.SalvosComPerfil(busca).Perfil!);
            Assert.Equal(npcEsperado, npc!.Id);
        }
    }
}
