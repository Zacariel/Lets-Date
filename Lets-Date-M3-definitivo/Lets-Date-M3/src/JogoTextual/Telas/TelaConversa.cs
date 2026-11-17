using System;
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    public class TelaConversa : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            var conversa = c.Partida.ConversaAtual;
            Npc? npc = conversa == null ? null : c.Catalogo.BuscarNpc(conversa.NpcId);
            if (conversa == null || npc == null)
            {
                RegrasConversa.EncerrarConversa(c.Partida);
                return Tela.MenuInicial;
            }

            MostrarPerfil(c, npc, conversa);

            c.Saida.Texto();
            bool temAssunto = RegrasConversa.TopicoAtual(conversa, npc) != null;

            string? dica = c.Salvos.Campanha == null
                ? null
                : RegrasCampanha.DicaConvite(c.Salvos.Campanha.Dificuldade, conversa.Interesse);

            c.Saida.Menu(new[]
            {
                (1, temAssunto ? "Conversar" : "Conversar (vocês já falaram de tudo)"),
                (2, $"Flertar (custa {Constantes.CustoFlerte} de coragem)"),
                (3, "Usar item"),
                (4, dica == null ? "Chamar para encontro" : $"Chamar para encontro ({dica})"),
                (5, "Bloquear")
            });

            switch (c.Entrada.LerOpcao(1, 5))
            {
                case 1: return Conversar(c, npc, conversa);
                case 2: return Flertar(c, npc, conversa);
                case 3: return UsarItem(c);
                case 4: return Convidar(c, npc);
                default: return Bloquear(c, npc);
            }
        }

        private static void MostrarPerfil(ContextoJogo c, Npc npc, EstadoConversa conversa)
        {
            c.Saida.Cabecalho($"Conversando com {npc.Nome}");
            c.Saida.Texto($"{npc.Nome}, {npc.Idade} anos");
            c.Saida.Narracao(npc.Biografia);
            c.Saida.Texto();
            c.Saida.Barra("Interesse", conversa.Interesse, Constantes.InteresseMinimo, Constantes.InteresseMaximo, ConsoleColor.Magenta);
            c.Saida.Barra("Coragem", c.Partida.Coragem, 0, Constantes.CoragemMaxima, ConsoleColor.Yellow);

            var rel = RegrasRelacionamento.Buscar(c.Salvos.Campanha, npc.Id);
            if (RegrasRelacionamento.Ativo(rel))
            {
                c.Saida.Destaque($"Relacionamento: {RegrasRelacionamento.NomeDoNivel(rel!.Nivel)} ({rel.EncontrosRealizados} encontro(s) juntos)");
            }

            if (c.Partida.Anotacoes.TryGetValue(npc.Id, out var notas) && notas.Count > 0)
            {
                c.Saida.Secao("Suas anotações");
                foreach (var nota in notas) c.Saida.Texto($"  • {nota}");
            }
        }

        private static Tela Conversar(ContextoJogo c, Npc npc, EstadoConversa conversa)
        {
            TopicoDialogo? topico = RegrasConversa.TopicoAtual(conversa, npc);
            if (topico == null)
            {
                c.Saida.Aviso($"Vocês já falaram de tudo! Talvez seja hora de flertar ou chamar {npc.Nome} para sair.");
                c.Entrada.Pausar();
                return Tela.Conversa;
            }

            c.Saida.Texto();
            c.Saida.Fala(npc.Nome, c.Personalizar(topico.Fala));
            c.Saida.Texto();

            // As opções são embaralhadas: a melhor resposta não fica sempre no número 1.
            List<OpcaoDialogo> opcoes = c.Aleatorio.Embaralhar(topico.Opcoes);
            for (int i = 0; i < opcoes.Count; i++)
            {
                c.Saida.Texto($"  {i + 1}. \"{c.Personalizar(opcoes[i].Texto)}\"");
            }

            OpcaoDialogo escolhida = opcoes[c.Entrada.LerOpcao(1, opcoes.Count, "Sua resposta: ") - 1];
            int variacao = c.RegrasConversa.Responder(c.Partida, npc, escolhida);

            c.Saida.Texto();
            c.Saida.Fala(npc.Nome, c.Personalizar(escolhida.Resposta));
            MostrarVariacao(c, variacao);
            if (!string.IsNullOrWhiteSpace(escolhida.Dica))
            {
                c.Saida.Destaque($"Anotado: {escolhida.Dica}");
            }

            return VerificarBloqueio(c, npc, conversa);
        }

        private static Tela Flertar(ContextoJogo c, Npc npc, EstadoConversa conversa)
        {
            var resultado = c.RegrasConversa.Flertar(c.Partida);
            c.Saida.Texto();

            if (!resultado.Permitido)
            {
                c.Saida.Aviso("Oh, parece que você não tem coragem o suficiente para flertar agora...");
                c.Saida.Narracao("Dica: um Energético da Coragem (Loja) recupera a coragem.");
                c.Entrada.Pausar();
                return Tela.Conversa;
            }

            c.Saida.Fala("Você", resultado.Fala);
            c.Saida.Narracao($"(D20: {resultado.Dado})");
            if (resultado.Variacao > 0) c.Saida.Sucesso($"{npc.Nome} adorou a cantada!");
            else if (resultado.Variacao == 0) c.Saida.Texto($"{npc.Nome} deu uma risadinha educada.");
            else c.Saida.Erro($"{npc.Nome} achou a cantada péssima...");
            MostrarVariacao(c, resultado.Variacao);

            return VerificarBloqueio(c, npc, conversa);
        }

        private static Tela UsarItem(ContextoJogo c)
        {
            var usaveis = c.Partida.Inventario.Keys
                .Select(id => c.Catalogo.BuscarItem(id))
                .Where(item => item != null && RegrasItens.UsavelEm(item, MomentoUso.Conversa))
                .Cast<Item>()
                .ToList();

            c.Saida.Secao("Itens para usar na conversa");
            if (usaveis.Count == 0)
            {
                c.Saida.Texto("Você não tem nenhum item que ajude agora. Passe na Loja!");
                c.Entrada.Pausar();
                return Tela.Conversa;
            }

            for (int i = 0; i < usaveis.Count; i++)
            {
                c.Saida.Texto($"  {i + 1}. {usaveis[i].Nome} (x{c.Partida.Quantidade(usaveis[i].Id)}) — {usaveis[i].Descricao}");
            }
            c.Saida.Texto("  0. Voltar");

            int escolha = c.Entrada.LerOpcao(0, usaveis.Count);
            if (escolha == 0) return Tela.Conversa;

            Item item = usaveis[escolha - 1];
            var resultado = c.RegrasItens.Usar(item.Id, c.Partida, MomentoUso.Conversa, out _);
            TelaInventario.MostrarResultadoUso(c, item, resultado);
            c.Entrada.Pausar();
            return Tela.Conversa;
        }

        private static Tela Convidar(ContextoJogo c, Npc npc)
        {
            var resultado = c.RegrasConversa.Convidar(c.Partida, c.Salvos);

            c.Saida.Texto();
            c.Saida.Narracao("Você aguarda a resposta por alguns minutos depois que a mensagem é visualizada, olhando fixamente para a tela...");
            c.Saida.Narracao("Depois do que pareceram horas de suspense, a resposta finalmente chega:");
            c.Saida.Texto();

            if (resultado.Aceito)
            {
                c.Saida.Fala(npc.Nome, c.Personalizar(npc.ConviteAceito));
                c.Saida.Sucesso("Você conseguiu um encontro!");
                if (resultado.DescontoLiberado != null)
                {
                    var loja = resultado.DescontoLiberado;
                    var nomes = loja.Itens.Select(id => c.Catalogo.BuscarItem(id)?.Nome ?? id);
                    c.Saida.Destaque($"{npc.Nome} é {npc.Flexao("dono", "dona")} de \"{loja.Nome}\"! Desconto permanente de {loja.Desconto}% em: {string.Join(", ", nomes)}.");
                }
            }
            else
            {
                c.Saida.Fala(npc.Nome, c.Personalizar(npc.ConviteRecusado));
                c.Saida.Erro("O convite foi recusado.");
                if (resultado.Motivo == MotivoConvite.InteresseBaixo)
                {
                    c.Saida.Narracao($"(Parece que {npc.Nome} ainda não estava {npc.Flexao("interessado", "interessada")} o suficiente.)");
                }
            }

            return FimDaConversa(c);
        }

        private static Tela Bloquear(ContextoJogo c, Npc npc)
        {
            c.Saida.Texto();
            if (!c.Entrada.LerSimNao($"Tem certeza que deseja bloquear \"{npc.Nome}\"?"))
            {
                return Tela.Conversa;
            }

            RegrasConversa.EncerrarConversa(c.Partida);
            c.Saida.Aviso($"{npc.Nome} foi {npc.Flexao("bloqueado", "bloqueada")}. A conversa usada não volta.");
            return FimDaConversa(c);
        }

        private static Tela VerificarBloqueio(ContextoJogo c, Npc npc, EstadoConversa conversa)
        {
            if (RegrasConversa.FoiBloqueadoPeloNpc(conversa))
            {
                c.Saida.Texto();
                c.Saida.Erro($"Você foi bloqueado(a) por {npc.Nome}! O interesse chegou a {Constantes.InteresseBloqueio}.");
                RegrasConversa.EncerrarConversa(c.Partida);
                return FimDaConversa(c);
            }

            c.Entrada.Pausar();
            return Tela.Conversa;
        }

        private static Tela FimDaConversa(ContextoJogo c)
        {
            c.VerificarNotificacao = true;
            c.Salvar();
            c.Entrada.Pausar();

            return c.Partida.Conversas <= 0 ? FluxoFimDasConversas.Executar(c) : Tela.MenuInicial;
        }

        private static void MostrarVariacao(ContextoJogo c, int variacao)
        {
            string texto = variacao > 0 ? $"(+{variacao} de interesse)" : $"({variacao} de interesse)";
            if (variacao > 0) c.Saida.Sucesso(texto);
            else if (variacao < 0) c.Saida.Erro(texto);
            else c.Saida.Narracao(texto);
        }
    }
}
