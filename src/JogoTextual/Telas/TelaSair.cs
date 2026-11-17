using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// <summary>Sair: ir para o(s) encontro(s) ou fechar o aplicativo guardando as conversas.</summary>
    public class TelaSair : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Sair");
            c.Saida.Menu(new[]
            {
                (1, "Sair para encontro"),
                (2, "Sair do aplicativo"),
                (0, "Voltar")
            });

            switch (c.Entrada.LerOpcao(0, 2))
            {
                case 1:
                    return Tela.Encontros;
                case 2:
                    return SairDoAplicativo(c);
                default:
                    return Tela.MenuInicial;
            }
        }

        private static Tela SairDoAplicativo(ContextoJogo c)
        {
            // Sair antes de iniciar qualquer conversa leva ao Final 1 – Melhor nem tentar.
            if (c.Partida.ConversasIniciadas == 0)
            {
                c.FecharAposFinal = true;
                return Tela.Final;
            }

            if (c.Partida.EncontrosMarcados.Count > 0 &&
                !c.Entrada.LerSimNao($"Você tem {c.Partida.EncontrosMarcados.Count} encontro(s) marcado(s) e vai perder todos se sair agora. Sair mesmo assim?"))
            {
                return Tela.Sair;
            }

            RegrasDia.GuardarConversas(c.Salvos, c.Partida);
            c.Salvar();
            if (c.Salvos.ConversasGuardadas > 0)
            {
                c.Saida.Sucesso($"Suas {c.Salvos.ConversasGuardadas} conversa(s) restantes foram guardadas para a próxima vez (máximo de {Constantes.ConversasMaximas} por dia).");
            }
            return Tela.Encerrar;
        }
    }
}
