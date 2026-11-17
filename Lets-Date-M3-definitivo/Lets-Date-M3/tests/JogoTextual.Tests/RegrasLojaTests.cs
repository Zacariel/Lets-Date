using JogoTextual.Dominio;
using JogoTextual.Regras;
using Xunit;

namespace JogoTextual.Tests
{
    public class RegrasLojaTests
    {
        private static readonly DadosCompra Cartao = new DadosCompra { Titular = "Fulano", ApelidoCartao = "Cartão do amor" };

        [Fact]
        public void ConversaExtra_Compra_AdicionaUmaConversaECobra5()
        {
            var partida = new EstadoPartida();
            var salvos = CatalogoTeste.SalvosComPerfil();

            var resultado = RegrasLoja.ComprarConversaExtra(partida, salvos, "Lucas", true, Cartao);

            Assert.Equal(ResultadoCompraConversa.Sucesso, resultado);
            Assert.Equal(4, partida.Conversas);
            Assert.Equal(Constantes.DinheiroInicial - 5, partida.Dinheiro);
            Assert.True(partida.ConversaExtraComprada);
            Assert.Equal("Cartão do amor", salvos.DadosCompra!.ApelidoCartao);
        }

        // Cenário do feedback da M2: comprar uma conversa, usá-la e tentar comprar outra.
        [Fact]
        public void ConversaExtra_DepoisDeUsar_NaoPodeComprarDeNovoNoMesmoDia()
        {
            var partida = new EstadoPartida();
            var salvos = CatalogoTeste.SalvosComPerfil();

            RegrasLoja.ComprarConversaExtra(partida, salvos, "Lucas", true, Cartao);
            partida.Conversas--; // gastou uma conversa: saldo volta a 3

            var segunda = RegrasLoja.ComprarConversaExtra(partida, salvos, "Lucas", true, null);

            Assert.Equal(ResultadoCompraConversa.JaComprouHoje, segunda);
            Assert.Equal(3, partida.Conversas);
        }

        [Fact]
        public void ConversaExtra_NoLimiteDe4_EBloqueada()
        {
            var partida = new EstadoPartida { Conversas = 4 };
            var resultado = RegrasLoja.ComprarConversaExtra(partida, CatalogoTeste.SalvosComPerfil(), "Lucas", true, Cartao);

            Assert.Equal(ResultadoCompraConversa.LimiteAtingido, resultado);
            Assert.Equal(4, partida.Conversas);
        }

        [Fact]
        public void ConversaExtra_NomeDiferenteDoCadastro_NaoCompra()
        {
            var partida = new EstadoPartida();
            var resultado = RegrasLoja.ComprarConversaExtra(partida, CatalogoTeste.SalvosComPerfil(), "Outro nome", true, Cartao);

            Assert.Equal(ResultadoCompraConversa.NomeIncorreto, resultado);
            Assert.Equal(3, partida.Conversas);
            Assert.False(partida.ConversaExtraComprada);
        }

        [Fact]
        public void ConversaExtra_NomeComEspacosEMaiusculas_Confere()
        {
            Assert.True(RegrasLoja.NomeConfere(new PerfilJogador { Nome = "Lucas" }, "  LUCAS "));
        }

        [Fact]
        public void ConversaExtra_RespostaNao_CancelaSemCobrar()
        {
            var partida = new EstadoPartida();
            var resultado = RegrasLoja.ComprarConversaExtra(partida, CatalogoTeste.SalvosComPerfil(), "Lucas", false, Cartao);

            Assert.Equal(ResultadoCompraConversa.Cancelada, resultado);
            Assert.Equal(Constantes.DinheiroInicial, partida.Dinheiro);
            Assert.False(partida.ConversaExtraComprada);
        }

        [Fact]
        public void ConversaExtra_SemDinheiro_NaoCompra()
        {
            var partida = new EstadoPartida { Dinheiro = 4 };
            Assert.Equal(ResultadoCompraConversa.SemDinheiro, RegrasLoja.PodeComprarConversaExtra(partida));
        }

        [Fact]
        public void ConversaExtra_DadosDoCartaoSaoGuardadosSoNaPrimeiraCompra()
        {
            var salvos = CatalogoTeste.SalvosComPerfil();
            salvos.DadosCompra = Cartao;

            RegrasLoja.ComprarConversaExtra(new EstadoPartida(), salvos, "Lucas", true,
                new DadosCompra { Titular = "Outro", ApelidoCartao = "Outro cartão" });

            Assert.Equal("Cartão do amor", salvos.DadosCompra.ApelidoCartao);
        }

        [Fact]
        public void Comprar_DescontaDaCarteiraEColocaNoInventario()
        {
            var loja = new RegrasLoja(CatalogoTeste.Criar());
            var partida = new EstadoPartida { Dinheiro = 30 };

            var resultado = loja.Comprar("livro", partida, CatalogoTeste.SalvosComPerfil());

            Assert.Equal(ResultadoCompra.Sucesso, resultado);
            Assert.Equal(10, partida.Dinheiro);
            Assert.Equal(1, partida.Quantidade("livro"));
        }

        [Fact]
        public void Comprar_SemDinheiro_NaoAlteraNada()
        {
            var loja = new RegrasLoja(CatalogoTeste.Criar());
            var partida = new EstadoPartida { Dinheiro = 5 };

            Assert.Equal(ResultadoCompra.SemDinheiro, loja.Comprar("livro", partida, CatalogoTeste.SalvosComPerfil()));
            Assert.Equal(5, partida.Dinheiro);
            Assert.Equal(0, partida.Quantidade("livro"));
        }

        [Fact]
        public void Preco_AplicaDescontoDaLojaConquistadaEDepoisOCupom()
        {
            var catalogo = CatalogoTeste.Criar();
            var loja = new RegrasLoja(catalogo);
            var salvos = CatalogoTeste.SalvosComPerfil();
            salvos.DescontosDesbloqueados.Add("ana"); // Livraria da Ana: 50% em livro
            var partida = new EstadoPartida { CupomDesconto = 20 };

            Assert.Equal(8, loja.Preco(catalogo.BuscarItem("livro")!, partida, salvos));     // 20 -> 10 -> 8
            Assert.Equal(10, loja.Preco(catalogo.BuscarItem("chocolates")!, partida, salvos)); // só o cupom: 12 -> 10
        }

        [Fact]
        public void Cupom_ValeParaUmaCompraSo()
        {
            var loja = new RegrasLoja(CatalogoTeste.Criar());
            var partida = new EstadoPartida { CupomDesconto = 20 };

            loja.Comprar("chocolates", partida, CatalogoTeste.SalvosComPerfil());

            Assert.Equal(0, partida.CupomDesconto);
        }
    }
}
