using System.Collections.Generic;
using JogoTextual.Dominio;
using JogoTextual.Regras;
using Xunit;

namespace JogoTextual.Tests
{
    public class RegrasRelacionamentoTests
    {
        private static ResultadoEncontro Resultado(AvaliacaoEncontro avaliacao, int satisfacao = 80)
        {
            return new ResultadoEncontro { Avaliacao = avaliacao, Satisfacao = satisfacao };
        }

        [Fact]
        public void InteresseInicial_SobeComONivel()
        {
            Assert.Equal(20, RegrasRelacionamento.InteresseInicial(null));
            Assert.Equal(35, RegrasRelacionamento.InteresseInicial(new Relacionamento { Nivel = 1 }));
            Assert.Equal(50, RegrasRelacionamento.InteresseInicial(new Relacionamento { Nivel = 2 }));
            Assert.Equal(65, RegrasRelacionamento.InteresseInicial(new Relacionamento { Nivel = 3 }));
        }

        [Fact]
        public void PrimeiroEncontroBom_ViraMatch()
        {
            var campanha = RegrasCampanha.Nova(Dificuldade.Medio);

            var rel = RegrasRelacionamento.RegistrarEncontro(campanha, "maria", Resultado(AvaliacaoEncontro.Gostou), "flores", "praca");

            Assert.Equal(1, rel.Nivel);
            Assert.Equal(1, rel.EncontrosRealizados);
            Assert.Contains("maria", RegrasRelacionamento.MatchesAtivos(campanha));
        }

        [Fact]
        public void PrimeiroEncontroNormalOuRuim_NaoViraMatch()
        {
            var campanha = RegrasCampanha.Nova(Dificuldade.Medio);

            RegrasRelacionamento.RegistrarEncontro(campanha, "maria", Resultado(AvaliacaoEncontro.Normal, 50), null, "praca");
            RegrasRelacionamento.RegistrarEncontro(campanha, "davi", Resultado(AvaliacaoEncontro.Odiou, 10), null, "cinema");

            Assert.Empty(RegrasRelacionamento.MatchesAtivos(campanha));
        }

        [Fact]
        public void Nivel_NaoPassaDeTres_ECaiComEncontroRuim()
        {
            var campanha = RegrasCampanha.Nova(Dificuldade.Facil);
            Relacionamento rel = null!;

            for (int i = 0; i < 5; i++)
                rel = RegrasRelacionamento.RegistrarEncontro(campanha, "davi", Resultado(AvaliacaoEncontro.Adorou, 100), null, $"local{i}");
            Assert.Equal(3, rel.Nivel);

            RegrasRelacionamento.RegistrarEncontro(campanha, "davi", Resultado(AvaliacaoEncontro.NaoGostou, 30), null, "praia");
            Assert.Equal(2, rel.Nivel);
        }

        [Fact]
        public void RepetirPresenteELugar_PerdeCincoPontosCada()
        {
            var rel = new Relacionamento
            {
                PresentesDados = new List<string> { "flores" },
                LocaisVisitados = new List<string> { "praca" }
            };

            Assert.Equal(10, RegrasRelacionamento.PenalidadeRepeticao(rel, "flores", "praca"));
            Assert.Equal(5, RegrasRelacionamento.PenalidadeRepeticao(rel, "livro", "praca"));
            Assert.Equal(0, RegrasRelacionamento.PenalidadeRepeticao(rel, null, "praia"));
            Assert.Equal(0, RegrasRelacionamento.PenalidadeRepeticao(null, "flores", "praca"));
        }

        [Fact]
        public void Namoro_PrecisaDeNivelTresEEncontroBom()
        {
            Assert.True(RegrasRelacionamento.ConfirmaNamoro(new Relacionamento { Nivel = 3 }, Resultado(AvaliacaoEncontro.Gostou)));
            Assert.False(RegrasRelacionamento.ConfirmaNamoro(new Relacionamento { Nivel = 2 }, Resultado(AvaliacaoEncontro.Adorou)));
            Assert.False(RegrasRelacionamento.ConfirmaNamoro(new Relacionamento { Nivel = 3 }, Resultado(AvaliacaoEncontro.Normal, 50)));
        }

        [Fact]
        public void Final7_SoComNamoroEUmUnicoEncontro()
        {
            Assert.Equal(FinalJogo.FelizesParaSempre, RegrasFinais.Determinar(1, 1, namoroConfirmado: true));
            Assert.Equal(FinalJogo.CalmaAmor, RegrasFinais.Determinar(2, 2, namoroConfirmado: true)); // namorou e saiu com outra pessoa
            Assert.Equal(FinalJogo.AlmasGemeas, RegrasFinais.Determinar(1, 1));                     // regra antiga continua igual
        }

        [Fact]
        public void EncontroComMatch_ComecaComMaisInteresseENoCapituloCerto()
        {
            var partida = new EstadoPartida();
            var rel = new Relacionamento { NpcId = "maria", Nivel = 2 };

            Assert.True(RegrasConversa.IniciarConversaComMatch(partida, rel));
            Assert.Equal(50, partida.ConversaAtual!.Interesse);
            Assert.Equal(2, partida.ConversaAtual.Capitulo);
            Assert.Equal(Constantes.ConversasIniciais - 1, partida.Conversas);

            // Não dá pra falar com a mesma pessoa duas vezes no mesmo dia
            Assert.False(RegrasConversa.IniciarConversaComMatch(partida, rel));
        }
    }
}
