using System;
using System.Collections.Generic;
using System.IO;

namespace JogoTextual.Interface
{
    /// Tudo o que é escrito no console: cabeçalhos, cores, barras e menus.
    /// Recebe um TextWriter para que os testes possam capturar a saída.
    public class SaidaConsole
    {
        public const string MensagemOpcaoInvalida = "Oops! Falha no nosso sistema, esta não é uma opção correspondente! :(";

        private readonly TextWriter _saida;
        private readonly bool _usarConsoleReal;

        public SaidaConsole(TextWriter? saida = null)
        {
            _saida = saida ?? Console.Out;
            _usarConsoleReal = saida == null;
        }

        public void Limpar()
        {
            if (!_usarConsoleReal || Console.IsOutputRedirected) return;
            try { Console.Clear(); } catch (IOException) { /* terminal sem suporte a limpar */ }
        }

        /// ADMITAM, TA BONITINHOO
        public void Cabecalho(string titulo)
        {
            Limpar();
            Colorido(ConsoleColor.Magenta, () =>
            {
                _saida.WriteLine("╔══════════════════════════════════════════════╗");
                _saida.WriteLine($"║  ♥ {Centralizar(titulo.ToUpper(), 39)} ♥ ║");
                _saida.WriteLine("╚══════════════════════════════════════════════╝");
            });
            _saida.WriteLine();
        }

        public void Secao(string titulo)
        {
            _saida.WriteLine();
            Colorido(ConsoleColor.Cyan, () => _saida.WriteLine($"── {titulo} ──"));
        }

        public void Texto(string texto = "") => _saida.WriteLine(texto);

        public void Escrever(string texto) => _saida.Write(texto);

        public void Sucesso(string texto) => Colorido(ConsoleColor.Green, () => _saida.WriteLine(texto));

        public void Erro(string texto) => Colorido(ConsoleColor.Red, () => _saida.WriteLine(texto));

        public void Aviso(string texto) => Colorido(ConsoleColor.Yellow, () => _saida.WriteLine(texto));

        public void Destaque(string texto) => Colorido(ConsoleColor.Cyan, () => _saida.WriteLine(texto));

        public void Narracao(string texto) => Colorido(ConsoleColor.DarkGray, () => _saida.WriteLine(texto));

        public void Fala(string quem, string texto)
        {
            Colorido(ConsoleColor.Magenta, () => _saida.Write($"{quem}: "));
            _saida.WriteLine($"\"{texto}\"");
        }

        public void OpcaoInvalida() => Erro(MensagemOpcaoInvalida);

        /// Barra do tipo [██████░░░░] 60 para interesse e coragem.
        public void Barra(string rotulo, int valor, int minimo, int maximo, ConsoleColor cor)
        {
            const int largura = 20;
            double proporcao = (double)(valor - minimo) / (maximo - minimo);
            int cheios = (int)Math.Round(Math.Clamp(proporcao, 0, 1) * largura);
            _saida.Write($"{rotulo,-10} [");
            Colorido(cor, () => _saida.Write(new string('█', cheios)));
            _saida.Write(new string('░', largura - cheios));
            _saida.WriteLine($"] {valor}");
        }

        public void Menu(IEnumerable<(int numero, string texto)> opcoes)
        {
            foreach (var (numero, texto) in opcoes)
            {
                _saida.WriteLine($"  {numero}. {texto}");
            }
        }

        private void Colorido(ConsoleColor cor, Action escrever)
        {
            if (_usarConsoleReal) Console.ForegroundColor = cor;
            escrever();
            if (_usarConsoleReal) Console.ResetColor();
        }

        private static string Centralizar(string texto, int largura)
        {
            if (texto.Length >= largura) return texto.Substring(0, largura);
            int esquerda = (largura - texto.Length) / 2;
            return texto.PadLeft(texto.Length + esquerda).PadRight(largura);
        }
    }
}
