using System.Collections.Generic;

namespace JogoTextual.Dominio
{
    public class EstadoJogo
    {
        public PerfilJogador Jogador { get; set; } = new PerfilJogador();
        public int ConversasDisponiveis { get; set; } = 3;
        public int Coragem { get; set; } = 100;
        public int InteresseAtualNpc { get; set; } = 20;
        public string TelaAtual { get; set; } = "Login";

        public List<string> NpcsJaVistos { get; set; } = new List<string>();
        public Npc? NpcAtual { get; set; }

        public int ConversasIniciadas { get; set; } = 0;
        public List<Npc> NpcsComEncontroMarcado { get; set; } = new List<Npc>();

        // NOVO: Guarda o histórico de estrelas dos encontros
        public List<string> HistoricoAvaliacoes { get; set; } = new List<string>();
    }
}