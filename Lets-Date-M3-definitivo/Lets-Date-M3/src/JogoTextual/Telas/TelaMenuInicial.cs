// e reação do NPC à biografia do jogador.
// A opção nova ficou no 6 para não mudar os números que já estão no roteiro de testes.
using System;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Menu Inicial: tela principal do aplicativo durante o dia.
    public class TelaMenuInicial : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            if (c.VerificarNotificacao)
            {
                c.VerificarNotificacao = false;
                MostrarNotificacao(c);
            }

            c.Saida.Cabecalho("Menu Inicial");
            MostrarResumo(c);

            int matches = RegrasRelacionamento.MatchesAtivos(c.Salvos.Campanha).Count; 

            c.Saida.Texto();
            c.Saida.Menu(new[]
            {
                (1, "Buscar encontro!"),
                (2, "Loja"),
                (3, "Inventário"),
                (4, "Configurações"),
                (5, "Avaliações e finais"),
                (6, $"Meus matches ({matches})"), 
                (0, "Sair")
            });

            switch (c.Entrada.LerOpcao(new[] { 1, 2, 3, 4, 5, 6, 0 }))
            {
                case 1: return BuscarEncontro(c);
                case 2: return Tela.Loja;
                case 3: return Tela.Inventario;
                case 4: return Tela.Perfil;
                case 5: return Tela.Avaliacoes;
                case 6: return Tela.Matches; 
                default: return Tela.Sair;
            }
        }

        public static void MostrarResumo(ContextoJogo c)
        {
            var p = c.Partida;
            c.Saida.Texto($"Olá, {c.Perfil.Apelido}!");
            if (c.Salvos.Campanha != null) c.Saida.Destaque(RegrasCampanha.DescreverDia(c.Salvos.Campanha)); 
            c.Saida.Texto($"Conversas disponíveis: {p.Conversas}   Carteira: R$ {p.Dinheiro}   Encontros marcados: {p.EncontrosMarcados.Count}");
            c.Saida.Barra("Coragem", p.Coragem, 0, Constantes.CoragemMaxima, ConsoleColor.Yellow);
            if (p.CupomDesconto > 0) c.Saida.Destaque($"Cupom ativo: {p.CupomDesconto}% na próxima compra.");
        }

        private static Tela BuscarEncontro(ContextoJogo c)
        {
            if (c.Partida.Conversas <= 0)
            {
                return FluxoFimDasConversas.Executar(c);
            }

            Npc? npc = c.RegrasConversa.IniciarConversa(c.Partida, c.Perfil, c.Salvos.Campanha); //  passa a campanha
            if (npc == null)
            {
                c.Saida.Aviso("Você já viu todos os perfis novos compatíveis da sua região hoje!");
                //  se ainda tiver matches para conversar, avisa em vez de ir direto para a noite.
                if (RegrasRelacionamento.MatchesAtivos(c.Salvos.Campanha).Count > 0)
                {
                    c.Saida.Texto("Mas você ainda pode falar com alguém em \"Meus matches\".");
                    c.Entrada.Pausar();
                    return Tela.MenuInicial;
                }
                c.Saida.Texto("Hora de se arrumar para a noite...");
                c.Entrada.Pausar();
                return Tela.Encontros;
            }

            ReagirABiografia(c, npc); 
            c.Salvar();
            return Tela.Conversa;
        }

        ///  se a bio do jogador tiver uma palavra-chave do NPC, ele comenta e o interesse começa +10.
        private static void ReagirABiografia(ContextoJogo c, Npc npc)
        {
            string? palavra = RegrasBiografia.PalavraEmComum(c.Perfil.Biografia, npc);
            if (palavra == null || c.Partida.ConversaAtual == null) return;

            RegrasConversa.AlterarInteresse(c.Partida.ConversaAtual, Constantes.BonusBiografia);

            c.Saida.Cabecalho($"{npc.Nome} leu sua bio!");
            c.Saida.Fala(npc.Nome, c.Personalizar(npc.ReacaoBiografia).Replace("{palavra}", palavra));
            c.Saida.Sucesso($"Vocês têm \"{palavra}\" em comum! (+{Constantes.BonusBiografia} de interesse)");
            c.Entrada.Pausar();
        }

        private static void MostrarNotificacao(ContextoJogo c)
        {
            EventoCelular? evento = c.RegrasEventos.TalvezSortear();
            if (evento == null) return;

            c.Saida.Cabecalho("Nova notificação");
            c.Saida.Destaque($"[{evento.Remetente}]");
            c.Saida.Texto(c.Personalizar(evento.Mensagem));
            c.Saida.Texto();
            c.Saida.Sucesso(c.RegrasEventos.Aplicar(evento, c.Partida));
            c.Entrada.Pausar();
        }
    }
}
