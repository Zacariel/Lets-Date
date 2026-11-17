using System.Collections.Generic;

namespace JogoTextual.Dominio
{
    // Classes que representam o conteúdo lido dos arquivos JSON da pasta Dados.
    // Para adicionar NPCs, itens, roupas, locais, flertes, eventos ou finais,
    // basta editar os JSON (veja docs/como-adicionar-conteudo.md), sem mexer no código.

    /// <summary>Uma das falas que o jogador pode escolher numa conversa.</summary>
    public class OpcaoDialogo
    {
        public string Texto { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public Qualidade Qualidade { get; set; }

        /// <summary>Pista sobre as preferências do NPC, anotada quando o jogador escolhe esta fala (opcional).</summary>
        public string? Dica { get; set; }
    }

    /// <summary>Um assunto da conversa: o que o NPC fala e as quatro respostas possíveis.</summary>
    public class TopicoDialogo
    {
        public string Fala { get; set; } = string.Empty;
        public List<OpcaoDialogo> Opcoes { get; set; } = new List<OpcaoDialogo>();
    }

    /// <summary>Roupa, presente e local preferidos de um NPC (ids dos arquivos de dados).</summary>
    public class PreferenciasNpc
    {
        public string Roupa { get; set; } = string.Empty;
        public string Presente { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
    }

    /// <summary>Loja de um NPC. Conseguir um encontro com o dono libera um desconto permanente.</summary>
    public class LojaNpc
    {
        public string Nome { get; set; } = string.Empty;
        public List<string> Itens { get; set; } = new List<string>();
        public int Desconto { get; set; }
    }

    public class Npc
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public Genero Genero { get; set; }
        public string Biografia { get; set; } = string.Empty;
        public PreferenciasNpc Preferencias { get; set; } = new PreferenciasNpc();
        public List<TopicoDialogo> Topicos { get; set; } = new List<TopicoDialogo>();
        public string ConviteAceito { get; set; } = "Claro, eu adoraria!";
        public string ConviteRecusado { get; set; } = "Acho melhor não...";
        public LojaNpc? Loja { get; set; }

        /// <summary>Escolhe a palavra conforme o gênero do NPC, ex.: Flexao("dono", "dona").</summary>
        public string Flexao(string masculino, string feminino) => Genero == Genero.Homem ? masculino : feminino;
    }

    public class Item
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public CategoriaItem Categoria { get; set; }
        public int Preco { get; set; }
        public TipoEfeito Efeito { get; set; } = TipoEfeito.Nenhum;
        public int ValorEfeito { get; set; }
    }

    /// <summary>Roupa ou local escolhidos na fase de encontro.</summary>
    public class OpcaoEncontro
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }

    public class OpcoesEncontro
    {
        public List<OpcaoEncontro> Roupas { get; set; } = new List<OpcaoEncontro>();
        public List<OpcaoEncontro> Locais { get; set; } = new List<OpcaoEncontro>();
    }

    /// <summary>Notificação que pode chegar no celular entre uma conversa e outra.</summary>
    public class EventoCelular
    {
        public string Id { get; set; } = string.Empty;
        public string Remetente { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public TipoEvento Tipo { get; set; }
        public int Valor { get; set; }
    }

    public class TextoFinal
    {
        public FinalJogo Final { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
    }
}
