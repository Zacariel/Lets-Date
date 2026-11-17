using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// <summary>Loja do aplicativo: conversa extra, presentes e itens consumíveis.</summary>
    public class TelaLoja : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Loja Let's Date");
            c.Saida.Texto($"Carteira: R$ {c.Partida.Dinheiro}");
            if (c.Partida.CupomDesconto > 0) c.Saida.Destaque($"Cupom ativo: {c.Partida.CupomDesconto}% de desconto na próxima compra.");
            MostrarDescontosConquistados(c);

            // Monta a lista numerada: 1 é sempre a conversa extra; depois presentes e consumíveis.
            var itens = new List<Item>();
            c.Saida.Secao("Aplicativo");
            string statusConversa = RegrasLoja.PodeComprarConversaExtra(c.Partida) switch
            {
                ResultadoCompraConversa.JaComprouHoje => " — já comprada hoje",
                ResultadoCompraConversa.LimiteAtingido => " — você já está no limite de conversas",
                _ => string.Empty
            };
            c.Saida.Texto($"  1. Conversa extra — R$ {Constantes.PrecoConversaExtra}{statusConversa}");

            int numero = 2;
            foreach (var categoria in new[] { CategoriaItem.Presente, CategoriaItem.Consumivel })
            {
                c.Saida.Secao(categoria == CategoriaItem.Presente ? "Presentes" : "Itens especiais");
                foreach (var item in c.Catalogo.ItensDaCategoria(categoria))
                {
                    c.Saida.Texto($"  {numero}. {DescreverItem(c, item)}");
                    itens.Add(item);
                    numero++;
                }
            }

            c.Saida.Texto();
            c.Saida.Texto("  0. Voltar ao menu");

            int escolha = c.Entrada.LerOpcao(0, itens.Count + 1);
            if (escolha == 0) return Tela.MenuInicial;

            if (escolha == 1)
            {
                FluxoCompraConversa.Executar(c);
                return Tela.Loja;
            }

            Comprar(c, itens[escolha - 2]);
            return Tela.Loja;
        }

        /// <summary>Nome, preço (com desconto riscado quando houver), descrição e quantidade que o jogador já tem.</summary>
        public static string DescreverItem(ContextoJogo c, Item item)
        {
            int preco = c.RegrasLoja.Preco(item, c.Partida, c.Salvos);
            string textoPreco = preco < item.Preco ? $"R$ {preco} (de R$ {item.Preco})" : $"R$ {preco}";
            int tem = c.Partida.Quantidade(item.Id);
            string posse = tem > 0 ? $" [você tem {tem}]" : string.Empty;
            return $"{item.Nome} — {textoPreco}{posse}\n       {item.Descricao}";
        }

        public static bool Comprar(ContextoJogo c, Item item)
        {
            int preco = c.RegrasLoja.Preco(item, c.Partida, c.Salvos);
            var resultado = c.RegrasLoja.Comprar(item.Id, c.Partida, c.Salvos);

            c.Saida.Texto();
            if (resultado == ResultadoCompra.Sucesso)
            {
                c.Saida.Sucesso($"Você comprou {item.Nome} por R$ {preco}! Carteira: R$ {c.Partida.Dinheiro}.");
            }
            else
            {
                c.Saida.Erro($"Saldo insuficiente! {item.Nome} custa R$ {preco} e você tem R$ {c.Partida.Dinheiro}.");
            }
            c.Entrada.Pausar();
            return resultado == ResultadoCompra.Sucesso;
        }

        private static void MostrarDescontosConquistados(ContextoJogo c)
        {
            var lojas = c.Salvos.DescontosDesbloqueados
                .Select(id => c.Catalogo.BuscarNpc(id))
                .Where(npc => npc?.Loja != null)
                .ToList();

            foreach (var npc in lojas)
            {
                c.Saida.Destaque($"Desconto de {npc!.Loja!.Desconto}% conquistado em \"{npc.Loja.Nome}\" ({npc.Nome}).");
            }
        }
    }
}
