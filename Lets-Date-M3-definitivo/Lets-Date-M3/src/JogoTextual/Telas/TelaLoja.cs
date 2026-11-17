
// A mensagem da compra aparece no topo da loja, que já volta pronta para a próxima compra.
// (O Comprar com pausa continua existindo porque o LeitorEscolhasEncontro usa.)
//to tentando explicar cada parte do código pra equipe entender -ass: lucas
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Loja do aplicativo: conversa extra, presentes e itens consumíveis.
    public class TelaLoja : ITelaJogo
    {
        // mensagem da última compra, mostrada quando a loja aparece de novo.
        private string? _avisoSucesso;
        private string? _avisoErro;

        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Loja Let's Date");

            if (_avisoSucesso != null) { c.Saida.Sucesso(_avisoSucesso); _avisoSucesso = null; }
            if (_avisoErro != null) { c.Saida.Erro(_avisoErro); _avisoErro = null; }

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
            c.Saida.Texto($" 1. Conversa extra — R$ {Constantes.PrecoConversaExtra}{statusConversa}");

            int numero = 2;
            foreach (var categoria in new[] { CategoriaItem.Presente, CategoriaItem.Consumivel })
            {
                c.Saida.Secao(categoria == CategoriaItem.Presente ? "Presentes" : "Itens especiais");
                foreach (var item in c.Catalogo.ItensDaCategoria(categoria))
                {
                    c.Saida.Texto($" {numero}. {DescreverItem(c, item)}");
                    itens.Add(item);
                    numero++;
                }
            }

            c.Saida.Texto();
            c.Saida.Texto(" 0. Voltar ao menu");

            int escolha = c.Entrada.LerOpcao(0, itens.Count + 1);
            if (escolha == 0) return Tela.MenuInicial;

            if (escolha == 1)
            {
                FluxoCompraConversa.Executar(c);
                return Tela.Loja;
            }

            ComprarSemPausa(c, itens[escolha - 2]); 
            return Tela.Loja;
        }

        /// Nome, preço (com desconto riscado quando houver), descrição e quantidade que o jogador já tem.
        public static string DescreverItem(ContextoJogo c, Item item)
        {
            int preco = c.RegrasLoja.Preco(item, c.Partida, c.Salvos);
            string textoPreco = preco < item.Preco ? $"R$ {preco} (de R$ {item.Preco})" : $"R$ {preco}";
            int tem = c.Partida.Quantidade(item.Id);
            string posse = tem > 0 ? $" [você tem {tem}]" : string.Empty;
            return $"{item.Nome} — {textoPreco}{posse}\n     {item.Descricao}";
        }

        /// Compra com pausa (usada na fase de encontro).
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

        ///  compra sem "Aperte ENTER". A mensagem aparece no topo da loja na próxima vez.
        private void ComprarSemPausa(ContextoJogo c, Item item)
        {
            int preco = c.RegrasLoja.Preco(item, c.Partida, c.Salvos);
            var resultado = c.RegrasLoja.Comprar(item.Id, c.Partida, c.Salvos);

            if (resultado == ResultadoCompra.Sucesso)
                _avisoSucesso = $"Você comprou {item.Nome} por R$ {preco}! Agora você tem {c.Partida.Quantidade(item.Id)}.";
            else
                _avisoErro = $"Saldo insuficiente! {item.Nome} custa R$ {preco} e você tem R$ {c.Partida.Dinheiro}.";
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
