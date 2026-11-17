# Como adicionar NPCs, itens e outros conteúdos

Todo o conteúdo do jogo fica em `src/JogoTextual/Dados`. Para adicionar ou mudar algo, edite o JSON e rode o jogo de novo. **Não é preciso mexer no código.**

Ao abrir, o jogo confere os arquivos. Se algo estiver errado (um presente que não existe, um id repetido, uma vírgula faltando), ele mostra a lista de problemas e não abre, para o erro não aparecer no meio da partida. O teste `dotnet test` faz a mesma conferência.

Dicas gerais:

- Linhas começando com `//` são comentários e são ignoradas.
- O `id` é o nome interno: sem espaços nem acentos (ex.: `"jogo-tabuleiro"`). O `nome` é o que aparece na tela.
- Em qualquer texto, `{apelido}` é trocado pelo apelido do jogador.

## Novo NPC (`npcs.json`)

Copie este modelo para dentro da lista, separado dos outros por vírgula:

```json
{
  "id": "julia",
  "nome": "Júlia Prado",
  "idade": 26,
  "genero": "Mulher",
  "biografia": "Fotógrafa, viciada em café e em pôr do sol.",
  "preferencias": { "roupa": "casual", "presente": "flores", "local": "parque" },
  "loja": { "nome": "Estúdio da Júlia", "itens": [ "flores" ], "desconto": 20 },
  "topicos": [
    {
      "fala": "Oi, {apelido}! Qual foi a última foto bonita que você tirou?",
      "opcoes": [
        { "qualidade": "Bom",     "texto": "Um pôr do sol no parque!", "resposta": "Ai, o parque é meu lugar favorito!", "dica": "Júlia ama o parque." },
        { "qualidade": "Regular", "texto": "Uma selfie, conta?",          "resposta": "Conta sim, haha." },
        { "qualidade": "Ruim",    "texto": "Não tiro fotos.",             "resposta": "Hm, que pena." },
        { "qualidade": "Pessimo", "texto": "Fotografia é hobby de rico.", "resposta": "Nossa. Ok, {apelido}." }
      ]
    }
  ],
  "conviteAceito": "Topo! Vou levar a câmera, {apelido}.",
  "conviteRecusado": "Acho que não, {apelido}..."
}
```

| Campo | O que é |
|---|---|
| `genero` | `"Homem"` ou `"Mulher"`. Define em qual busca o NPC aparece ("Ambos" mostra todos). |
| `idade` | 18 ou mais (o jogo recusa NPCs menores de idade). |
| `preferencias.roupa` / `preferencias.local` | Um `id` de `encontro.json`. |
| `preferencias.presente` | Um `id` de presente de `itens.json`. |
| `topicos` | Cada vez que o jogador escolhe "Conversar", o próximo tópico é usado. Pode ter quantos quiser. |
| `qualidade` | `"Bom"` (+40), `"Regular"` (+20), `"Ruim"` (-10) ou `"Pessimo"` (-30) de interesse. As opções aparecem embaralhadas. |
| `dica` | Opcional. O jogador anota a dica e a vê na conversa e na hora de se arrumar para o encontro. |
| `loja` | Opcional. Se o NPC aceitar um encontro, os itens listados ganham `desconto` (%) permanente. |

Para um dia completo, cada busca ("Homem" e "Mulher") precisa de pelo menos 4 NPCs.

## Novo item (`itens.json`)

```json
{ "id": "caneca", "nome": "Caneca personalizada", "categoria": "Presente", "preco": 14, "descricao": "Com uma foto de vocês dois." }
```

Itens consumíveis precisam de um efeito:

```json
{ "id": "bala-menta", "nome": "Bala de menta", "categoria": "Consumivel", "preco": 3, "efeito": "Coragem", "valorEfeito": 10, "descricao": "+10 de coragem." }
```

| `efeito` | Onde pode ser usado | O que faz |
|---|---|---|
| `Coragem` | Inventário ou conversa | Soma coragem (máximo 100) |
| `Interesse` | Conversa (1 vez por conversa) | Soma interesse do NPC |
| `Satisfacao` | Fase de encontro | Soma satisfação daquele encontro |

## Nova roupa ou local (`encontro.json`)

```json
{ "id": "boliche", "nome": "Boliche", "descricao": "pista 7, sapatos alugados" }
```

## Novo flerte (`flertes.json`)

Basta acrescentar uma frase na lista.

## Nova notificação (`eventos.json`)

```json
{ "id": "chuva", "remetente": "Previsão do tempo", "tipo": "Coragem", "valor": -5, "mensagem": "Vai chover à noite. Ótimo..." }
```

`tipo`: `Dinheiro`, `Coragem`, `Cupom` (desconto em % na próxima compra), `Dica` (revela uma preferência de alguém com encontro marcado) ou `Nada`.
