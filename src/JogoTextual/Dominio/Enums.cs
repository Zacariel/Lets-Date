namespace JogoTextual.Dominio
{
    /// <summary>Telas do jogo. Substitui os textos livres ("Login", "MenuInicial"...) usados na M2.</summary>
    public enum Tela
    {
        Login,
        MenuInicial,
        Perfil,
        Loja,
        Inventario,
        Conversa,
        Avaliacoes,
        Sair,
        Encontros,
        Final,
        Encerrar
    }

    /// <summary>Gênero de um NPC.</summary>
    public enum Genero
    {
        Homem,
        Mulher
    }

    /// <summary>O que o jogador busca no aplicativo.</summary>
    public enum Preferencia
    {
        Homem,
        Mulher,
        Ambos
    }

    /// <summary>Classificação de uma opção de diálogo (BOM, REGULAR, RUIM, PÉSSIMO).</summary>
    public enum Qualidade
    {
        Bom,
        Regular,
        Ruim,
        Pessimo
    }

    /// <summary>Categorias de itens vendidos na loja.</summary>
    public enum CategoriaItem
    {
        Presente,
        Consumivel
    }

    /// <summary>Efeitos que um item consumível pode ter.</summary>
    public enum TipoEfeito
    {
        Nenhum,
        Coragem,
        Interesse,
        Satisfacao
    }

    /// <summary>Efeitos das notificações (eventos do celular).</summary>
    public enum TipoEvento
    {
        Dinheiro,
        Coragem,
        Cupom,
        Dica,
        Nada
    }

    /// <summary>Os seis finais definidos no documento do jogo.</summary>
    public enum FinalJogo
    {
        MelhorNemTentar = 1,
        NaoFoiDessaVez = 2,
        AlmasGemeas = 3,
        CalmaAmor = 4,
        SaoMuitasNamoradas = 5,
        NaoDeuContaDoRecado = 6
    }

    /// <summary>Faixas de resultado de um encontro.</summary>
    public enum AvaliacaoEncontro
    {
        Odiou = 1,
        NaoGostou = 2,
        Normal = 3,
        Gostou = 4,
        Adorou = 5
    }
}
