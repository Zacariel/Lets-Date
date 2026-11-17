using System;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// <summary>Cálculo da satisfação de cada encontro e da faixa de resultado.</summary>
    public static class RegrasEncontro
    {
        /// <summary>
        /// Satisfação começa em 70. Roupa, presente e local: +10 se forem os preferidos, -10 se não forem.
        /// Ir sem presente (<paramref name="presenteId"/> nulo) conta como presente errado.
        /// Cada encontro além do primeiro tira 10 pontos de todos os encontros da noite.
        /// Bônus de itens (perfume) é somado no fim. O resultado fica entre 0 e 100.
        /// </summary>
        public static ResultadoEncontro Calcular(Npc npc, string roupaId, string? presenteId, string localId, int totalEncontros, int bonus = 0)
        {
            var resultado = new ResultadoEncontro
            {
                RoupaCerta = roupaId == npc.Preferencias.Roupa,
                PresenteCerto = presenteId != null && presenteId == npc.Preferencias.Presente,
                LocalCerto = localId == npc.Preferencias.Local,
                PenalidadeEncontrosExtras = Penalidade(totalEncontros),
                Bonus = bonus
            };

            int satisfacao = Constantes.SatisfacaoInicial;
            satisfacao += resultado.RoupaCerta ? Constantes.AjusteEscolha : -Constantes.AjusteEscolha;
            satisfacao += resultado.PresenteCerto ? Constantes.AjusteEscolha : -Constantes.AjusteEscolha;
            satisfacao += resultado.LocalCerto ? Constantes.AjusteEscolha : -Constantes.AjusteEscolha;
            satisfacao -= resultado.PenalidadeEncontrosExtras;
            satisfacao += bonus;

            resultado.Satisfacao = Math.Clamp(satisfacao, Constantes.SatisfacaoMinima, Constantes.SatisfacaoMaxima);
            resultado.Avaliacao = Classificar(resultado.Satisfacao);
            return resultado;
        }

        /// <summary>-10 de satisfação para cada encontro marcado além do primeiro.</summary>
        public static int Penalidade(int totalEncontros)
        {
            return Math.Max(0, totalEncontros - 1) * Constantes.PenalidadePorEncontroExtra;
        }

        public static AvaliacaoEncontro Classificar(int satisfacao)
        {
            if (satisfacao >= 81) return AvaliacaoEncontro.Adorou;
            if (satisfacao >= 61) return AvaliacaoEncontro.Gostou;
            if (satisfacao >= 41) return AvaliacaoEncontro.Normal;
            if (satisfacao >= 21) return AvaliacaoEncontro.NaoGostou;
            return AvaliacaoEncontro.Odiou;
        }

        public static string Mensagem(AvaliacaoEncontro avaliacao, string nomeNpc)
        {
            return avaliacao switch
            {
                AvaliacaoEncontro.Adorou => $"{nomeNpc} adorou o encontro!",
                AvaliacaoEncontro.Gostou => $"{nomeNpc} gostou do encontro!",
                AvaliacaoEncontro.Normal => $"{nomeNpc} achou o encontro normal.",
                AvaliacaoEncontro.NaoGostou => $"{nomeNpc} não gostou do encontro...",
                _ => $"{nomeNpc} odiou o encontro!"
            };
        }

        public static string Estrelas(AvaliacaoEncontro avaliacao)
        {
            int cheias = (int)avaliacao;
            return new string('★', cheias) + new string('☆', 5 - cheias);
        }
    }
}
