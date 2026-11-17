using System.Collections.Generic;
using System.Linq;
using JogoTextual.Dominio;

namespace JogoTextual.Dados
{
    /// <summary>
    /// Todo o conteúdo do jogo (NPCs, itens, roupas, locais, flertes, eventos e finais),
    /// carregado dos arquivos JSON desta pasta pelo <see cref="CarregadorCatalogo"/>.
    /// </summary>
    public class CatalogoJogo
    {
        public List<Npc> Npcs { get; set; } = new List<Npc>();
        public List<Item> Itens { get; set; } = new List<Item>();
        public OpcoesEncontro Encontro { get; set; } = new OpcoesEncontro();
        public List<string> Flertes { get; set; } = new List<string>();
        public List<EventoCelular> Eventos { get; set; } = new List<EventoCelular>();
        public List<TextoFinal> Finais { get; set; } = new List<TextoFinal>();

        public Npc? BuscarNpc(string id) => Npcs.FirstOrDefault(n => n.Id == id);
        public Item? BuscarItem(string id) => Itens.FirstOrDefault(i => i.Id == id);
        public OpcaoEncontro? BuscarRoupa(string id) => Encontro.Roupas.FirstOrDefault(r => r.Id == id);
        public OpcaoEncontro? BuscarLocal(string id) => Encontro.Locais.FirstOrDefault(l => l.Id == id);
        public TextoFinal? BuscarFinal(FinalJogo final) => Finais.FirstOrDefault(f => f.Final == final);

        public List<Item> ItensDaCategoria(CategoriaItem categoria) =>
            Itens.Where(i => i.Categoria == categoria).ToList();

        /// <summary>NPCs que combinam com o que o jogador busca (Homem, Mulher ou Ambos).</summary>
        public List<Npc> NpcsCompativeis(Preferencia busca)
        {
            if (busca == Preferencia.Ambos) return new List<Npc>(Npcs);
            Genero genero = busca == Preferencia.Homem ? Genero.Homem : Genero.Mulher;
            return Npcs.Where(n => n.Genero == genero).ToList();
        }
    }
}
