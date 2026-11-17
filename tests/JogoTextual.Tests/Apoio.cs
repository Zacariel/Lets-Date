using System;
using System.Collections.Generic;
using System.IO;
using JogoTextual.Dados;
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Tests
{
    /// <summary>Aleatório controlado: devolve os números na ordem em que foram passados.</summary>
    public class AleatorioFixo : IAleatorio
    {
        private readonly Queue<int> _valores;

        public AleatorioFixo(params int[] valores)
        {
            _valores = new Queue<int>(valores);
        }

        public int Usados { get; private set; }

        public int Entre(int minimo, int maximo)
        {
            Usados++;
            int valor = _valores.Count > 0 ? _valores.Dequeue() : minimo;
            if (valor < minimo || valor > maximo)
                throw new InvalidOperationException($"Valor de teste {valor} fora do intervalo {minimo}..{maximo}.");
            return valor;
        }
    }

    /// <summary>Catálogo pequeno e previsível para os testes de regras.</summary>
    public static class CatalogoTeste
    {
        public static CatalogoJogo Criar()
        {
            return new CatalogoJogo
            {
                Npcs = new List<Npc>
                {
                    new Npc
                    {
                        Id = "ana", Nome = "Ana", Idade = 25, Genero = Genero.Mulher,
                        Preferencias = new PreferenciasNpc { Roupa = "elegante", Presente = "livro", Local = "praia" },
                        Loja = new LojaNpc { Nome = "Livraria da Ana", Itens = new List<string> { "livro" }, Desconto = 50 },
                        Topicos = new List<TopicoDialogo>
                        {
                            new TopicoDialogo
                            {
                                Fala = "Oi!",
                                Opcoes = new List<OpcaoDialogo>
                                {
                                    new OpcaoDialogo { Texto = "a", Resposta = "A", Qualidade = Qualidade.Bom, Dica = "Ana ama a praia." },
                                    new OpcaoDialogo { Texto = "b", Resposta = "B", Qualidade = Qualidade.Pessimo }
                                }
                            }
                        }
                    },
                    new Npc
                    {
                        Id = "beto", Nome = "Beto", Idade = 30, Genero = Genero.Homem,
                        Preferencias = new PreferenciasNpc { Roupa = "casual", Presente = "chocolates", Local = "parque" },
                        Topicos = new List<TopicoDialogo>
                        {
                            new TopicoDialogo
                            {
                                Fala = "E aí?",
                                Opcoes = new List<OpcaoDialogo>
                                {
                                    new OpcaoDialogo { Texto = "a", Resposta = "A", Qualidade = Qualidade.Bom },
                                    new OpcaoDialogo { Texto = "b", Resposta = "B", Qualidade = Qualidade.Ruim }
                                }
                            }
                        }
                    }
                },
                Itens = new List<Item>
                {
                    new Item { Id = "livro", Nome = "Livro", Categoria = CategoriaItem.Presente, Preco = 20 },
                    new Item { Id = "chocolates", Nome = "Chocolates", Categoria = CategoriaItem.Presente, Preco = 12 },
                    new Item { Id = "energetico", Nome = "Energético", Categoria = CategoriaItem.Consumivel, Preco = 8, Efeito = TipoEfeito.Coragem, ValorEfeito = 50 },
                    new Item { Id = "super-like", Nome = "Super Like", Categoria = CategoriaItem.Consumivel, Preco = 10, Efeito = TipoEfeito.Interesse, ValorEfeito = 15 },
                    new Item { Id = "perfume", Nome = "Perfume", Categoria = CategoriaItem.Consumivel, Preco = 18, Efeito = TipoEfeito.Satisfacao, ValorEfeito = 10 }
                },
                Encontro = new OpcoesEncontro
                {
                    Roupas = new List<OpcaoEncontro> { new OpcaoEncontro { Id = "elegante", Nome = "Elegante" }, new OpcaoEncontro { Id = "casual", Nome = "Casual" } },
                    Locais = new List<OpcaoEncontro> { new OpcaoEncontro { Id = "praia", Nome = "Praia" }, new OpcaoEncontro { Id = "parque", Nome = "Parque" } }
                },
                Flertes = new List<string> { "Cantada de teste" },
                Finais = new List<TextoFinal>()
            };
        }

        /// <summary>Perfil e save prontos, com o nome "Lucas" cadastrado.</summary>
        public static DadosSalvos SalvosComPerfil(Preferencia busca = Preferencia.Ambos)
        {
            return new DadosSalvos { Perfil = new PerfilJogador { Nome = "Lucas", Apelido = "Zac", Idade = 25, Busca = busca } };
        }

        /// <summary>Partida com uma conversa em andamento com o NPC informado.</summary>
        public static EstadoPartida PartidaConversando(string npcId, int interesse)
        {
            return new EstadoPartida { ConversaAtual = new EstadoConversa { NpcId = npcId, Interesse = interesse } };
        }

        /// <summary>Encontra a pasta src/JogoTextual/Dados subindo a partir da pasta dos testes.</summary>
        public static string PastaDadosReal()
        {
            foreach (var inicio in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
            {
                var pasta = new DirectoryInfo(inicio);
                while (pasta != null)
                {
                    string candidato = Path.Combine(pasta.FullName, "src", "JogoTextual", "Dados");
                    if (Directory.Exists(candidato)) return candidato;
                    pasta = pasta.Parent;
                }
            }
            return CarregadorCatalogo.PastaPadrao;
        }
    }
}
