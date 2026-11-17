using System;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Mostra o final do Dia dos Namorados, registra na galeria e oferece outra campanha.
    public class TelaFinal : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            FinalJogo final = RegrasFinais.Registrar(c.Salvos, c.Partida);
            TextoFinal? texto = c.Catalogo.BuscarFinal(final);
            c.Salvos.Campanha = null; //  a campanha terminou

            c.Saida.Cabecalho("Fim da noite");
            c.Saida.Aviso($"Final {(int)final} — {texto?.Titulo ?? final.ToString()}");
            c.Saida.Texto();
            c.Saida.Texto(c.Personalizar(texto?.Texto ?? string.Empty));

            c.Saida.Secao("Resumo do dia");
            c.Saida.Texto($"Conversas iniciadas: {c.Partida.ConversasIniciadas}");
            c.Saida.Texto($"Encontros conseguidos: {c.Partida.EncontrosMarcados.Count}");
            int total = Enum.GetValues(typeof(FinalJogo)).Length;
            c.Saida.Texto($"Finais desbloqueados: {c.Salvos.FinaisDesbloqueados.Count}/{total}");
            c.Salvar();

            if (c.FecharAposFinal)
            {
                // Saiu do aplicativo sem conversar: as conversas do dia ficam guardadas para a próxima vez.
                RegrasDia.GuardarConversas(c.Salvos, c.Partida);
                c.Salvar();
                c.Entrada.Pausar("Aperte ENTER para fechar o aplicativo...");
                return Tela.Encerrar;
            }

            c.Saida.Texto();
            c.Saida.Texto("Deseja começar uma nova contagem até o Dia dos Namorados?");
            c.Saida.Menu(new[] { (1, "Sim, jogar de novo!"), (2, "Não, fechar o aplicativo.") });

            if (c.Entrada.LerOpcao(1, 2) == 1)
            {
                c.Salvos.Campanha = RegrasCampanha.Nova(TelaLogin.EscolherDificuldade(c)); 
                c.Partida = RegrasDia.NovoDia(c.Salvos);
                c.Salvar();
                return Tela.MenuInicial;
            }

            return Tela.Encerrar;
        }
    }
}
