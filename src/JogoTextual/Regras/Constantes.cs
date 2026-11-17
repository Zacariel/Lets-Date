namespace JogoTextual.Regras
{
    /// <summary>Valores das regras do documento do jogo, reunidos num só lugar.</summary>
    public static class Constantes
    {
        // Conversas
        public const int ConversasIniciais = 3;
        public const int ConversasMaximas = 4;
        public const int PrecoConversaExtra = 5;

        // Interesse
        public const int InteresseInicial = 20;
        public const int InteresseMaximo = 100;
        public const int InteresseMinimo = -100;
        public const int InteresseBloqueio = -60;
        public const int InteresseMinimoConvite = 60;

        public const int VariacaoBom = 40;
        public const int VariacaoRegular = 20;
        public const int VariacaoRuim = -10;
        public const int VariacaoPessimo = -30;

        // Coragem e flerte
        public const int CoragemMaxima = 100;
        public const int CustoFlerte = 25;

        // Carteira
        public const int DinheiroInicial = 70;

        // Encontro
        public const int SatisfacaoInicial = 70;
        public const int SatisfacaoMaxima = 100;
        public const int SatisfacaoMinima = 0;
        public const int AjusteEscolha = 10;
        public const int PenalidadePorEncontroExtra = 10;

        // Eventos do celular: chance (em %) de chegar uma notificação ao voltar para o menu
        public const int ChanceEvento = 50;

        // Login
        public const int IdadeMinima = 18;
        public const int IdadeMaxima = 100;
        public const int TamanhoMaximoBiografia = 100;
    }
}
