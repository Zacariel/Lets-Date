
namespace JogoTextual.Regras
{
    /// Valores das regras do documento do jogo, reunidos num só lugar.
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

        // Campanha: quantos dias ANTES do Dia dos Namorados cada dificuldade começa
        public const int DiasAntesFacil = 7;    // 1 semanas
        public const int DiasAntesMedio = 3;    // 3 dias 
        public const int DiasAntesDificil = 0;  // no próprio dia (jogo clássico)

        // Relacionamento
        public const int NivelMaximoRelacionamento = 3;  // 1 Match, 2 Ficando, 3 Namorando
        public const int InteresseExtraPorNivel = 15;    // match começa a conversa com 35, ficando 50, namorando 65
        public const int PenalidadeRepeticao = 5;        // repetir presente ou lugar com a mesma pessoa

        //Biografia
        public const int BonusBiografia = 10;            // NPC achou algo em comum na sua bio
    }
}
