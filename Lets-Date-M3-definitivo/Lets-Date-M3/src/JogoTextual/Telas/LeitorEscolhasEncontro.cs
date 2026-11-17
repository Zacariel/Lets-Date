using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;

namespace JogoTextual.Telas
{
    /// Roupa, presente e local escolhidos para um encontro.
    public class EscolhasEncontro
    {
        public string RoupaId { get; set; } = string.Empty;

        /// Null quando o jogador vai sem presente.
        public string? PresenteId { get; set; }

        public string LocalId { get; set; } = string.Empty;
    }

    /// 
    /// Leitura validada das escolhas do encontro (sugestão do feedback da M2).
    /// Cada pergunta só termina com uma opção válida, então a satisfação
    /// nunca é calculada com uma escolha vazia.
    /// 
    public class LeitorEscolhasEncontro
    {
        private readonly ContextoJogo _c;

        public LeitorEscolhasEncontro(ContextoJogo contexto)
        {
            _c = contexto;
        }

        public EscolhasEncontro Ler()
        {
            return new EscolhasEncontro
            {
                RoupaId = EscolherDaLista("O que vai vestir?", _c.Catalogo.Encontro.Roupas),
                PresenteId = EscolherPresente(),
                LocalId = EscolherDaLista("Melhor escolher um bom lugar.", _c.Catalogo.Encontro.Locais)
            };
        }

        private string EscolherDaLista(string pergunta, List<OpcaoEncontro> opcoes)
        {
            _c.Saida.Secao(pergunta);
            for (int i = 0; i < opcoes.Count; i++)
            {
                _c.Saida.Texto($"  {i + 1}. {opcoes[i].Nome}: {opcoes[i].Descricao}");
            }
            return opcoes[_c.Entrada.LerOpcao(1, opcoes.Count) - 1].Id;
        }

        /// 
        /// "Vamos comprar presentes!": entrega um presente do inventário, compra um agora
        /// ou vai sem presente. O presente entregue sai do inventário.
        /// 
        private string? EscolherPresente()
        {
            while (true)
            {
                var meus = _c.Partida.Inventario.Keys
                    .Select(id => _c.Catalogo.BuscarItem(id))
                    .Where(item => item?.Categoria == CategoriaItem.Presente)
                    .Cast<Item>()
                    .OrderBy(item => item.Nome)
                    .ToList();

                _c.Saida.Secao("Vamos comprar presentes!");
                for (int i = 0; i < meus.Count; i++)
                {
                    _c.Saida.Texto($"  {i + 1}. Entregar {meus[i].Nome} (você tem {_c.Partida.Quantidade(meus[i].Id)})");
                }
                int opcaoComprar = meus.Count + 1;
                _c.Saida.Texto($"  {opcaoComprar}. Comprar um presente agora (carteira: R$ {_c.Partida.Dinheiro})");
                _c.Saida.Texto("  0. Ir sem presente");

                int maisBarato = _c.Catalogo.ItensDaCategoria(CategoriaItem.Presente)
                    .Select(item => _c.RegrasLoja.Preco(item, _c.Partida, _c.Salvos))
                    .DefaultIfEmpty(0)
                    .Min();
                if (meus.Count == 0 && _c.Partida.Dinheiro < maisBarato)
                {
                    _c.Saida.Narracao("(O dinheiro acabou... ir sem presente é melhor do que não ir!)");
                }

                int escolha = _c.Entrada.LerOpcao(0, opcaoComprar);
                if (escolha == 0) return null;
                if (escolha <= meus.Count)
                {
                    _c.Partida.RemoverItem(meus[escolha - 1].Id);
                    return meus[escolha - 1].Id;
                }

                ComprarPresente();
            }
        }

        private void ComprarPresente()
        {
            var presentes = _c.Catalogo.ItensDaCategoria(CategoriaItem.Presente);
            _c.Saida.Secao("Loja de presentes");
            for (int i = 0; i < presentes.Count; i++)
            {
                _c.Saida.Texto($"  {i + 1}. {TelaLoja.DescreverItem(_c, presentes[i])}");
            }
            _c.Saida.Texto("  0. Voltar");

            int escolha = _c.Entrada.LerOpcao(0, presentes.Count);
            if (escolha > 0)
            {
                TelaLoja.Comprar(_c, presentes[escolha - 1]);
            }
        }
    }
}