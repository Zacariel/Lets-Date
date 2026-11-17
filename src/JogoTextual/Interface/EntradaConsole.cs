using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JogoTextual.Interface
{
    /// <summary>Lançada quando a entrada termina (por exemplo, o terminal foi fechado).</summary>
    public class FimDaEntradaException : Exception
    {
        public FimDaEntradaException() : base("A entrada do console terminou.") { }
    }

    /// <summary>
    /// Leitura validada. Nenhum método devolve um valor inválido: enquanto a entrada
    /// não servir, a mensagem de erro aparece e a mesma pergunta é feita de novo.
    /// </summary>
    public class EntradaConsole
    {
        private readonly TextReader _entrada;
        private readonly SaidaConsole _saida;

        public EntradaConsole(SaidaConsole saida, TextReader? entrada = null)
        {
            _saida = saida;
            _entrada = entrada ?? Console.In;
        }

        private string LerLinha()
        {
            string? linha = _entrada.ReadLine();
            if (linha == null) throw new FimDaEntradaException();
            return linha.Trim();
        }

        /// <summary>Lê um número entre <paramref name="minimo"/> e <paramref name="maximo"/>.</summary>
        public int LerOpcao(int minimo, int maximo, string pergunta = "Escolha: ")
        {
            return LerOpcao(Enumerable.Range(minimo, maximo - minimo + 1), pergunta);
        }

        /// <summary>Lê um dos números da lista de opções válidas.</summary>
        public int LerOpcao(IEnumerable<int> validas, string pergunta = "Escolha: ")
        {
            var conjunto = new HashSet<int>(validas);
            while (true)
            {
                _saida.Escrever(pergunta);
                if (int.TryParse(LerLinha(), out int numero) && conjunto.Contains(numero))
                {
                    return numero;
                }
                _saida.OpcaoInvalida();
            }
        }

        /// <summary>Lê um número inteiro qualquer. Repete enquanto o texto não for um número.</summary>
        public int LerInteiro(string pergunta, string mensagemErro)
        {
            while (true)
            {
                _saida.Escrever(pergunta);
                if (int.TryParse(LerLinha(), out int numero))
                {
                    return numero;
                }
                _saida.Erro(mensagemErro);
            }
        }

        /// <summary>Lê um texto. Pode ser obrigatório e ter tamanho máximo.</summary>
        public string LerTexto(string pergunta, int tamanhoMaximo = int.MaxValue, bool obrigatorio = true)
        {
            while (true)
            {
                _saida.Escrever(pergunta);
                string texto = LerLinha();

                if (obrigatorio && texto.Length == 0)
                {
                    _saida.Erro("Esse campo não pode ficar vazio.");
                    continue;
                }
                if (texto.Length > tamanhoMaximo)
                {
                    _saida.Erro($"Use no máximo {tamanhoMaximo} caracteres (você usou {texto.Length}).");
                    continue;
                }
                return texto;
            }
        }

        /// <summary>Lê SIM ou NÃO (também aceita S, N e NAO, em maiúsculas ou minúsculas).</summary>
        public bool LerSimNao(string pergunta)
        {
            while (true)
            {
                _saida.Escrever(pergunta + " (SIM/NÃO): ");
                string resposta = LerLinha().ToUpperInvariant();
                if (resposta == "SIM" || resposta == "S") return true;
                if (resposta == "NÃO" || resposta == "NAO" || resposta == "N") return false;
                _saida.OpcaoInvalida();
            }
        }

        public void Pausar(string mensagem = "Aperte ENTER para continuar...")
        {
            _saida.Texto();
            _saida.Narracao(mensagem);
            LerLinha();
        }
    }
}
