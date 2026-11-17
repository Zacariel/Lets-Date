using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// Dificuldade e contagem regressiva até o Dia dos Namorados.
    /// Regras puras (sem console), fáceis de testar.
    public static class RegrasCampanha
    {
        public static int DiasAntes(Dificuldade dificuldade)
        {
            return dificuldade switch
            {
                Dificuldade.Facil => Constantes.DiasAntesFacil,
                Dificuldade.Medio => Constantes.DiasAntesMedio,
                _ => Constantes.DiasAntesDificil
            };
        }

        /// Começa uma campanha nova no dia 1. O último dia é o Dia dos Namorados.
        public static Campanha Nova(Dificuldade dificuldade)
        {
            return new Campanha
            {
                Dificuldade = dificuldade,
                DiasTotais = DiasAntes(dificuldade) + 1,
                DiaAtual = 1
            };
        }

        /// Passa para o próximo dia. Devolve falso se já era o Dia dos Namorados (a campanha acabou).
        public static bool AvancarDia(Campanha campanha)
        {
            if (campanha.EhDiaDosNamorados) return false;
            campanha.DiaAtual++;
            return true;
        }

        public static string DescreverDia(Campanha campanha)
        {
            if (campanha.EhDiaDosNamorados) return "Hoje é o Dia dos Namorados!";
            if (campanha.FaltamDias == 1) return "Falta 1 dia para o Dia dos Namorados!";
            return $"Faltam {campanha.FaltamDias} dias para o Dia dos Namorados.";
        }

        public static string NomeDificuldade(Dificuldade dificuldade)
        {
            return dificuldade switch
            {
                Dificuldade.Facil => "Fácil (2 semanas)",
                Dificuldade.Medio => "Médio (1 semana)",
                _ => "Difícil (no próprio dia)"
            };
        }

        /// Dica mostrada no botão "Chamar para encontro", conforme a dificuldade:
        /// Fácil mostra a chance exata, Médio só diz se pode aceitar, Difícil não mostra nada.
        public static string? DicaConvite(Dificuldade dificuldade, int interesse)
        {
            int chance = RegrasConversa.ChanceDeAceitar(interesse);
            return dificuldade switch
            {
                Dificuldade.Facil => $"chance de aceitar: {chance}%",
                Dificuldade.Medio => chance > 0 ? "pode aceitar" : "ainda não vai aceitar",
                _ => null
            };
        }
    }
}
