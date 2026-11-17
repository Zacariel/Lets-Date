using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    ///Escolha do final pela quantidade de encontros conseguidos.
    public static class RegrasFinais
    {
        /// NOVO: se o namoro foi confirmado no Dia dos Namorados e esse foi o ÚNICO encontro da noite,
        /// o final é o 7. (Namorar e sair com mais gente na mesma noite continua dando os finais 4 a 6.)
        public static FinalJogo Determinar(int conversasIniciadas, int encontros, bool namoroConfirmado = false)
        {
            if (namoroConfirmado && encontros == 1) return FinalJogo.FelizesParaSempre; // NOVO

            if (encontros == 0)
            {
                return conversasIniciadas == 0 ? FinalJogo.MelhorNemTentar : FinalJogo.NaoFoiDessaVez;
            }

            return encontros switch
            {
                1 => FinalJogo.AlmasGemeas,
                2 => FinalJogo.CalmaAmor,
                3 => FinalJogo.SaoMuitasNamoradas,
                _ => FinalJogo.NaoDeuContaDoRecado
            };
        }

        ///Registra o final na galeria e encerra o dia (as conversas do dia não ficam guardadas).
        public static FinalJogo Registrar(DadosSalvos salvos, EstadoPartida partida)
        {
            FinalJogo final = Determinar(partida.ConversasIniciadas, partida.EncontrosMarcados.Count, partida.NamoroConfirmado);
            partida.FinalAlcancado = final;
            salvos.ConversasGuardadas = 0;
            if (!salvos.FinaisDesbloqueados.Contains(final))
            {
                salvos.FinaisDesbloqueados.Add(final);
                salvos.FinaisDesbloqueados.Sort();
            }
            return final;
        }
    }
}
