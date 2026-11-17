using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// <summary>Escolha do final pela quantidade de encontros conseguidos.</summary>
    public static class RegrasFinais
    {
        public static FinalJogo Determinar(int conversasIniciadas, int encontros)
        {
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

        /// <summary>Registra o final na galeria e encerra o dia (as conversas do dia não ficam guardadas).</summary>
        public static FinalJogo Registrar(DadosSalvos salvos, EstadoPartida partida)
        {
            FinalJogo final = Determinar(partida.ConversasIniciadas, partida.EncontrosMarcados.Count);
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
