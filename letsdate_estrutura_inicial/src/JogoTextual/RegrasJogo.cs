using System;
using JogoTextual.Dominio;

namespace JogoTextual
{
    public class RegrasJogo
    {
        public static int Flertar(EstadoJogo estado)
        {
            if (estado.Coragem < 25) return -999; // Código de erro para falta de coragem[cite: 62]

            estado.Coragem -= 25;
            int d20 = new Random().Next(1, 21);
            int resultadoInteresse = (d20 - 10) * 10;

            AlterarInteresse(estado, resultadoInteresse);
            return resultadoInteresse;
        }

        public static void AlterarInteresse(EstadoJogo estado, int valor)
        {
            estado.InteresseAtualNpc += valor;
            if (estado.InteresseAtualNpc > 100) estado.InteresseAtualNpc = 100;
            if (estado.InteresseAtualNpc < -100) estado.InteresseAtualNpc = -100;
        }

        public static bool ChamarParaEncontro(EstadoJogo estado)
        {
            if (estado.InteresseAtualNpc < 60) return false;

            int chance = new Random().Next(1, 101);
            if (chance <= estado.InteresseAtualNpc)
            {
                if (estado.NpcAtual != null)
                {
                    estado.NpcsComEncontroMarcado.Add(estado.NpcAtual);
                }
                return true;
            }
            return false;
        }

        public static int CalcularSatisfacaoEncontro(Npc npc, string roupa, string presente, string local, int totalEncontrosMarcados)
        {
            int satisfacao = 70; // Inicia em 70[cite: 60]

            satisfacao += (roupa == npc.VestuarioPreferido) ? 10 : -10;
            satisfacao += (presente == npc.PresentePreferido) ? 10 : -10;
            satisfacao += (local == npc.LocalPreferido) ? 10 : -10;

            // Punição de -10 para CADA encontro a mais[cite: 63]
            if (totalEncontrosMarcados > 1)
            {
                int punicao = (totalEncontrosMarcados - 1) * 10;
                satisfacao -= punicao;
            }

            if (satisfacao > 100) satisfacao = 100;
            if (satisfacao < 0) satisfacao = 0;

            return satisfacao;
        }
    }
}