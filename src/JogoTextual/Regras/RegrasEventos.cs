using System;
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dados;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// <summary>Notificações do celular que chegam entre uma conversa e outra.</summary>
    public class RegrasEventos
    {
        private readonly CatalogoJogo _catalogo;
        private readonly IAleatorio _aleatorio;

        public RegrasEventos(CatalogoJogo catalogo, IAleatorio aleatorio)
        {
            _catalogo = catalogo;
            _aleatorio = aleatorio;
        }

        /// <summary>Com 50% de chance, sorteia uma notificação. Devolve null quando não chega nada.</summary>
        public EventoCelular? TalvezSortear()
        {
            if (_catalogo.Eventos.Count == 0) return null;
            if (_aleatorio.Entre(1, 100) > Constantes.ChanceEvento) return null;
            return _aleatorio.Escolher(_catalogo.Eventos);
        }

        /// <summary>Aplica o efeito da notificação e devolve uma frase explicando o que mudou.</summary>
        public string Aplicar(EventoCelular evento, EstadoPartida partida)
        {
            switch (evento.Tipo)
            {
                case TipoEvento.Dinheiro:
                    int antes = partida.Dinheiro;
                    partida.Dinheiro = Math.Max(0, partida.Dinheiro + evento.Valor);
                    int diferenca = partida.Dinheiro - antes;
                    return diferenca >= 0 ? $"+R$ {diferenca} na carteira." : $"-R$ {-diferenca} na carteira.";

                case TipoEvento.Coragem:
                    int coragemAntes = partida.Coragem;
                    partida.Coragem = Math.Clamp(partida.Coragem + evento.Valor, 0, Constantes.CoragemMaxima);
                    int variacao = partida.Coragem - coragemAntes;
                    return variacao >= 0 ? $"+{variacao} de coragem." : $"{variacao} de coragem.";

                case TipoEvento.Cupom:
                    partida.CupomDesconto = Math.Max(partida.CupomDesconto, evento.Valor);
                    return $"Cupom de {partida.CupomDesconto}% guardado para a sua próxima compra na loja.";

                case TipoEvento.Dica:
                    return RevelarDica(partida);

                default:
                    return "Nada mudou.";
            }
        }

        /// <summary>Anota uma preferência ainda desconhecida de alguém com encontro marcado.</summary>
        private string RevelarDica(EstadoPartida partida)
        {
            var candidatas = new List<(Npc npc, string dica)>();
            foreach (var npcId in partida.EncontrosMarcados)
            {
                Npc? npc = _catalogo.BuscarNpc(npcId);
                if (npc == null) continue;
                foreach (var dica in DicasDePreferencia(npc))
                {
                    if (!partida.Anotacoes.TryGetValue(npc.Id, out var notas) || !notas.Contains(dica))
                        candidatas.Add((npc, dica));
                }
            }

            if (candidatas.Count == 0)
            {
                return "Mas não tinha nenhuma informação nova sobre os seus encontros.";
            }

            var escolhida = _aleatorio.Escolher(candidatas);
            partida.Anotar(escolhida.npc.Id, escolhida.dica);
            return $"Anotado sobre {escolhida.npc.Nome}: {escolhida.dica}";
        }

        public IEnumerable<string> DicasDePreferencia(Npc npc)
        {
            var roupa = _catalogo.BuscarRoupa(npc.Preferencias.Roupa);
            var presente = _catalogo.BuscarItem(npc.Preferencias.Presente);
            var local = _catalogo.BuscarLocal(npc.Preferencias.Local);
            return new[]
            {
                roupa != null ? $"curte o estilo {roupa.Nome}." : null,
                presente != null ? $"adoraria ganhar {presente.Nome}." : null,
                local != null ? $"prefere sair para: {local.Nome}." : null
            }.OfType<string>();
        }
    }
}
