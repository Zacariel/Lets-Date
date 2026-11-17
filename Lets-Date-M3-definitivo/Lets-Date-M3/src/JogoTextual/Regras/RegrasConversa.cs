using System;
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dados;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// Regras de "Buscar encontro!": conversar, flertar, convidar e bloquear.
    public class RegrasConversa
    {
        private readonly CatalogoJogo _catalogo;
        private readonly IAleatorio _aleatorio;

        public RegrasConversa(CatalogoJogo catalogo, IAleatorio aleatorio)
        {
            _catalogo = catalogo;
            _aleatorio = aleatorio;
        }

        /// NPCs compatíveis com a busca do jogador que ainda não apareceram hoje.
        /// quem já é match fica fora do sorteio (aparece em "Meus matches").
        public List<Npc> NpcsDisponiveis(EstadoPartida partida, PerfilJogador perfil, Campanha? campanha = null)
        {
            var matches = RegrasRelacionamento.MatchesAtivos(campanha);
            return _catalogo.NpcsCompativeis(perfil.Busca)
                .Where(n => !partida.NpcsVistos.Contains(n.Id))
                .Where(n => !matches.Contains(n.Id))
                .ToList();
        }

        /// Sorteia um NPC que ainda não apareceu e gasta uma conversa.
        /// Devolve null se não houver conversas ou NPCs disponíveis (nada é gasto).
        public Npc? IniciarConversa(EstadoPartida partida, PerfilJogador perfil, Campanha? campanha = null)
        {
            if (partida.Conversas <= 0) return null;

            var disponiveis = NpcsDisponiveis(partida, perfil, campanha);
            if (disponiveis.Count == 0) return null;

            Npc npc = _aleatorio.Escolher(disponiveis);
            partida.Conversas--;
            partida.ConversasIniciadas++;
            partida.NpcsVistos.Add(npc.Id);
            partida.ConversaAtual = new EstadoConversa { NpcId = npc.Id };
            return npc;
        }

        /// conversa com alguém que já é match. Gasta uma conversa, mas começa com mais
        /// interesse e no capítulo da história que corresponde ao nível do relacionamento.
        /// Devolve falso se não houver conversas ou se já falaram hoje.
        public static bool IniciarConversaComMatch(EstadoPartida partida, Relacionamento relacionamento)
        {
            if (partida.Conversas <= 0) return false;
            if (partida.NpcsVistos.Contains(relacionamento.NpcId)) return false;

            partida.Conversas--;
            partida.ConversasIniciadas++;
            partida.NpcsVistos.Add(relacionamento.NpcId);
            partida.ConversaAtual = new EstadoConversa
            {
                NpcId = relacionamento.NpcId,
                Interesse = RegrasRelacionamento.InteresseInicial(relacionamento),
                Capitulo = relacionamento.Nivel
            };
            return true;
        }

        public static int VariacaoDe(Qualidade qualidade)
        {
            return qualidade switch
            {
                Qualidade.Bom => Constantes.VariacaoBom,
                Qualidade.Regular => Constantes.VariacaoRegular,
                Qualidade.Ruim => Constantes.VariacaoRuim,
                _ => Constantes.VariacaoPessimo
            };
        }

        /// Soma a variação ao interesse, sem passar de 100 nem de -100.
        public static void AlterarInteresse(EstadoConversa conversa, int variacao)
        {
            conversa.Interesse = Math.Clamp(conversa.Interesse + variacao, Constantes.InteresseMinimo, Constantes.InteresseMaximo);
        }

        /// Próximo assunto da conversa, ou null se os assuntos já acabaram.
        ///usa os assuntos do capítulo da conversa (se o NPC não tiver capítulo, usa os normais).
        public static TopicoDialogo? TopicoAtual(EstadoConversa conversa, Npc npc)
        {
            var topicos = npc.TopicosDoNivel(conversa.Capitulo);
            return conversa.ProximoTopico < topicos.Count ? topicos[conversa.ProximoTopico] : null;
        }

        /// Aplica a resposta escolhida: muda o interesse, avança o assunto e anota a dica (se houver).
        public int Responder(EstadoPartida partida, Npc npc, OpcaoDialogo opcao)
        {
            var conversa = partida.ConversaAtual ?? throw new InvalidOperationException("Não há conversa em andamento.");
            int variacao = VariacaoDe(opcao.Qualidade);
            AlterarInteresse(conversa, variacao);
            conversa.ProximoTopico++;

            if (!string.IsNullOrWhiteSpace(opcao.Dica))
            {
                partida.Anotar(npc.Id, opcao.Dica!);
            }

            return variacao;
        }

        /// Flerte: custa 25 de coragem e rola um D20. Variação = (resultado - 10) x 10.
        public ResultadoFlerte Flertar(EstadoPartida partida)
        {
            var conversa = partida.ConversaAtual ?? throw new InvalidOperationException("Não há conversa em andamento.");

            if (partida.Coragem < Constantes.CustoFlerte)
            {
                return new ResultadoFlerte { Permitido = false };
            }

            partida.Coragem -= Constantes.CustoFlerte;
            int dado = _aleatorio.Entre(1, 20);
            int variacao = (dado - 10) * 10;
            AlterarInteresse(conversa, variacao);

            return new ResultadoFlerte
            {
                Permitido = true,
                Dado = dado,
                Variacao = variacao,
                Fala = _aleatorio.Escolher(_catalogo.Flertes)
            };
        }

        /// Convite para o encontro. Abaixo de 60 de interesse a resposta é sempre negativa.
        /// A partir de 60, sorteia um número de 1 a 100: aceita se o número for menor ou igual ao interesse.
        /// A conversa termina em qualquer caso.
        public ResultadoConvite Convidar(EstadoPartida partida, DadosSalvos salvos)
        {
            var conversa = partida.ConversaAtual ?? throw new InvalidOperationException("Não há conversa em andamento.");
            var resultado = new ResultadoConvite();

            if (conversa.Interesse < Constantes.InteresseMinimoConvite)
            {
                resultado.Motivo = MotivoConvite.InteresseBaixo;
            }
            else
            {
                resultado.Sorteio = _aleatorio.Entre(1, 100);
                resultado.Motivo = resultado.Sorteio <= conversa.Interesse
                    ? MotivoConvite.Aceito
                    : MotivoConvite.RecusadoNoSorteio;
            }

            if (resultado.Aceito)
            {
                partida.EncontrosMarcados.Add(conversa.NpcId);

                Npc? npc = _catalogo.BuscarNpc(conversa.NpcId);
                if (npc?.Loja != null && !salvos.DescontosDesbloqueados.Contains(npc.Id))
                {
                    salvos.DescontosDesbloqueados.Add(npc.Id);
                    resultado.DescontoLiberado = npc.Loja;
                }
            }

            EncerrarConversa(partida);
            return resultado;
        }

        /// Chance (em %) de o NPC aceitar o convite com o interesse atual.
        public static int ChanceDeAceitar(int interesse)
        {
            return interesse < Constantes.InteresseMinimoConvite ? 0 : Math.Min(100, interesse);
        }

        /// Bloqueio automático quando o interesse chega a -60.
        public static bool FoiBloqueadoPeloNpc(EstadoConversa conversa)
        {
            return conversa.Interesse <= Constantes.InteresseBloqueio;
        }

        public static void EncerrarConversa(EstadoPartida partida)
        {
            partida.ConversaAtual = null;
        }
    }
}
