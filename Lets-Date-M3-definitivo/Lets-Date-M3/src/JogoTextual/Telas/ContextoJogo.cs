using JogoTextual.Dados;
using JogoTextual.Dominio;
using JogoTextual.Interface;
using JogoTextual.Persistencia;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Uma tela do jogo. Executa uma vez e devolve qual tela vem depois.
    public interface ITelaJogo
    {
        Tela Executar(ContextoJogo contexto);
    }

  
    // Tudo o que as telas precisam: conteúdo, estado, entrada/saída e regras.
    public class ContextoJogo
    {
        public CatalogoJogo Catalogo { get; }
        public SaidaConsole Saida { get; }
        public EntradaConsole Entrada { get; }
        public IAleatorio Aleatorio { get; }
        public PersistenciaJogo Persistencia { get; }

        public RegrasConversa RegrasConversa { get; }
        public RegrasLoja RegrasLoja { get; }
        public RegrasItens RegrasItens { get; }
        public RegrasEventos RegrasEventos { get; }

        public DadosSalvos Salvos { get; set; }
        public EstadoPartida Partida { get; set; } = new EstadoPartida();

        // Quando verdadeiro, o menu sorteia uma notificação do celular antes de aparecer.
        public bool VerificarNotificacao { get; set; }

        // Quando verdadeiro, a tela de final fecha o aplicativo em vez de oferecer outro dia.
        public bool FecharAposFinal { get; set; }

        public ContextoJogo(CatalogoJogo catalogo, SaidaConsole saida, EntradaConsole entrada,
                            IAleatorio aleatorio, PersistenciaJogo persistencia, DadosSalvos salvos)
        {
            Catalogo = catalogo;
            Saida = saida;
            Entrada = entrada;
            Aleatorio = aleatorio;
            Persistencia = persistencia;
            Salvos = salvos;

            RegrasConversa = new RegrasConversa(catalogo, aleatorio);
            RegrasLoja = new RegrasLoja(catalogo);
            RegrasItens = new RegrasItens(catalogo);
            RegrasEventos = new RegrasEventos(catalogo, aleatorio);
        }

        public PerfilJogador Perfil => Salvos.Perfil ?? new PerfilJogador();

        // Troca {apelido} nos textos dos JSON pelo apelido do jogador
        public string Personalizar(string texto) => texto.Replace("{apelido}", Perfil.Apelido);

        public void Salvar() => Persistencia.Salvar(Salvos);
    }
}
