namespace JogoTextual.Dominio
{
    // Resultados devolvidos pelas classes de regras. As telas só leem esses
    // resultados para decidir o que mostrar, sem repetir a lógica das regras.

    public class ResultadoFlerte
    {
        public bool Permitido { get; set; }
        public int Dado { get; set; }
        public int Variacao { get; set; }
        public string Fala { get; set; } = string.Empty;
    }

    public enum MotivoConvite
    {
        Aceito,
        InteresseBaixo,
        RecusadoNoSorteio
    }

    public class ResultadoConvite
    {
        public bool Aceito => Motivo == MotivoConvite.Aceito;
        public MotivoConvite Motivo { get; set; }

        /// <summary>Número sorteado de 1 a 100 (0 quando não houve sorteio).</summary>
        public int Sorteio { get; set; }

        /// <summary>Loja cujo desconto foi liberado com este convite (se houver).</summary>
        public LojaNpc? DescontoLiberado { get; set; }
    }

    public enum ResultadoCompra
    {
        Sucesso,
        SemDinheiro,
        ItemInexistente
    }

    public enum ResultadoCompraConversa
    {
        Sucesso,
        NomeIncorreto,
        JaComprouHoje,
        LimiteAtingido,
        SemDinheiro,
        Cancelada
    }

    public enum ResultadoUsoItem
    {
        Sucesso,
        NaoPossui,
        NaoUsavelAgora,
        JaUsado,
        SemEfeito
    }

    public class ResultadoEncontro
    {
        public int Satisfacao { get; set; }
        public AvaliacaoEncontro Avaliacao { get; set; }
        public bool RoupaCerta { get; set; }
        public bool PresenteCerto { get; set; }
        public bool LocalCerto { get; set; }
        public int PenalidadeEncontrosExtras { get; set; }
        public int Bonus { get; set; }
    }
}
