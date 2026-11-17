using System;
using System.Collections.Generic;

namespace JogoTextual.Regras
{
    /// <summary>
    /// Fonte de números aleatórios. Fica atrás de uma interface para que os testes
    /// possam escolher o resultado do D20 e do sorteio do convite.
    /// </summary>
    public interface IAleatorio
    {
        /// <summary>Número inteiro entre <paramref name="minimo"/> e <paramref name="maximo"/>, incluindo os dois.</summary>
        int Entre(int minimo, int maximo);
    }

    public class AleatorioPadrao : IAleatorio
    {
        private readonly Random _random;

        /// <summary>Com semente, os sorteios se repetem sempre iguais (útil para demonstrações e testes manuais).</summary>
        public AleatorioPadrao(int? semente = null)
        {
            _random = semente.HasValue ? new Random(semente.Value) : new Random();
        }

        public int Entre(int minimo, int maximo) => _random.Next(minimo, maximo + 1);
    }

    public static class ExtensoesAleatorio
    {
        public static T Escolher<T>(this IAleatorio aleatorio, IReadOnlyList<T> lista)
        {
            return lista[aleatorio.Entre(0, lista.Count - 1)];
        }

        /// <summary>Embaralha uma cópia da lista (algoritmo de Fisher-Yates).</summary>
        public static List<T> Embaralhar<T>(this IAleatorio aleatorio, IEnumerable<T> origem)
        {
            var lista = new List<T>(origem);
            for (int i = lista.Count - 1; i > 0; i--)
            {
                int j = aleatorio.Entre(0, i);
                (lista[i], lista[j]) = (lista[j], lista[i]);
            }
            return lista;
        }
    }
}
