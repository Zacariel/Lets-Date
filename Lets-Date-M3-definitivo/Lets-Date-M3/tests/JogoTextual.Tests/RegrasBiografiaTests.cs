using System.Collections.Generic;
using JogoTextual.Dominio;
using JogoTextual.Regras;
using Xunit;

namespace JogoTextual.Tests
{
    public class RegrasBiografiaTests
    {
        private static Npc NpcCom(params string[] palavras)
        {
            return new Npc { Id = "teste", PalavrasChave = new List<string>(palavras) };
        }

        [Fact]
        public void AchaPalavra_SemImportarMaiusculas()
        {
            Assert.Equal("cachorro", RegrasBiografia.PalavraEmComum("Amo CACHORROS e café", NpcCom("cachorro")));
        }

        [Fact]
        public void AchaPalavra_SemImportarAcentos()
        {
            Assert.Equal("música", RegrasBiografia.PalavraEmComum("curto musica alta", NpcCom("música")));
            Assert.Equal("veterinaria", RegrasBiografia.PalavraEmComum("Estudo Veterinária", NpcCom("veterinaria")));
        }

        [Fact]
        public void SemPalavraEmComum_DevolveNull()
        {
            Assert.Null(RegrasBiografia.PalavraEmComum("Gosto de dormir", NpcCom("praia", "surf")));
        }

        [Fact]
        public void BioVazia_DevolveNull()
        {
            Assert.Null(RegrasBiografia.PalavraEmComum("", NpcCom("praia")));
        }
    }
}
