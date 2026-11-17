using System;
using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// <summary>Histórico de avaliações (estrelas) e galeria dos finais já desbloqueados.</summary>
    public class TelaAvaliacoes : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Avaliações e finais");

            c.Saida.Secao("Seus encontros");
            if (c.Salvos.Avaliacoes.Count == 0)
            {
                c.Saida.Texto("Você ainda não foi a nenhum encontro.");
            }
            else
            {
                foreach (var registro in c.Salvos.Avaliacoes.OrderByDescending(r => r.Dia))
                {
                    c.Saida.Aviso($"  Dia {registro.Dia}: {registro.NomeNpc,-18} {RegrasEncontro.Estrelas(registro.Avaliacao)} ({registro.Satisfacao} pts)");
                }
                double media = c.Salvos.Avaliacoes.Average(r => r.Satisfacao);
                c.Saida.Texto($"  Satisfação média: {media:0}");
            }

            int total = Enum.GetValues(typeof(FinalJogo)).Length;
            c.Saida.Secao($"Finais desbloqueados ({c.Salvos.FinaisDesbloqueados.Count}/{total})");
            foreach (FinalJogo final in Enum.GetValues(typeof(FinalJogo)))
            {
                bool desbloqueado = c.Salvos.FinaisDesbloqueados.Contains(final);
                string titulo = desbloqueado ? c.Catalogo.BuscarFinal(final)?.Titulo ?? final.ToString() : "???";
                c.Saida.Texto($"  Final {(int)final} — {titulo}");
            }

            c.Saida.Texto();
            c.Saida.Texto($"Dias jogados: {c.Salvos.DiasJogados}");
            c.Entrada.Pausar();
            return Tela.MenuInicial;
        }
    }
}
