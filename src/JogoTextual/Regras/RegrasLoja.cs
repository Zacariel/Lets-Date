using System;
using JogoTextual.Dados;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// <summary>Preços, descontos e compras da loja, inclusive a conversa extra.</summary>
    public class RegrasLoja
    {
        private readonly CatalogoJogo _catalogo;

        public RegrasLoja(CatalogoJogo catalogo)
        {
            _catalogo = catalogo;
        }

        /// <summary>Maior desconto permanente liberado para este item (lojas de NPCs que aceitaram um encontro).</summary>
        public int DescontoPermanente(string itemId, DadosSalvos salvos)
        {
            int maior = 0;
            foreach (var npcId in salvos.DescontosDesbloqueados)
            {
                var loja = _catalogo.BuscarNpc(npcId)?.Loja;
                if (loja != null && loja.Itens.Contains(itemId))
                {
                    maior = Math.Max(maior, loja.Desconto);
                }
            }
            return maior;
        }

        /// <summary>Preço final: aplica o desconto permanente e depois o cupom do dia.</summary>
        public int Preco(Item item, EstadoPartida partida, DadosSalvos salvos)
        {
            int preco = AplicarDesconto(item.Preco, DescontoPermanente(item.Id, salvos));
            return AplicarDesconto(preco, partida.CupomDesconto);
        }

        public static int AplicarDesconto(int preco, int percentual)
        {
            if (percentual <= 0) return preco;
            return preco - (preco * Math.Min(percentual, 100) / 100);
        }

        public ResultadoCompra Comprar(string itemId, EstadoPartida partida, DadosSalvos salvos)
        {
            Item? item = _catalogo.BuscarItem(itemId);
            if (item == null) return ResultadoCompra.ItemInexistente;

            int preco = Preco(item, partida, salvos);
            if (partida.Dinheiro < preco) return ResultadoCompra.SemDinheiro;

            partida.Dinheiro -= preco;
            partida.AdicionarItem(item.Id);
            partida.CupomDesconto = 0; // o cupom vale para uma compra
            return ResultadoCompra.Sucesso;
        }

        /// <summary>
        /// Confere se a conversa extra pode ser comprada agora, antes de pedir os dados ao jogador.
        /// Regra: uma única conversa extra por dia, sem passar de 4 conversas e com dinheiro suficiente.
        /// </summary>
        public static ResultadoCompraConversa PodeComprarConversaExtra(EstadoPartida partida)
        {
            if (partida.ConversaExtraComprada) return ResultadoCompraConversa.JaComprouHoje;
            if (partida.Conversas >= Constantes.ConversasMaximas) return ResultadoCompraConversa.LimiteAtingido;
            if (partida.Dinheiro < Constantes.PrecoConversaExtra) return ResultadoCompraConversa.SemDinheiro;
            return ResultadoCompraConversa.Sucesso;
        }

        public static bool NomeConfere(PerfilJogador perfil, string nomeDigitado)
        {
            return string.Equals(perfil.Nome.Trim(), nomeDigitado.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Finaliza a compra da conversa extra. O registro <see cref="EstadoPartida.ConversaExtraComprada"/>
        /// impede comprar de novo no mesmo dia, mesmo depois de gastar a conversa.
        /// </summary>
        public static ResultadoCompraConversa ComprarConversaExtra(
            EstadoPartida partida, DadosSalvos salvos, string nomeConfirmacao, bool confirmou, DadosCompra? dadosNovos)
        {
            var verificacao = PodeComprarConversaExtra(partida);
            if (verificacao != ResultadoCompraConversa.Sucesso) return verificacao;

            if (salvos.Perfil == null || !NomeConfere(salvos.Perfil, nomeConfirmacao))
                return ResultadoCompraConversa.NomeIncorreto;

            if (!confirmou) return ResultadoCompraConversa.Cancelada;

            partida.Dinheiro -= Constantes.PrecoConversaExtra;
            partida.Conversas++;
            partida.ConversaExtraComprada = true;
            salvos.DadosCompra ??= dadosNovos;
            return ResultadoCompraConversa.Sucesso;
        }
    }
}
