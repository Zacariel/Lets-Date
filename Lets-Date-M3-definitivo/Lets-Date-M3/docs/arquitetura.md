# Arquitetura

Na M2, `Jogo.cs` concentrava telas, leitura do teclado, decisões e atualização de dados, e a tela atual era um texto livre (`"MenuInicial"`, `"Conversa"`...). Na M3 o código foi dividido por responsabilidade:

| Pasta | Responsabilidade | Exemplos |
|---|---|---|
| `Dominio` | Dados do jogo, sem lógica de tela | `EstadoPartida`, `DadosSalvos`, `Npc`, `Item`, `Tela` (enum) |
| `Dados` | Conteúdo em JSON, leitura e validação | `CatalogoJogo`, `CarregadorCatalogo`, `npcs.json` |
| `Regras` | Regras do documento, sem `Console` (por isso são testáveis) | `RegrasConversa`, `RegrasLoja`, `RegrasEncontro`, `RegrasFinais` |
| `Telas` | O que cada tela mostra e pergunta; devolvem a próxima `Tela` | `TelaMenuInicial`, `TelaConversa`, `TelaEncontros` |
| `Interface` | Entrada validada e saída formatada | `EntradaConsole`, `SaidaConsole` |
| `Persistencia` | Arquivo de save | `PersistenciaJogo` |

`Jogo` só guarda a tela atual e chama a tela correspondente até chegar em `Tela.Encerrar`:

```csharp
while (TelaAtual != Tela.Encerrar)
{
    TelaAtual = _telas[TelaAtual].Executar(_contexto);
}
```

## Diagrama de classes

```mermaid
classDiagram
    direction LR

    class Program { +Main() int }
    class Jogo {
        -ContextoJogo contexto
        -Dictionary~Tela, ITelaJogo~ telas
        +Tela TelaAtual
        +Iniciar()
    }
    class ITelaJogo { <<interface>> +Executar(ContextoJogo) Tela }
    class ContextoJogo {
        +CatalogoJogo Catalogo
        +DadosSalvos Salvos
        +EstadoPartida Partida
        +EntradaConsole Entrada
        +SaidaConsole Saida
        +RegrasConversa RegrasConversa
        +RegrasLoja RegrasLoja
        +RegrasItens RegrasItens
        +RegrasEventos RegrasEventos
        +Salvar()
    }

    class TelaLogin
    class TelaMenuInicial
    class TelaConversa
    class TelaLoja
    class TelaInventario
    class TelaPerfil
    class TelaAvaliacoes
    class TelaSair
    class TelaEncontros
    class TelaFinal
    class LeitorEscolhasEncontro { +Ler() EscolhasEncontro }
    class FluxoCompraConversa { +Executar(ContextoJogo) bool }

    class EntradaConsole {
        +LerOpcao(min, max) int
        +LerInteiro(pergunta, erro) int
        +LerTexto(pergunta, max) string
        +LerSimNao(pergunta) bool
    }
    class SaidaConsole

    class CatalogoJogo {
        +List~Npc~ Npcs
        +List~Item~ Itens
        +OpcoesEncontro Encontro
        +List~string~ Flertes
        +List~EventoCelular~ Eventos
        +List~TextoFinal~ Finais
    }
    class CarregadorCatalogo { +Carregar(pasta) CatalogoJogo +Validar(catalogo) List~string~ }

    class RegrasConversa { +IniciarConversa() Npc +Responder() int +Flertar() ResultadoFlerte +Convidar() ResultadoConvite }
    class RegrasLoja { +Preco() int +Comprar() ResultadoCompra +ComprarConversaExtra() ResultadoCompraConversa }
    class RegrasItens { +Usar(item, partida, momento) ResultadoUsoItem }
    class RegrasEncontro { +Calcular() ResultadoEncontro +Classificar(satisfacao) AvaliacaoEncontro }
    class RegrasFinais { +Determinar(conversas, encontros) FinalJogo }
    class RegrasDia { +NovoDia(salvos) EstadoPartida }
    class RegrasEventos { +TalvezSortear() EventoCelular +Aplicar() string }
    class IAleatorio { <<interface>> +Entre(min, max) int }

    class DadosSalvos {
        +PerfilJogador Perfil
        +int ConversasGuardadas
        +DadosCompra DadosCompra
        +List~string~ DescontosDesbloqueados
        +List~RegistroAvaliacao~ Avaliacoes
        +List~FinalJogo~ FinaisDesbloqueados
    }
    class EstadoPartida {
        +int Conversas
        +bool ConversaExtraComprada
        +int Coragem
        +int Dinheiro
        +Dictionary Inventario
        +List~string~ EncontrosMarcados
        +EstadoConversa ConversaAtual
    }
    class EstadoConversa { +string NpcId +int Interesse +int ProximoTopico }
    class Npc { +string Id +string Nome +Genero Genero +PreferenciasNpc Preferencias +List~TopicoDialogo~ Topicos +LojaNpc Loja }
    class Item { +string Id +CategoriaItem Categoria +int Preco +TipoEfeito Efeito }
    class PersistenciaJogo { +Salvar(DadosSalvos) +Carregar() DadosSalvos }

    Program --> Jogo
    Program --> CarregadorCatalogo
    Program --> PersistenciaJogo
    Jogo --> ITelaJogo
    Jogo --> ContextoJogo
    ITelaJogo <|.. TelaLogin
    ITelaJogo <|.. TelaMenuInicial
    ITelaJogo <|.. TelaConversa
    ITelaJogo <|.. TelaLoja
    ITelaJogo <|.. TelaInventario
    ITelaJogo <|.. TelaPerfil
    ITelaJogo <|.. TelaAvaliacoes
    ITelaJogo <|.. TelaSair
    ITelaJogo <|.. TelaEncontros
    ITelaJogo <|.. TelaFinal
    TelaEncontros --> LeitorEscolhasEncontro
    TelaLoja --> FluxoCompraConversa
    TelaPerfil --> FluxoCompraConversa
    ContextoJogo --> CatalogoJogo
    ContextoJogo --> DadosSalvos
    ContextoJogo --> EstadoPartida
    ContextoJogo --> EntradaConsole
    ContextoJogo --> SaidaConsole
    ContextoJogo --> RegrasConversa
    ContextoJogo --> RegrasLoja
    RegrasConversa --> IAleatorio
    RegrasEventos --> IAleatorio
    CarregadorCatalogo --> CatalogoJogo
    CatalogoJogo "1" *-- "*" Npc
    CatalogoJogo "1" *-- "*" Item
    EstadoPartida "1" o-- "0..1" EstadoConversa
    PersistenciaJogo --> DadosSalvos
```

