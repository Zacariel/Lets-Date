
using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Fim de um dia comum da campanha (antes do Dia dos Namorados).
    /// Mostra um resumo, avança a contagem e deixa o jogador seguir ou fechar o app (fica salvo).
    public class TelaFimDoDia : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            Campanha? campanha = c.Salvos.Campanha;
            if (campanha == null) return Tela.Final;

            c.Saida.Cabecalho($"Fim do dia {campanha.DiaAtual}");
            c.Saida.Texto($"Conversas iniciadas hoje: {c.Partida.ConversasIniciadas}");
            c.Saida.Texto($"Encontros hoje: {c.Partida.EncontrosMarcados.Count}");

            var matches = campanha.Relacionamentos.Values.Where(r => r.Nivel > 0).ToList();
            if (matches.Count > 0)
            {
                c.Saida.Secao("Seus matches");
                foreach (var r in matches)
                {
                    string nome = c.Catalogo.BuscarNpc(r.NpcId)?.Nome ?? r.NpcId;
                    c.Saida.Texto($" • {nome}: {RegrasRelacionamento.NomeDoNivel(r.Nivel)}");
                }
            }

            RegrasCampanha.AvancarDia(campanha);
            c.Salvar();

            c.Saida.Texto();
            c.Saida.Destaque($"Um novo dia começa... {RegrasCampanha.DescreverDia(campanha)}");
            c.Saida.Menu(new[]
            {
                (1, "Começar o próximo dia"),
                (0, "Fechar o aplicativo (o progresso fica salvo)")
            });

            if (c.Entrada.LerOpcao(new[] { 1, 0 }) == 0) return Tela.Encerrar;

            c.Partida = RegrasDia.NovoDia(c.Salvos);
            c.Salvar();
            return Tela.MenuInicial;
        }
    }
}
