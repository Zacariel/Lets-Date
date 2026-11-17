using System;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// <summary>Menu Inicial: tela principal do aplicativo durante o dia.</summary>
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

            c.Saida.Texto();
            c.Saida.Menu(new[]
            {
                (1, "Buscar encontro!"),
                (2, "Loja"),
                (3, "Inventário"),
                (4, "Configurações"),
                (5, "Avaliações e finais"),
                (0, "Sair")
            });

            switch (c.Entrada.LerOpcao(new[] { 1, 2, 3, 4, 5, 0 }))
            {
                case 1: return BuscarEncontro(c);
                case 2: return Tela.Loja;
                case 3: return Tela.Inventario;
                case 4: return Tela.Perfil;
                case 5: return Tela.Avaliacoes;
                default: return Tela.Sair;
            }
        }

        public static void MostrarResumo(ContextoJogo c)
        {
            var p = c.Partida;
            c.Saida.Texto($"Olá, {c.Perfil.Apelido}!  Dia {c.Salvos.DiasJogados} no Let's Date.");
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

            Npc? npc = c.RegrasConversa.IniciarConversa(c.Partida, c.Perfil);
            if (npc == null)
            {
                c.Saida.Aviso("Você já viu todos os perfis compatíveis da sua região hoje!");
                c.Saida.Texto("Hora de se arrumar para a noite...");
                c.Entrada.Pausar();
                return Tela.Encontros;
            }

            c.Salvar();
            return Tela.Conversa;
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
