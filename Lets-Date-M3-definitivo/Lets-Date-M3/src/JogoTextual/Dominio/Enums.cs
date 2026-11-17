namespace JogoTextual.Dominio
{
    /// Telas do jogo. Substitui os textos livres ("Login", "MenuInicial"...) usados na M2.
    public enum Tela
    {
        Login,
        MenuInicial,
        Perfil,
        Loja,
        Inventario,
        Conversa,
        Matches,    //  conversar de novo com quem você já saiu
        Avaliacoes,
        Sair,
        Encontros,
        FimDoDia,   //  resumo de um dia comum da campanha (antes do Dia dos Namorados)
        Final,
        Encerrar
    }

    ///  quanto tempo falta para o Dia dos Namorados no começo da campanha.
    public enum Dificuldade
    {
        Facil,   // 2 semanas
        Medio,   // 1 semana
        Dificil  // no próprio dia (jogo clássico)
    }

    /// Gênero de um NPC.
    public enum Genero
    {
        Homem,
        Mulher
    }

    /// O que o jogador busca no aplicativo.
    public enum Preferencia
    {
        Homem,
        Mulher,
        Ambos
    }

    /// Classificação de uma opção de diálogo (BOM, REGULAR, RUIM, PÉSSIMO).
    public enum Qualidade
    {
        Bom,
        Regular,
        Ruim,
        Pessimo
    }

    /// Categorias de itens vendidos na loja.
    public enum CategoriaItem
    {
        Presente,
        Consumivel
    }

    /// Efeitos que um item consumível pode ter.
    public enum TipoEfeito
    {
        Nenhum,
        Coragem,
        Interesse,
        Satisfacao
    }

    /// Efeitos das notificações (eventos do celular).
    public enum TipoEvento
    {
        Dinheiro,
        Coragem,
        Cupom,
        Dica,
        Nada
    }

    /// Os finais do jogo.
    public enum FinalJogo
    {
        MelhorNemTentar = 1,
        NaoFoiDessaVez = 2,
        AlmasGemeas = 3,
        CalmaAmor = 4,
        SaoMuitasNamoradas = 5,
        NaoDeuContaDoRecado = 6,
        FelizesParaSempre = 7   //  namoro confirmado no Dia dos Namorados
    }

    /// Faixas de resultado de um encontro.
    public enum AvaliacaoEncontro
    {
        Odiou = 1,
        NaoGostou = 2,
        Normal = 3,
        Gostou = 4,
        Adorou = 5
    }
}