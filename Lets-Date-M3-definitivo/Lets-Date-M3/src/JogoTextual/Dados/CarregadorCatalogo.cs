using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JogoTextual.Dominio;

namespace JogoTextual.Dados
{
    /// erro de conteúdoalgum JSON está faltando, quebrado ou com dados inconsistentes.
    public class ErroCatalogo : Exception
    {
        public List<string> Problemas { get; }

        public ErroCatalogo(List<string> problemas)
            : base("Problemas nos arquivos de dados:\n - " + string.Join("\n - ", problemas))
        {
            Problemas = problemas;
        }
    }

    /// Lê os arquivos JSON da pasta Dados e confere se o conteúdo está consistente.
    public static class CarregadorCatalogo
    {
        public static readonly JsonSerializerOptions OpcoesJson = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// Pasta padrão: Dados, ao lado do executável.
        public static string PastaPadrao => Path.Combine(AppContext.BaseDirectory, "Dados");

        public static CatalogoJogo Carregar(string? pasta = null)
        {
            pasta ??= PastaPadrao;
            var problemas = new List<string>();

            var catalogo = new CatalogoJogo
            {
                Npcs = Ler<List<Npc>>(pasta, "npcs.json", problemas) ?? new List<Npc>(),
                Itens = Ler<List<Item>>(pasta, "itens.json", problemas) ?? new List<Item>(),
                Encontro = Ler<OpcoesEncontro>(pasta, "encontro.json", problemas) ?? new OpcoesEncontro(),
                Flertes = Ler<List<string>>(pasta, "flertes.json", problemas) ?? new List<string>(),
                Eventos = Ler<List<EventoCelular>>(pasta, "eventos.json", problemas) ?? new List<EventoCelular>(),
                Finais = Ler<List<TextoFinal>>(pasta, "finais.json", problemas) ?? new List<TextoFinal>()
            };

            if (problemas.Count == 0)
            {
                problemas.AddRange(Validar(catalogo));
            }

            if (problemas.Count > 0)
            {
                throw new ErroCatalogo(problemas);
            }

            return catalogo;
        }

        private static T? Ler<T>(string pasta, string arquivo, List<string> problemas) where T : class
        {
            string caminho = Path.Combine(pasta, arquivo);
            if (!File.Exists(caminho))
            {
                problemas.Add($"Arquivo não encontrado: {caminho}");
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(caminho), OpcoesJson);
            }
            catch (JsonException erro)
            {
                problemas.Add($"{arquivo}: JSON inválido perto da linha {erro.LineNumber + 1} ({erro.Message})");
                return null;
            }
        }

        /// Confere ids repetidos, referências a itens/roupas/locais que não existem
        /// e diálogos incompletos. Devolve a lista de problemas (vazia se estiver tudo certo).
        public static List<string> Validar(CatalogoJogo catalogo)
        {
            var problemas = new List<string>();

            VerificarIdsRepetidos(catalogo.Npcs.Select(n => n.Id), "NPC", problemas);
            VerificarIdsRepetidos(catalogo.Itens.Select(i => i.Id), "item", problemas);
            VerificarIdsRepetidos(catalogo.Encontro.Roupas.Select(r => r.Id), "roupa", problemas);
            VerificarIdsRepetidos(catalogo.Encontro.Locais.Select(l => l.Id), "local", problemas);

            if (catalogo.Npcs.Count == 0) problemas.Add("npcs.json não tem nenhum NPC.");
            if (catalogo.Flertes.Count == 0) problemas.Add("flertes.json precisa de pelo menos um flerte.");
            if (catalogo.Encontro.Roupas.Count == 0) problemas.Add("encontro.json precisa de pelo menos uma roupa.");
            if (catalogo.Encontro.Locais.Count == 0) problemas.Add("encontro.json precisa de pelo menos um local.");

            foreach (var npc in catalogo.Npcs)
            {
                string nome = string.IsNullOrWhiteSpace(npc.Nome) ? npc.Id : npc.Nome;

                if (string.IsNullOrWhiteSpace(npc.Id)) problemas.Add($"NPC \"{nome}\" está sem id.");
                if (string.IsNullOrWhiteSpace(npc.Nome)) problemas.Add($"NPC \"{npc.Id}\" está sem nome.");
                if (npc.Idade < Regras.Constantes.IdadeMinima) problemas.Add($"NPC \"{nome}\": todos os NPCs precisam ter 18 anos ou mais.");

                if (catalogo.BuscarRoupa(npc.Preferencias.Roupa) == null)
                    problemas.Add($"NPC \"{nome}\": roupa preferida \"{npc.Preferencias.Roupa}\" não existe em encontro.json.");
                if (catalogo.BuscarLocal(npc.Preferencias.Local) == null)
                    problemas.Add($"NPC \"{nome}\": local preferido \"{npc.Preferencias.Local}\" não existe em encontro.json.");
                var presente = catalogo.BuscarItem(npc.Preferencias.Presente);
                if (presente == null || presente.Categoria != CategoriaItem.Presente)
                    problemas.Add($"NPC \"{nome}\": presente preferido \"{npc.Preferencias.Presente}\" não é um presente de itens.json.");

                if (npc.Topicos.Count == 0)
                    problemas.Add($"NPC \"{nome}\": precisa de pelo menos um tópico de conversa.");

                for (int t = 0; t < npc.Topicos.Count; t++)
                {
                    var topico = npc.Topicos[t];
                    if (topico.Opcoes.Count < 2)
                        problemas.Add($"NPC \"{nome}\", tópico {t + 1}: precisa de pelo menos duas opções.");
                    if (topico.Opcoes.Any(o => string.IsNullOrWhiteSpace(o.Texto) || string.IsNullOrWhiteSpace(o.Resposta)))
                        problemas.Add($"NPC \"{nome}\", tópico {t + 1}: toda opção precisa de \"texto\" e \"resposta\".");
                }

                if (npc.Loja != null)
                {
                    if (npc.Loja.Desconto <= 0 || npc.Loja.Desconto >= 100)
                        problemas.Add($"NPC \"{nome}\": o desconto da loja deve ficar entre 1 e 99.");
                    foreach (var itemId in npc.Loja.Itens.Where(id => catalogo.BuscarItem(id) == null))
                        problemas.Add($"NPC \"{nome}\": a loja cita o item \"{itemId}\", que não existe em itens.json.");
                }
            }

            foreach (var item in catalogo.Itens)
            {
                if (item.Preco < 0) problemas.Add($"Item \"{item.Id}\": preço não pode ser negativo.");
                if (item.Categoria == CategoriaItem.Consumivel && item.Efeito == TipoEfeito.Nenhum)
                    problemas.Add($"Item \"{item.Id}\": consumível precisa de um efeito (Coragem, Interesse ou Satisfacao).");
            }

            foreach (FinalJogo final in Enum.GetValues(typeof(FinalJogo)))
            {
                if (catalogo.BuscarFinal(final) == null)
                    problemas.Add($"finais.json: falta o texto do final {(int)final} ({final}).");
            }

            return problemas;
        }

        private static void VerificarIdsRepetidos(IEnumerable<string> ids, string tipo, List<string> problemas)
        {
            foreach (var grupo in ids.GroupBy(id => id).Where(g => g.Count() > 1))
            {
                problemas.Add($"Há mais de um {tipo} com o id \"{grupo.Key}\".");
            }
        }
    }
}
