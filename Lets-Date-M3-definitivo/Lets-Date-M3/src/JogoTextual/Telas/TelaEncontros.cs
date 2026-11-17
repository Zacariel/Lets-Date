using System.Linq;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    public class TelaEncontros : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            var marcados = c.Partida.EncontrosMarcados
                .Select(id => c.Catalogo.BuscarNpc(id))
                .Where(npc => npc != null)
                .Cast<Npc>()
                .ToList();

            // em dias comuns da campanha a noite termina no resumo do dia, não num final.
            Campanha? campanha = c.Salvos.Campanha;
            bool ehDiaDosNamorados = campanha == null || campanha.EhDiaDosNamorados;
            Tela depois = ehDiaDosNamorados ? Tela.Final : Tela.FimDoDia;

            // Sem encontros marcados: direto para os finais 1 ou 2 (ou para o fim do dia).
            if (marcados.Count == 0) return depois;

            c.Saida.Cabecalho("Hora do encontro");
            c.Saida.Texto(ehDiaDosNamorados
                ? "Após algumas horas no aplicativo buscando por encontros, finalmente chegou o momento de começar a noite, será o seu melhor dia dos namorados!"
                : "O dia passou voando no aplicativo. Ainda não é o Dia dos Namorados, mas cada encontro conta. Hora de se arrumar!");
            c.Saida.Texto();
            c.Saida.Texto($"Encontros marcados: {string.Join(", ", marcados.Select(n => n.Nome))}");

            int penalidade = RegrasEncontro.Penalidade(marcados.Count);
            if (penalidade > 0)
            {
                c.Saida.Aviso($"Atenção: com {marcados.Count} encontros na mesma noite, cada um perde {penalidade} pontos de satisfação pela correria.");
            }
            c.Entrada.Pausar();

            var leitor = new LeitorEscolhasEncontro(c);
            for (int i = 0; i < marcados.Count; i++)
            {
                RealizarEncontro(c, leitor, marcados[i], i + 1, marcados.Count);
            }

            return depois;
        }

        private static void RealizarEncontro(ContextoJogo c, LeitorEscolhasEncontro leitor, Npc npc, int numero, int total)
        {
            // Relacionamento atual com esa pessoa (null se é o primeiro encontro).
            Campanha? campanha = c.Salvos.Campanha;
            Relacionamento? rel = RegrasRelacionamento.Buscar(campanha, npc.Id);
            int nivelAntes = rel?.Nivel ?? 0;

            c.Saida.Cabecalho($"Encontro {numero}/{total}: {npc.Nome}");
            c.Saida.Narracao(npc.Biografia);

            if (rel != null && rel.EncontrosRealizados > 0)
            {
                c.Saida.Destaque($"{RegrasRelacionamento.NomeDoNivel(nivelAntes)}: este é o {rel.EncontrosRealizados + 1}º encontro de vocês. Repetir presente ou lugar perde pontos!");
            }

            if (c.Partida.Anotacoes.TryGetValue(npc.Id, out var notas) && notas.Count > 0)
            {
                c.Saida.Secao($"Suas anotações sobre {npc.Nome}");
                foreach (var nota in notas) c.Saida.Texto($" • {nota}");
            }
            else
            {
                c.Saida.Narracao("(Você não anotou nada sobre as preferências. Confie na biografia!)");
            }

            EscolhasEncontro escolhas = leitor.Ler();
            int bonus = UsarItensDeSatisfacao(c);
            int repeticao = RegrasRelacionamento.PenalidadeRepeticao(rel, escolhas.PresenteId, escolhas.LocalId); 

            ResultadoEncontro resultado = RegrasEncontro.Calcular(
                npc, escolhas.RoupaId, escolhas.PresenteId, escolhas.LocalId, total, bonus,
                npc.PreferenciasDoNivel(nivelAntes), repeticao); // gostos do capítulo + repetição

            MostrarResultado(c, npc, escolhas, resultado);

            if (campanha != null) AtualizarRelacionamento(c, campanha, npc, escolhas, resultado, nivelAntes); 

            c.Salvos.Avaliacoes.Add(new RegistroAvaliacao
            {
                Dia = c.Salvos.DiasJogados,
                NomeNpc = npc.Nome,
                Satisfacao = resultado.Satisfacao,
                Avaliacao = resultado.Avaliacao
            });
            c.Salvar();
            c.Entrada.Pausar();
        }

        ///  sobe ou desce o nível do relacionamento e, no Dia dos Namorados, verifica o pedido de namoro.
        private static void AtualizarRelacionamento(ContextoJogo c, Campanha campanha, Npc npc,
                                                    EscolhasEncontro escolhas, ResultadoEncontro resultado, int nivelAntes)
        {
            Relacionamento rel = RegrasRelacionamento.RegistrarEncontro(campanha, npc.Id, resultado, escolhas.PresenteId, escolhas.LocalId);
            string antes = RegrasRelacionamento.NomeDoNivel(nivelAntes);
            string agora = RegrasRelacionamento.NomeDoNivel(rel.Nivel);

            c.Saida.Texto();
            if (rel.Nivel > nivelAntes)
                c.Saida.Sucesso($"Relacionamento com {npc.Nome}: {antes} → {agora}!");
            else if (rel.Nivel < nivelAntes)
                c.Saida.Erro($"O clima esfriou... relacionamento com {npc.Nome}: {antes} → {agora}.");
            else if (rel.Nivel == 0)
                c.Saida.Narracao($"Foi legal, mas não rolou match com {npc.Nome}.");
            else
                c.Saida.Narracao($"O relacionamento com {npc.Nome} continua em \"{agora}\".");

            if (campanha.EhDiaDosNamorados && RegrasRelacionamento.ConfirmaNamoro(rel, resultado))
            {
                c.Partida.NamoroConfirmado = true;
                c.Saida.Destaque($"{npc.Nome} segura sua mão e pergunta: \"Quer namorar comigo?\" ♥");
            }
        }

        /// Oferece os itens de satisfação (perfume) que o jogador tiver. Devolve o bônus total ;)
        private static int UsarItensDeSatisfacao(ContextoJogo c)
        {
            int bonus = 0;
            var itens = c.Partida.Inventario.Keys
                .Select(id => c.Catalogo.BuscarItem(id))
                .Where(item => item != null && RegrasItens.UsavelEm(item, MomentoUso.Encontro))
                .Cast<Item>()
                .ToList();

            foreach (var item in itens)
            {
                c.Saida.Texto();
                if (c.Entrada.LerSimNao($"Usar {item.Nome} neste encontro? ({item.Descricao})"))
                {
                    c.RegrasItens.Usar(item.Id, c.Partida, MomentoUso.Encontro, out int bonusItem);
                    bonus += bonusItem;
                }
            }
            return bonus;
        }

        private static void MostrarResultado(ContextoJogo c, Npc npc, EscolhasEncontro escolhas, ResultadoEncontro r)
        {
            string roupa = c.Catalogo.BuscarRoupa(escolhas.RoupaId)?.Nome ?? escolhas.RoupaId;
            string presente = escolhas.PresenteId == null ? "sem presente" : c.Catalogo.BuscarItem(escolhas.PresenteId)?.Nome ?? escolhas.PresenteId;
            string local = c.Catalogo.BuscarLocal(escolhas.LocalId)?.Nome ?? escolhas.LocalId;

            c.Saida.Secao("Como foi a noite");
            Linha(c, "Roupa", roupa, r.RoupaCerta);
            Linha(c, "Presente", presente, r.PresenteCerto);
            Linha(c, "Local", local, r.LocalCerto);
            if (r.PenalidadeEncontrosExtras > 0) c.Saida.Erro($"  Correria entre encontros: -{r.PenalidadeEncontrosExtras}");
            if (r.PenalidadeRepeticao > 0) c.Saida.Erro($"  Repetiu presente ou lugar de outro encontro: -{r.PenalidadeRepeticao}"); 
            if (r.Bonus > 0) c.Saida.Sucesso($"  Itens especiais: +{r.Bonus}");

            c.Saida.Texto();
            c.Saida.Aviso($"{RegrasEncontro.Mensagem(r.Avaliacao, npc.Nome)} {RegrasEncontro.Estrelas(r.Avaliacao)} ({r.Satisfacao} pontos de satisfação)");
        }

        private static void Linha(ContextoJogo c, string rotulo, string escolha, bool acertou)
        {
            string texto = $"  {rotulo,-9} {escolha,-22} {(acertou ? "+10" : "-10")}";
            if (acertou) c.Saida.Sucesso(texto);
            else c.Saida.Erro(texto);
        }
    }
}
