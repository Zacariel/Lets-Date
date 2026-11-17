using System;
using System.Text;
using JogoTextual.Dados;
using JogoTextual.Interface;
using JogoTextual.Persistencia;
using JogoTextual.Regras;
using JogoTextual.Telas;

namespace JogoTextual
{
    public static class Program
    {
        public static int Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var saida = new SaidaConsole();
            var entrada = new EntradaConsole(saida);

            CatalogoJogo catalogo;
            try
            {
                catalogo = CarregadorCatalogo.Carregar();
            }
            catch (ErroCatalogo erro)
            {
                saida.Erro("Não foi possível abrir o Let's Date: há problemas nos arquivos da pasta Dados.");
                foreach (var problema in erro.Problemas) saida.Texto($" - {problema}");
                return 1;
            }

            var persistencia = new PersistenciaJogo();
            var salvos = persistencia.Carregar(out bool corrompido);
            if (corrompido)
            {
                saida.Aviso($"O arquivo de save estava corrompido. Uma cópia foi guardada em {persistencia.Caminho}.bak e um perfil novo será criado.");
            }

            var contexto = new ContextoJogo(catalogo, saida, entrada, CriarAleatorio(), persistencia, salvos);

            try
            {
                new Jogo(contexto).Iniciar();
            }
            catch (FimDaEntradaException)
            {
                // O terminal foi fechado ou a entrada acabou: guarda o progresso e sai sem erro.
                contexto.Salvar();
            }

            return 0;
        }

        /// <summary>
        /// A variável de ambiente LETSDATE_SEMENTE fixa os sorteios (NPCs, D20, convite e notificações),
        /// para repetir exatamente a mesma partida numa demonstração.
        /// </summary>
        private static IAleatorio CriarAleatorio()
        {
            string? semente = Environment.GetEnvironmentVariable("LETSDATE_SEMENTE");
            return int.TryParse(semente, out int valor) ? new AleatorioPadrao(valor) : new AleatorioPadrao();
        }
    }
}
