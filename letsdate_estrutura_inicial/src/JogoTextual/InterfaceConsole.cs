using System;

namespace JogoTextual
{
    public class InterfaceConsole
    {
        public static void MostrarCabecalho(string titulo)
        {
            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine($"   {titulo.ToUpper()}");
            Console.WriteLine("=================================");
            Console.WriteLine(); // Usamos um WriteLine vazio para dar o espaço com segurança, ou usei no caso afinal foi só eu... alguem vai ler isso será?
        }

        public static void MostrarErro(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Oops! {mensagem}");
            Console.ResetColor();
        }
    }
}