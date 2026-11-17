# Roteiro de testes manuais

Use estes casos para conferir o jogo antes da entrega e para gravar a demonstração. Os testes automáticos (`dotnet test`) cobrem as mesmas regras.

Para repetir uma partida igual, rode com uma semente fixa:

```powershell
$env:LETSDATE_SEMENTE = "7"
dotnet run --project src/JogoTextual
```

Para começar do zero, apague o arquivo `save_letsdate.json` da pasta de saída (`src/JogoTextual/bin/Debug/net9.0/`) ou escolha "Criar um perfil novo" no Login.

Isso não é obrigatório, porem evita erros futuros

## Cenários da devolutiva da M2

| # | Situação | Ação | Resultado esperado |
|---|---|---|---|
| 1 | Login, campo Idade | Digitar `abc` | "Digite sua idade usando apenas números inteiros." e a idade é pedida de novo (o nome não é pedido outra vez). |
| 2 | Login, campo Idade | Digitar `101` | Mensagem de opção inválida e a idade é pedida de novo. |
| 3 | Login, campo Idade | Digitar `17` | "Oh, parece que você ainda é muito novo para usar nosso aplicativo! Volte em breve." e o aplicativo fecha. |
| 4 | Fase de encontro, roupa | Digitar `9`, depois `x` | "Oops! Falha no nosso sistema, esta não é uma opção correspondente! :(" e o menu de roupas volta. A satisfação só é calculada depois das três escolhas válidas. |
| 5 | Loja, dia com 3 conversas | Comprar a conversa extra (vai a 4), usar uma conversa e tentar comprar de novo | "Você já usou sua conversa extra de hoje!" A conversa extra é uma por dia. |
| 6 | Conversa com interesse 40 | Chamar para encontro | Recusa imediata (abaixo de 60 não há sorteio). |
| 7 | Conversa com interesse 60 | Chamar para encontro | Sorteio de 1 a 100: aceita se o número for até 60 (60% de chance). |
| 8 | Conversa com interesse 100 | Chamar para encontro | Sempre aceita. |

## Outros casos

| # | Situação | Ação | Resultado esperado |
|---|---|---|---|
| 9 | Menu, nenhuma conversa iniciada | Sair → Sair do aplicativo | Final 1 – Melhor nem tentar; o aplicativo fecha. Ao abrir de novo, o dia começa com 4 conversas (as 3 não usadas ficaram guardadas, limite de 4). |
| 10 | Menu, 2 conversas restantes | Sair → Sair do aplicativo; abrir de novo e "Continuar" | "Você guardou 2 conversa(s)" e o dia começa com 4 (3 + 2, limitado a 4). |
| 11 | Conversa | Escolher respostas péssimas até o interesse chegar a -60 | "Você foi bloqueado(a)..." e a conversa termina. |
| 12 | Conversa com coragem 0 | Flertar | "Oh, parece que você não tem coragem o suficiente para flertar agora..." |
| 13 | Última conversa termina sem ter comprado a extra | — | Aparece "Você pode ter mais uma conversa! Deseja comprá-la na loja?". Com NÃO, vai para os encontros. |
| 14 | Conseguir encontro com Roberta, Kayla ou Davi | Abrir a Loja | Desconto permanente no item da loja do NPC. |
| 15 | Fase de encontro com 3 encontros | Fazer tudo certo | Cada encontro fica com 80 (70 + 30 − 20 de correria). |
| 16 | Busca "Homem" | Buscar encontros | Só aparecem Davi, Thiago, Rafael e Bruno. |
| 17 | Arquivo `npcs.json` com presente inexistente | Abrir o jogo | Lista de problemas nos dados e o jogo não abre. |
