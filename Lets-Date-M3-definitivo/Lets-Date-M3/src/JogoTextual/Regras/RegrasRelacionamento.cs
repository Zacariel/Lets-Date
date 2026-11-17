using System;
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// Relacionamentos que continuam entre os dias da campanha.
    ///
    /// Como o nível muda depois de cada encontro:
    ///   Gostou ou Adorou (4-5 estrelas)  → sobe 1 nível (máximo 3)
    ///   Normal (3 estrelas)              → fica igual
    ///   Não gostou ou Odiou (1-2)        → desce 1 nível (mínimo 0)
    /// Nível 0 = não é match (a pessoa volta a aparecer só no sorteio).
    public static class RegrasRelacionamento
    {
        public static string NomeDoNivel(int nivel)
        {
            return nivel switch
            {
                0 => "Sem match",
                1 => "Match",
                2 => "Ficando",
                _ => "Namorando"
            };
        }

        public static Relacionamento? Buscar(Campanha? campanha, string npcId)
        {
            if (campanha == null) return null;
            return campanha.Relacionamentos.TryGetValue(npcId, out var r) ? r : null;
        }

        public static bool Ativo(Relacionamento? r) => r != null && r.Nivel > 0;

        /// Ids dos NPCs que são match (nível 1 ou mais).
        public static List<string> MatchesAtivos(Campanha? campanha)
        {
            if (campanha == null) return new List<string>();
            return campanha.Relacionamentos.Values
                .Where(r => r.Nivel > 0)
                .Select(r => r.NpcId)
                .ToList();
        }

        /// Interesse no começo da conversa: 20 para desconhecidos; +15 por nível (35, 50, 65).
        public static int InteresseInicial(Relacionamento? r)
        {
            if (!Ativo(r)) return Constantes.InteresseInicial;
            return Constantes.InteresseInicial + r!.Nivel * Constantes.InteresseExtraPorNivel;
        }

        /// -5 se repetir o presente e -5 se repetir o lugar de um encontro anterior com a mesma pessoa.
        public static int PenalidadeRepeticao(Relacionamento? r, string? presenteId, string localId)
        {
            if (r == null) return 0;

            int penalidade = 0;
            if (presenteId != null && r.PresentesDados.Contains(presenteId)) penalidade += Constantes.PenalidadeRepeticao;
            if (r.LocaisVisitados.Contains(localId)) penalidade += Constantes.PenalidadeRepeticao;
            return penalidade;
        }

        /// Guarda o resultado do encontro e ajusta o nível. Cria o relacionamento se for o primeiro encontro.
        public static Relacionamento RegistrarEncontro(Campanha campanha, string npcId, ResultadoEncontro resultado,
                                                       string? presenteId, string localId)
        {
            if (!campanha.Relacionamentos.TryGetValue(npcId, out var r))
            {
                r = new Relacionamento { NpcId = npcId };
                campanha.Relacionamentos[npcId] = r;
            }

            r.EncontrosRealizados++;
            r.UltimaSatisfacao = resultado.Satisfacao;

            if (resultado.Avaliacao >= AvaliacaoEncontro.Gostou)
                r.Nivel = Math.Min(Constantes.NivelMaximoRelacionamento, r.Nivel + 1);
            else if (resultado.Avaliacao <= AvaliacaoEncontro.NaoGostou)
                r.Nivel = Math.Max(0, r.Nivel - 1);

            if (presenteId != null) r.PresentesDados.Add(presenteId);
            r.LocaisVisitados.Add(localId);
            return r;
        }

        /// Pedido de namoro: depois do encontro, o relacionamento está no nível máximo
        /// e o encontro foi bom (4 ou 5 estrelas). Usado só no Dia dos Namorados.
        public static bool ConfirmaNamoro(Relacionamento r, ResultadoEncontro resultado)
        {
            return r.Nivel >= Constantes.NivelMaximoRelacionamento && resultado.Avaliacao >= AvaliacaoEncontro.Gostou;
        }
    }
}
