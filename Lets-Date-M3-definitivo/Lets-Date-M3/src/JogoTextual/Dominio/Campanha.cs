using System.Collections.Generic;

namespace JogoTextual.Dominio
{
    /// Relação com um NPC com quem o jogador já saiu. Fica salva entre os dias da campanha.
    /// Nível: 0 = sem match, 1 = Match, 2 = Ficando, 3 = Namorando.
    public class Relacionamento
    {
        public string NpcId { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public int EncontrosRealizados { get; set; }
        public int UltimaSatisfacao { get; set; }

        /// Presentes já entregues para essa pessoa (repetir perde pontos).
        public List<string> PresentesDados { get; set; } = new List<string>();

        /// Locais onde vocês já foram juntos (repetir perde pontos).
        public List<string> LocaisVisitados { get; set; } = new List<string>();
    }

    /// Contagem regressiva até o Dia dos Namorados, definida pela dificuldade.
    /// O último dia da campanha é sempre o próprio Dia dos Namorados.
    /// Fácil: 14 dias antes + o dia (15). Médio: 7 + 1 (8). Difícil: só o dia (1) = jogo clássico da M3.
    public class Campanha
    {
        public Dificuldade Dificuldade { get; set; }

        ///Total de dias, contando o Dia dos Namorados.
        public int DiasTotais { get; set; } = 1;

        ///Dia atual, começando em 1.
        public int DiaAtual { get; set; } = 1;

        ///  Relacionamentos com os NPCs (id do NPC → relacionamento).
        public Dictionary<string, Relacionamento> Relacionamentos { get; set; } = new Dictionary<string, Relacionamento>();

        public int FaltamDias => DiasTotais - DiaAtual;
        public bool EhDiaDosNamorados => FaltamDias <= 0;
    }
}
