using System;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// Cálculo da satisfação de cada encontro e da faixa de resultado.
    public static class RegrasEncontro
    {
        /// Satisfação começa em 70. Roupa, presente e local: +10 se forem os preferidos, -10 se não forem.
        /// Ir sem presente (<paramref name="presenteId"/> nulo) conta como presente errado.
        /// Cada encontro além do primeiro tira 10 pontos de todos os encontros da noite.
        /// Bônus de itens (perfume) é somado no fim. O resultado fica entre 0 e 100.
        /// NOVO: <paramref name="preferencias"/> permite usar os gostos do capítulo atual do NPC
        /// (se for null, usa npc.Preferencias) e <paramref name="penalidadeRepeticao"/> desconta
        /// presente ou lugar repetidos com a mesma pessoa.
        public static ResultadoEncontro Calcular(Npc npc, string roupaId, string? presenteId, string localId, int totalEncontros,
                                                 int bonus = 0, PreferenciasNpc? preferencias = null, int penalidadeRepeticao = 0)
        {
            PreferenciasNpc gostos = preferencias ?? npc.Preferencias; // NOVO

            var resultado = new ResultadoEncontro
            {
                RoupaCerta = roupaId == gostos.Roupa,
                PresenteCerto = presenteId != null && presenteId == gostos.Presente,
                LocalCerto = localId == gostos.Local,
                PenalidadeEncontrosExtras = Penalidade(totalEncontros),
                PenalidadeRepeticao = penalidadeRepeticao, // NOVO
                Bonus = bonus
            };

            int satisfacao = Constantes.SatisfacaoInicial;
            satisfacao += resultado.RoupaCerta ? Constantes.AjusteEscolha : -Constantes.AjusteEscolha;
            satisfacao += resultado.PresenteCerto ? Constantes.AjusteEscolha : -Constantes.AjusteEscolha;
            satisfacao += resultado.LocalCerto ? Constantes.AjusteEscolha : -Constantes.AjusteEscolha;
            satisfacao -= resultado.PenalidadeEncontrosExtras;
            satisfacao -= penalidadeRepeticao; // NOVO
            satisfacao += bonus;

            resultado.Satisfacao = Math.Clamp(satisfacao, Constantes.SatisfacaoMinima, Constantes.SatisfacaoMaxima);
            resultado.Avaliacao = Classificar(resultado.Satisfacao);
            return resultado;
        }

        /// -10 de satisfação para cada encontro marcado além do primeiro.
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
