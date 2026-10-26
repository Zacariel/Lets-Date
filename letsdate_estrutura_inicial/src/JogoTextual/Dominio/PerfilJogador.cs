namespace JogoTextual.Dominio
{
    public class PerfilJogador
    {
        public string Nome { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Buscando { get; set; } = string.Empty;
        public string Biografia { get; set; } = string.Empty;
    }
}