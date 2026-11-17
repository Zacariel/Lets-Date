
using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// "Meus matches": conversar de novo com quem já saiu com você.
    /// Gasta uma conversa, como "Buscar encontro!", mas começa com mais interesse
    /// e com os assuntos do próximo capítulo da história da pessoa.
    public class TelaMatches : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Meus matches");

            List<Relacionamento> matches = c.Salvos.Campanha == null
                ? new List<Relacionamento>()
                : c.Salvos.Campanha.Relacionamentos.Values.Where(r => r.Nivel > 0).ToList();

            if (matches.Count == 0)
            {
                c.Saida.Texto("Você ainda não tem matches.");
                c.Saida.Narracao("Saia com alguém e faça o encontro ser bom (4 ou 5 estrelas) para virar match!");
                c.Entrada.Pausar();
                return Tela.MenuInicial;
            }

            c.Saida.Texto($"Conversas disponíveis: {c.Partida.Conversas}");
            c.Saida.Texto();

            var validas = new List<int> { 0 };
            for (int i = 0; i < matches.Count; i++)
            {
                Relacionamento r = matches[i];
                string nome = c.Catalogo.BuscarNpc(r.NpcId)?.Nome ?? r.NpcId;
                bool jaFalouHoje = c.Partida.NpcsVistos.Contains(r.NpcId);

                string linha = $" {i + 1}. {nome} [{RegrasRelacionamento.NomeDoNivel(r.Nivel)}] " +
                               $"{r.EncontrosRealizados} encontro(s), último com {r.UltimaSatisfacao} pontos";
                if (jaFalouHoje) linha += " (vocês já conversaram hoje)";
                else validas.Add(i + 1);

                c.Saida.Texto(linha);
            }
            c.Saida.Texto(" 0. Voltar ao menu");

            int escolha = c.Entrada.LerOpcao(validas);
            if (escolha == 0) return Tela.MenuInicial;

            if (c.Partida.Conversas <= 0)
            {
                return FluxoFimDasConversas.Executar(c);
            }

            RegrasConversa.IniciarConversaComMatch(c.Partida, matches[escolha - 1]);
            c.Salvar();
            return Tela.Conversa;
        }
    }
}
