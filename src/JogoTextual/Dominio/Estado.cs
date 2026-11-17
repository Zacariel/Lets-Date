using System.Collections.Generic;

namespace JogoTextual.Dominio
{
    /// <summary>Dados do perfil criados no Login.</summary>
    public class PerfilJogador
    {
        public string Nome { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty;
        public int Idade { get; set; }
        public Preferencia Busca { get; set; } = Preferencia.Ambos;
        public string Biografia { get; set; } = string.Empty;
    }

    /// <summary>Dados fictícios da compra, pedidos só na primeira compra de conversa extra.</summary>
    public class DadosCompra
    {
        public string Titular { get; set; } = string.Empty;
        public string ApelidoCartao { get; set; } = string.Empty;
    }

    /// <summary>Avaliação (estrelas) de um encontro já realizado.</summary>
    public class RegistroAvaliacao
    {
        public int Dia { get; set; }
        public string NomeNpc { get; set; } = string.Empty;
        public int Satisfacao { get; set; }
        public AvaliacaoEncontro Avaliacao { get; set; }
    }

    /// <summary>
    /// Tudo o que fica salvo no arquivo entre uma execução e outra.
    /// </summary>
    public class DadosSalvos
    {
        public PerfilJogador? Perfil { get; set; }

        /// <summary>Conversas que sobraram quando o jogador usou "Sair do aplicativo". Somadas às 3 do próximo dia (máximo 4).</summary>
        public int ConversasGuardadas { get; set; }

        public DadosCompra? DadosCompra { get; set; }

        /// <summary>Ids dos NPCs donos de loja que já aceitaram um encontro (desconto permanente).</summary>
        public List<string> DescontosDesbloqueados { get; set; } = new List<string>();

        public List<RegistroAvaliacao> Avaliacoes { get; set; } = new List<RegistroAvaliacao>();
        public List<FinalJogo> FinaisDesbloqueados { get; set; } = new List<FinalJogo>();
        public int DiasJogados { get; set; }
    }

    /// <summary>Uma conversa em andamento com um NPC.</summary>
    public class EstadoConversa
    {
        public string NpcId { get; set; } = string.Empty;
        public int Interesse { get; set; } = Regras.Constantes.InteresseInicial;
        public int ProximoTopico { get; set; }
        public bool SuperLikeUsado { get; set; }
    }

    /// <summary>
    /// Estado de uma partida (um dia dos namorados). Começa do zero a cada dia.
    /// </summary>
    public class EstadoPartida
    {
        public int Conversas { get; set; } = Regras.Constantes.ConversasIniciais;
        public int ConversasIniciadas { get; set; }
        public bool ConversaExtraComprada { get; set; }
        public int Coragem { get; set; } = Regras.Constantes.CoragemMaxima;
        public int Dinheiro { get; set; } = Regras.Constantes.DinheiroInicial;

        /// <summary>Desconto de cupom (em %) aplicado na próxima compra da loja.</summary>
        public int CupomDesconto { get; set; }

        /// <summary>Quantidade de cada item que o jogador tem (id do item → quantidade).</summary>
        public Dictionary<string, int> Inventario { get; set; } = new Dictionary<string, int>();

        public List<string> NpcsVistos { get; set; } = new List<string>();
        public List<string> EncontrosMarcados { get; set; } = new List<string>();

        /// <summary>Pistas descobertas nas conversas (id do NPC → anotações).</summary>
        public Dictionary<string, List<string>> Anotacoes { get; set; } = new Dictionary<string, List<string>>();

        public EstadoConversa? ConversaAtual { get; set; }
        public FinalJogo? FinalAlcancado { get; set; }

        public int Quantidade(string itemId) => Inventario.TryGetValue(itemId, out int qtd) ? qtd : 0;

        public void AdicionarItem(string itemId, int quantidade = 1)
        {
            Inventario[itemId] = Quantidade(itemId) + quantidade;
        }

        /// <summary>Remove uma unidade do item. Retorna falso se o jogador não tiver o item.</summary>
        public bool RemoverItem(string itemId)
        {
            int atual = Quantidade(itemId);
            if (atual <= 0) return false;
            if (atual == 1) Inventario.Remove(itemId);
            else Inventario[itemId] = atual - 1;
            return true;
        }

        public void Anotar(string npcId, string dica)
        {
            if (!Anotacoes.TryGetValue(npcId, out var lista))
            {
                lista = new List<string>();
                Anotacoes[npcId] = lista;
            }
            if (!lista.Contains(dica)) lista.Add(dica);
        }
    }
}
