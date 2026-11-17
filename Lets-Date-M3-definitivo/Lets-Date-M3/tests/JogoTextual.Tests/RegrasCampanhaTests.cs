using JogoTextual.Dominio;
using JogoTextual.Regras;
using Xunit;

namespace JogoTextual.Tests
{
    public class RegrasCampanhaTests
    {
        [Theory]
        [InlineData(Dificuldade.Facil, 15)]   // 14 dias antes + o Dia dos Namorados
        [InlineData(Dificuldade.Medio, 8)]    // 7 + 1
        [InlineData(Dificuldade.Dificil, 1)]  // só o dia
        public void NovaCampanha_TemOsDiasDaDificuldade(Dificuldade dificuldade, int diasTotais)
        {
            Campanha campanha = RegrasCampanha.Nova(dificuldade);

            Assert.Equal(diasTotais, campanha.DiasTotais);
            Assert.Equal(1, campanha.DiaAtual);
        }

        [Fact]
        public void Dificil_JaComecaNoDiaDosNamorados()
        {
            Assert.True(RegrasCampanha.Nova(Dificuldade.Dificil).EhDiaDosNamorados);
        }

        [Fact]
        public void Medio_ChegaNoDiaDosNamoradosDepoisDeSeteDias_ENaoPassaDele()
        {
            Campanha campanha = RegrasCampanha.Nova(Dificuldade.Medio);

            for (int i = 0; i < 7; i++)
            {
                Assert.False(campanha.EhDiaDosNamorados);
                Assert.True(RegrasCampanha.AvancarDia(campanha));
            }

            Assert.True(campanha.EhDiaDosNamorados);
            Assert.False(RegrasCampanha.AvancarDia(campanha)); // não existe dia depois do último
            Assert.Equal(8, campanha.DiaAtual);
        }

        [Fact]
        public void DescreverDia_MostraQuantoFalta()
        {
            Campanha campanha = RegrasCampanha.Nova(Dificuldade.Medio);
            Assert.Equal("Faltam 7 dias para o Dia dos Namorados.", RegrasCampanha.DescreverDia(campanha));

            campanha.DiaAtual = 7;
            Assert.Equal("Falta 1 dia para o Dia dos Namorados!", RegrasCampanha.DescreverDia(campanha));

            campanha.DiaAtual = 8;
            Assert.Equal("Hoje é o Dia dos Namorados!", RegrasCampanha.DescreverDia(campanha));
        }

        [Theory]
        [InlineData(Dificuldade.Facil, 75, "chance de aceitar: 75%")]
        [InlineData(Dificuldade.Facil, 59, "chance de aceitar: 0%")]
        [InlineData(Dificuldade.Medio, 59, "ainda não vai aceitar")]
        [InlineData(Dificuldade.Medio, 60, "pode aceitar")]
        [InlineData(Dificuldade.Dificil, 100, null)]
        public void DicaConvite_DependeDaDificuldade(Dificuldade dificuldade, int interesse, string? esperado)
        {
            Assert.Equal(esperado, RegrasCampanha.DicaConvite(dificuldade, interesse));
        }
    }
}
