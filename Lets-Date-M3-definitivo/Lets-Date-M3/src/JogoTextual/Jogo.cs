using System.Collections.Generic;
using JogoTextual.Dominio;
using JogoTextual.Telas;

namespace JogoTextual
{
    /// Ciclo principal. O Jogo só guarda qual tela está ativa e faz as transições:
    /// cada tela executa, devolve a próxima <see cref="Tela"/> e o laço continua até Encerrar.
    public class Jogo
    {
        private readonly ContextoJogo _contexto;
        private readonly Dictionary<Tela, ITelaJogo> _telas;

        public Tela TelaAtual { get; private set; } = Tela.Login;

        public Jogo(ContextoJogo contexto)
        {
            _contexto = contexto;
            _telas = new Dictionary<Tela, ITelaJogo>
            {
                [Tela.Login] = new TelaLogin(),
                [Tela.MenuInicial] = new TelaMenuInicial(),
                [Tela.Perfil] = new TelaPerfil(),
                [Tela.Loja] = new TelaLoja(),
                [Tela.Inventario] = new TelaInventario(),
                [Tela.Conversa] = new TelaConversa(),
                [Tela.Matches] = new TelaMatches(),  
                [Tela.Avaliacoes] = new TelaAvaliacoes(),
                [Tela.Sair] = new TelaSair(),
                [Tela.Encontros] = new TelaEncontros(),
                [Tela.FimDoDia] = new TelaFimDoDia(),   
                [Tela.Final] = new TelaFinal()
            };
        }

        public void Iniciar()
        {
            while (TelaAtual != Tela.Encerrar)
            {
                TelaAtual = _telas[TelaAtual].Executar(_contexto);
            }

            _contexto.Saida.Texto();
            _contexto.Saida.Destaque("Fechando o Let's Date... Até a próxima! ♥");
        }
    }
}
