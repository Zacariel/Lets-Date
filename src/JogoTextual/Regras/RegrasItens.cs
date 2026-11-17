using System;
using JogoTextual.Dados;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// <summary>Onde o item está sendo usado.</summary>
    public enum MomentoUso
    {
        Menu,
        Conversa,
        Encontro
    }

    /// <summary>Uso dos itens consumíveis (energético, super like, perfume...).</summary>
    public class RegrasItens
    {
        private readonly CatalogoJogo _catalogo;

        public RegrasItens(CatalogoJogo catalogo)
        {
            _catalogo = catalogo;
        }

        /// <summary>Diz se o item faz sentido no momento, sem gastar nada.</summary>
        public static bool UsavelEm(Item item, MomentoUso momento)
        {
            if (item.Categoria != CategoriaItem.Consumivel) return false;
            return item.Efeito switch
            {
                TipoEfeito.Coragem => momento != MomentoUso.Encontro,
                TipoEfeito.Interesse => momento == MomentoUso.Conversa,
                TipoEfeito.Satisfacao => momento == MomentoUso.Encontro,
                _ => false
            };
        }

        /// <summary>
        /// Usa o item e aplica o efeito. Para itens de satisfação, o bônus é devolvido em
        /// <paramref name="bonusSatisfacao"/> para ser somado no cálculo do encontro.
        /// </summary>
        public ResultadoUsoItem Usar(string itemId, EstadoPartida partida, MomentoUso momento, out int bonusSatisfacao)
        {
            bonusSatisfacao = 0;
            Item? item = _catalogo.BuscarItem(itemId);
            if (item == null || partida.Quantidade(itemId) <= 0) return ResultadoUsoItem.NaoPossui;
            if (!UsavelEm(item, momento)) return ResultadoUsoItem.NaoUsavelAgora;

            switch (item.Efeito)
            {
                case TipoEfeito.Coragem:
                    if (partida.Coragem >= Constantes.CoragemMaxima) return ResultadoUsoItem.SemEfeito;
                    partida.Coragem = Math.Min(Constantes.CoragemMaxima, partida.Coragem + item.ValorEfeito);
                    break;

                case TipoEfeito.Interesse:
                    var conversa = partida.ConversaAtual;
                    if (conversa == null) return ResultadoUsoItem.NaoUsavelAgora;
                    if (conversa.SuperLikeUsado) return ResultadoUsoItem.JaUsado;
                    RegrasConversa.AlterarInteresse(conversa, item.ValorEfeito);
                    conversa.SuperLikeUsado = true;
                    break;

                case TipoEfeito.Satisfacao:
                    bonusSatisfacao = item.ValorEfeito;
                    break;
            }

            partida.RemoverItem(itemId);
            return ResultadoUsoItem.Sucesso;
        }
    }
}