## Fluxo de telas

```mermaid
stateDiagram-v2
    [*] --> Login
    Login --> MenuInicial : perfil criado ou continuado
    Login --> Encerrar : menor de 18 / Sair

    MenuInicial --> Conversa : Buscar encontro!
    MenuInicial --> Loja
    MenuInicial --> Inventario
    MenuInicial --> Perfil : Configurações
    MenuInicial --> Avaliacoes
    MenuInicial --> Sair
    MenuInicial --> Encontros : acabaram os perfis compatíveis

    Loja --> MenuInicial
    Inventario --> MenuInicial
    Perfil --> MenuInicial
    Avaliacoes --> MenuInicial

    Conversa --> Conversa : conversar / flertar / usar item
    Conversa --> MenuInicial : convite, bloqueio ou -60 de interesse
    Conversa --> Encontros : conversas acabaram e não há compra possível

    Sair --> Encontros : Sair para encontro
    Sair --> Final : Sair do aplicativo sem ter conversado (Final 1)
    Sair --> Encerrar : Sair do aplicativo (guarda conversas)

    Encontros --> Final
    Final --> MenuInicial : jogar outro dia
    Final --> Encerrar
    Encerrar --> [*]
```

## Por que as regras não usam `Random`

As classes da pasta `Regras` recebem e devolvem dados (`ResultadoConvite`, `ResultadoCompraConversa`...). As telas decidem o que escrever a partir desses resultados. Assim os testes conseguem conferir as regras sem simular o teclado, e o sorteio pode ser controlado com `IAleatorio` (nos testes, `AleatorioFixo` escolhe o resultado do D20 e do convite).
