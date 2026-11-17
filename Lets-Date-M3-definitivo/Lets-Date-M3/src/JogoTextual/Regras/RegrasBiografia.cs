using System.Globalization;
using System.Text;
using JogoTextual.Dominio;

namespace JogoTextual.Regras
{
    /// Dá uma função para a biografia do jogador: se ela tiver uma das palavras-chave
    /// do NPC (ex.: "cachorro" para a Maria), a conversa começa com +10 de interesse.
    public static class RegrasBiografia
    {
        /// Primeira palavra-chave do NPC que aparece na biografia, ou null se não tiver nenhuma.
        public static string? PalavraEmComum(string biografia, Npc npc)
        {
            if (string.IsNullOrWhiteSpace(biografia)) return null;

            string bio = Normalizar(biografia);
            foreach (string palavra in npc.PalavrasChave)
            {
                if (bio.Contains(Normalizar(palavra))) return palavra;
            }
            return null;
        }

        /// Deixa o texto em minúsculas e sem acentos, para "Música" e "musica" contarem igual.
        /// (FormD separa a letra do acento; depois jogamos os acentos fora.)
        public static string Normalizar(string texto)
        {
            var resultado = new StringBuilder();
            foreach (char letra in texto.ToLowerInvariant().Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(letra);
                }
            }
            return resultado.ToString();
        }
    }
}
