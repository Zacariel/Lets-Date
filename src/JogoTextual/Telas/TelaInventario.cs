using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// <summary>Inventário: mostra os itens comprados e permite usar os que funcionam fora das conversas.</summary>
    public class TelaInventario : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Inventário");

            var itens = c.Partida.Inventario.Keys
                .Select(id => c.Catalogo.BuscarItem(id))
                .Where(item => item != null)
                .Cast<Item>()
                .OrderBy(item => item.Categoria)
                .ThenBy(item => item.Nome)
                .ToList();

            if (itens.Count == 0)
            {
                c.Saida.Texto("Seu inventário está vazio. Presentes e itens especiais são comprados na Loja.");
                c.Entrada.Pausar();
                return Tela.MenuInicial;
            }

            for (int i = 0; i < itens.Count; i++)
            {
                var item = itens[i];
                string ondeUsar = item.Categoria == CategoriaItem.Presente
                    ? "presente: entregue na fase de encontro"
                    : item.Efeito switch
                    {
                        TipoEfeito.Coragem => "pode usar agora ou durante uma conversa",
                        TipoEfeito.Interesse => "use durante uma conversa",
                        _ => "use na fase de encontro"
                    };
                c.Saida.Texto($"  {i + 1}. {item.Nome} x{c.Partida.Quantidade(item.Id)} — {ondeUsar}");
            }
            c.Saida.Texto("  0. Voltar");

            c.Saida.Texto();
            int escolha = c.Entrada.LerOpcao(0, itens.Count, "Usar qual item? ");
            if (escolha == 0) return Tela.MenuInicial;

            var escolhido = itens[escolha - 1];
            var resultado = c.RegrasItens.Usar(escolhido.Id, c.Partida, MomentoUso.Menu, out _);
            MostrarResultadoUso(c, escolhido, resultado);
            c.Entrada.Pausar();
            return Tela.Inventario;
        }

        public static void MostrarResultadoUso(ContextoJogo c, Item item, ResultadoUsoItem resultado)
        {
            switch (resultado)
            {
                case ResultadoUsoItem.Sucesso:
                    string efeito = item.Efeito switch
                    {
                        TipoEfeito.Coragem => $"Coragem agora: {c.Partida.Coragem}.",
                        TipoEfeito.Interesse => $"+{item.ValorEfeito} de interesse!",
                        _ => $"+{item.ValorEfeito} de satisfação neste encontro!"
                    };
                    c.Saida.Sucesso($"Você usou {item.Nome}. {efeito}");
                    break;
                case ResultadoUsoItem.JaUsado:
                    c.Saida.Aviso($"Você já usou {item.Nome} nesta conversa. Exagerar pega mal!");
                    break;
                case ResultadoUsoItem.SemEfeito:
                    c.Saida.Aviso("Sua coragem já está no máximo. Guarde o item para depois!");
                    break;
                case ResultadoUsoItem.NaoUsavelAgora:
                    c.Saida.Aviso(item.Categoria == CategoriaItem.Presente
                        ? "Presentes são entregues na fase de encontro."
                        : "Esse item não pode ser usado agora.");
                    break;
                default:
                    c.Saida.Erro("Você não tem esse item.");
                    break;
            }
        }
    }
}
