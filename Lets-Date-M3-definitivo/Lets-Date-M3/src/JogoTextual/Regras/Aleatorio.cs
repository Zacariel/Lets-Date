using System;
using System.Collections.Generic;

namespace JogoTextual.Regras
{
    public interface IAleatorio
    {
        int Entre(int minimo, int maximo);
    }

    public class AleatorioPadrao : IAleatorio
    {
        private readonly Random _random;

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
