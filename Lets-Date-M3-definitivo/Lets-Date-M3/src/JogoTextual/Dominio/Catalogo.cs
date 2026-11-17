using System.Collections.Generic;
using System.Linq;

namespace JogoTextual.Dominio
{
    // Classes que representam o conteúdo lido dos arquivos JSON da pasta Dados.
    // Para adicionar NPCs, itens, roupas, locais, flertes, eventos ou finais,
    // basta editar os JSON (veja docs/como-adicionar-conteudo.md), sem mexer no código.

    public class OpcaoDialogo
    {
        public string Texto { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public Qualidade Qualidade { get; set; }

        /// Pista sobre as preferências do NPC, anotada quando o jogador escolhe esta fala (opcional).
        public string? Dica { get; set; }
    }

    /// Um assunto da conversa: o que o NPC fala e as quatro respostas possíveis.
    public class TopicoDialogo
    {
        public string Fala { get; set; } = string.Empty;
        public List<OpcaoDialogo> Opcoes { get; set; } = new List<OpcaoDialogo>();
    }

    /// Roupa, presente e local preferidos de um NPC (ids dos arquivos de dados).
    public class PreferenciasNpc
    {
        public string Roupa { get; set; } = string.Empty;
        public string Presente { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
    }

    /// um capítulo da história do NPC. Aparece quando o relacionamento chega no nível indicado
    /// (1 = Match, 2 = Ficando, 3 = Namorando). Traz assuntos novos e, se quiser, gostos novos
    /// para o próximo encontro.
    /// não me matem por ter colocado historia no jogo, eu sei que voces negariam
    public class CapituloNpc
    {
        public int Nivel { get; set; }
        public List<TopicoDialogo> Topicos { get; set; } = new List<TopicoDialogo>();

        /// Opcional. Se ficar vazio, valem as preferências normais do NPC.
        public PreferenciasNpc? Preferencias { get; set; }
    }

    /// Loja de um NPC. Conseguir um encontro com o dono libera um desconto permanente.
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

        /// Capítulos da história (opcional).
        public List<CapituloNpc> Capitulos { get; set; } = new List<CapituloNpc>();

        ///  palavras que, se aparecerem na biografia do jogador, dão +10 de interesse.
        public List<string> PalavrasChave { get; set; } = new List<string>();

        ///  o que o NPC fala quando acha uma palavra-chave na bio. Aceita {apelido} e {palavra}.
        public string ReacaoBiografia { get; set; } = "Li sua bio e vi que você curte {palavra}... a gente tem algo em comum, {apelido}!";

        /// Escolhe a palavra conforme o gênero do NPC, ex.: Flexao("dono", "dona").
        public string Flexao(string masculino, string feminino) => Genero == Genero.Homem ? masculino : feminino;

        ///  capítulo do nível pedido, ou null se não existir.
        public CapituloNpc? Capitulo(int nivel) => Capitulos.FirstOrDefault(c => c.Nivel == nivel);

        ///  assuntos do capítulo; se o capítulo não existir, usa os assuntos normais.
        public List<TopicoDialogo> TopicosDoNivel(int nivel) => Capitulo(nivel)?.Topicos ?? Topicos;

        ///  gostos do capítulo; se não tiver, usa as preferências normais.
        public PreferenciasNpc PreferenciasDoNivel(int nivel) => Capitulo(nivel)?.Preferencias ?? Preferencias;
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

    /// Roupa ou local escolhidos na fase de encontro.
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

    /// Notificação que pode chegar no celular entre uma conversa e outra.
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