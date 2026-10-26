namespace JogoTextual.Dominio
{
    public class Npc
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Biografia { get; set; } = string.Empty;
        public string VestuarioPreferido { get; set; } = string.Empty;
        public string PresentePreferido { get; set; } = string.Empty;
        public string LocalPreferido { get; set; } = string.Empty;

        public string PerguntaBom { get; set; } = string.Empty;
        public string RespostaBom { get; set; } = string.Empty;
        public string PerguntaRegular { get; set; } = string.Empty;
        public string RespostaRegular { get; set; } = string.Empty;
        public string PerguntaRuim { get; set; } = string.Empty;
        public string RespostaRuim { get; set; } = string.Empty;
        public string PerguntaPessimo { get; set; } = string.Empty;
        public string RespostaPessimo { get; set; } = string.Empty;
    }
}