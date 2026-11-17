using System;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// Início e fim de um dia (partida).
    public static class RegrasDia
    {
        /// Começa um novo dia: 3 conversas mais as que foram guardadas ao sair do
        /// aplicativo, até o máximo de 4. Coragem, carteira e encontros recomeçam.
        public static EstadoPartida NovoDia(DadosSalvos salvos)
        {
            var partida = new EstadoPartida
            {
                Conversas = Math.Min(Constantes.ConversasMaximas, Constantes.ConversasIniciais + salvos.ConversasGuardadas)
            };

            salvos.ConversasGuardadas = 0;
            salvos.DiasJogados++;
            return partida;
        }

        //"Sair do aplicativo": guarda as conversas que sobraram para o próximo dia.
        public static void GuardarConversas(DadosSalvos salvos, EstadoPartida partida)
        {
            salvos.ConversasGuardadas = Math.Max(0, partida.Conversas);
        }
    }
}
