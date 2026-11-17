using System;
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dados;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// <summary>Regras de "Buscar encontro!": conversar, flertar, convidar e bloquear.</summary>
    public class RegrasConversa
    {
        private readonly CatalogoJogo _catalogo;
        private readonly IAleatorio _aleatorio;

        public RegrasConversa(CatalogoJogo catalogo, IAleatorio aleatorio)
        {
            _catalogo = catalogo;
            _aleatorio = aleatorio;
        }

        /// <summary>NPCs compatíveis com a busca do jogador que ainda não apareceram hoje.</summary>
        public List<Npc> NpcsDisponiveis(EstadoPartida partida, PerfilJogador perfil)
        {
            return _catalogo.NpcsCompativeis(perfil.Busca)
                .Where(n => !partida.NpcsVistos.Contains(n.Id))
                .ToList();
        }

        /// <summary>
        /// Sorteia um NPC que ainda não apareceu e gasta uma conversa.
        /// Devolve null se não houver conversas ou NPCs disponíveis (nada é gasto).
        /// </summary>
        public Npc? IniciarConversa(EstadoPartida partida, PerfilJogador perfil)
        {
            if (partida.Conversas <= 0) return null;

            var disponiveis = NpcsDisponiveis(partida, perfil);
            if (disponiveis.Count == 0) return null;

            Npc npc = _aleatorio.Escolher(disponiveis);
            partida.Conversas--;
            partida.ConversasIniciadas++;
            partida.NpcsVistos.Add(npc.Id);
            partida.ConversaAtual = new EstadoConversa { NpcId = npc.Id };
            return npc;
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

        /// <summary>Soma a variação ao interesse, sem passar de 100 nem de -100.</summary>
        public static void AlterarInteresse(EstadoConversa conversa, int variacao)
        {
            conversa.Interesse = Math.Clamp(conversa.Interesse + variacao, Constantes.InteresseMinimo, Constantes.InteresseMaximo);
        }

        /// <summary>Próximo assunto da conversa, ou null se os assuntos já acabaram.</summary>
        public static TopicoDialogo? TopicoAtual(EstadoConversa conversa, Npc npc)
        {
            return conversa.ProximoTopico < npc.Topicos.Count ? npc.Topicos[conversa.ProximoTopico] : null;
        }

        /// <summary>Aplica a resposta escolhida: muda o interesse, avança o assunto e anota a dica (se houver).</summary>
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

        /// <summary>
        /// Flerte: custa 25 de coragem e rola um D20. Variação = (resultado - 10) x 10.
        /// </summary>
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

        /// <summary>
        /// Convite para o encontro. Abaixo de 60 de interesse a resposta é sempre negativa.
        /// A partir de 60, sorteia um número de 1 a 100: aceita se o número for menor ou igual ao interesse.
        /// A conversa termina em qualquer caso.
        /// </summary>
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

        /// <summary>Chance (em %) de o NPC aceitar o convite com o interesse atual.</summary>
        public static int ChanceDeAceitar(int interesse)
        {
            return interesse < Constantes.InteresseMinimoConvite ? 0 : Math.Min(100, interesse);
        }

        /// <summary>Bloqueio automático quando o interesse chega a -60.</summary>
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
