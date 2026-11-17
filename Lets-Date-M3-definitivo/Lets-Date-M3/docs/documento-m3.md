# Let's Date! — Documento do jogo (M3)

**Alunos:** Kimberly Bernardo da Silva, Lucas Gonçalves, Rau Guerreiro Weiss
**Professor:** Marcelo
**Disciplina:** Lógica de Programação — Design de Games, 2026-02

*"É dia dos namorados! Mas, infelizmente, você está sozinho, mas por pouco tempo. Com nosso novo app de encontros, o "Let's Date!", você terá um par até o fim do dia!"*

## Resumo da proposta

O jogo simula um aplicativo de namoro em que o jogador pode iniciar até quatro conversas e convidar NPCs para um encontro, depois sair com eles. O final varia de acordo com a quantidade de encontros conseguidos até o fim do dia.

## Objetivo do jogador

Não passar o dia dos namorados sozinho. Pelos diálogos, o jogador deve fazer as escolhas certas para aumentar o interesse do NPC e convidá-lo para um encontro antes de atingir o limite de conversas. À noite, precisa acertar a roupa, o presente e o local preferidos de cada pessoa.

## Ciclo principal

1. O jogador cria o perfil (ou continua o perfil salvo).
2. No Menu Inicial, escolhe "Buscar encontro!".
3. Uma conversa com um NPC compatível com o que ele busca começa.
4. Ele conversa, flerta e usa itens para aumentar o interesse, anotando as dicas que o NPC dá.
5. Quando achar que é a hora, chama o NPC para um encontro. O NPC pode aceitar ou não.
6. Entre uma conversa e outra, notificações do celular podem trazer dinheiro, coragem, cupons ou dicas. O jogador pode passar na Loja.
7. Quando as conversas acabam (ou ele escolhe "Sair para encontro"), começa a fase de encontro.
8. Para cada encontro, escolhe roupa, presente e local. O jogo mostra a satisfação e as estrelas.
9. O final é mostrado de acordo com a quantidade de encontros. O jogador pode jogar outro dia.

## Estados e telas

| Tela | O que acontece |
|---|---|
| Login | Cria o perfil (nome, apelido, idade, busca e biografia) ou continua o perfil salvo. |
| Menu Inicial | Mostra conversas, carteira, coragem e encontros marcados. Dá acesso a todas as outras telas. |
| Conversa | Conversar, flertar, usar item, chamar para encontro ou bloquear. |
| Loja | Conversa extra, presentes e itens especiais. |
| Inventário | Itens comprados; permite usar os que funcionam fora das conversas. |
| Configurações | Perfil, alterações permitidas e compra de conversa extra. |
| Avaliações e finais | Estrelas de todos os encontros e galeria dos finais desbloqueados. |
| Sair | "Sair para encontro" ou "Sair do aplicativo". |
| Encontros | Preparação e resultado de cada encontro. |
| Final | Texto do final, resumo do dia e opção de jogar outro dia. |

## Ações do jogador

### Login

> Começar! / Sair

- **Nome:** obrigatório. Aparece nas Configurações e não pode ser alterado. É usado para confirmar a compra da conversa extra.
- **Apelido:** obrigatório (até 20 caracteres). Pode ser alterado. Os NPCs chamam o jogador por ele.
- **Idade:** só aceita número inteiro. Texto não numérico mostra *"Digite sua idade usando apenas números inteiros."* e pede só a idade de novo. Acima de 100 mostra a mensagem de opção inválida e pede de novo. Abaixo de 18 mostra *"Oh, parece que você ainda é muito novo para usar nosso aplicativo! Volte em breve."* e fecha o aplicativo. Não pode ser alterada.
- **O que você busca?** 1. Homem, 2. Mulher, 3. Ambos. Define quais NPCs aparecem. Pode ser alterado.
- **Biografia:** até 100 caracteres (texto maior é recusado e pedido de novo). Pode ser alterada.

Se já existir um perfil salvo, o Login oferece "Continuar como (apelido)" ou "Criar um perfil novo" (que apaga o progresso, com confirmação).

### Menu Inicial

1. Buscar encontro! · 2. Loja · 3. Inventário · 4. Configurações · 5. Avaliações e finais · 0. Sair

### Buscar encontro! (Conversa)

O jogo sorteia um NPC compatível que ainda não apareceu no dia e gasta uma conversa. A tela mostra nome, idade, biografia, as barras de **interesse** e **coragem** e as anotações já feitas sobre esse NPC.

1. **Conversar:** o NPC puxa um assunto e o jogador escolhe uma de quatro respostas, que aparecem **embaralhadas** (a melhor não fica sempre no número 1). Cada NPC tem dois assuntos.
   - BOM: +40 de interesse
   - REGULAR: +20
   - RUIM: −10
   - PÉSSIMO: −30

   Algumas respostas fazem o NPC revelar uma preferência, que vira uma **anotação** (ex.: *"Maria ama piquenique na praça e adora ganhar flores."*).
2. **Flertar:** custa 25 de coragem. O jogo sorteia uma cantada e rola um D20: variação = (resultado − 10) × 10. Exemplos: 4 → −60, 10 → 0, 13 → +30.
3. **Usar item:** Energético da Coragem ou Super Like.
4. **Chamar para encontro:** veja "Condição de aceite". A conversa termina com qualquer resposta.
5. **Bloquear:** pede confirmação (*"Tem certeza que deseja bloquear "Nome"?"*). SIM encerra a conversa; NÃO volta à conversa. A conversa gasta não é devolvida.

### Loja

A loja tem a conversa extra, sete presentes e três itens especiais. Mostra o preço com os descontos aplicados e quantos de cada item o jogador já tem.

| Item | Preço | Efeito |
|---|---:|---|
| Conversa extra | R$ 5 | +1 conversa (uma por dia) |
| Chocolates | R$ 12 | presente |
| Flores | R$ 15 | presente |
| Vinho | R$ 25 | presente |
| Livro | R$ 20 | presente |
| Pelúcia | R$ 10 | presente |
| Ingressos de show | R$ 30 | presente |
| Jogo de tabuleiro | R$ 22 | presente |
| Energético da Coragem | R$ 8 | +50 de coragem |
| Super Like | R$ 10 | +15 de interesse (uma vez por conversa) |
| Perfume Importado | R$ 18 | +10 de satisfação em um encontro |

### Comprar mais conversas!

Pela Loja, pelas Configurações ou pelo aviso que aparece quando as conversas acabam.

1. *"Confirme seu nome:"* — precisa ser o nome do cadastro (maiúsculas e espaços não importam). Se estiver errado: *"Informações incorretas! Preencha seu nome novamente."* e o nome é pedido de novo (0 cancela).
2. Na primeira compra, o jogo pede dados **fictícios** (nome do titular e apelido do cartão) e os guarda no save.
3. *"Você deseja comprar mais uma conversa por R$ 5,00?"* — SIM ou NÃO.

### Sair

- **Sair para encontro:** vai para a fase de encontro (ou direto para os finais 1 ou 2, se não houver encontros).
- **Sair do aplicativo:** se nenhuma conversa foi iniciada, mostra o Final 1 e fecha. Se houver encontros marcados, pede confirmação (eles são perdidos). Nos dois casos, as conversas que sobraram ficam guardadas para o próximo dia.

### Encontro

*"Após algumas horas no aplicativo buscando por encontros, finalmente chegou o momento de começar a noite, será o seu melhor dia dos namorados!"*

Para cada NPC com encontro marcado, o jogo mostra a biografia e as anotações e pergunta:

- **O que vai vestir?** Clássico, Elegante, Esportivo, Romântico, Sexy ou Casual.
- **Vamos comprar presentes!** Entregar um presente do inventário, comprar um agora ou ir sem presente (conta como presente errado).
- **Melhor escolher um bom lugar.** Restaurante, Praça, Praia, Parque ou Cinema.
- Se tiver Perfume Importado, pergunta se quer usar.

Cada pergunta só aceita uma opção da lista: uma opção inválida mostra a mensagem de erro e repete a pergunta. A satisfação só é calculada depois das três escolhas válidas. O resultado mostra cada acerto e erro e a avaliação:

| Satisfação | Mensagem | Estrelas |
|---|---|---|
| 81–100 | "Nome" adorou o encontro! | ★★★★★ |
| 61–80 | "Nome" gostou do encontro! | ★★★★☆ |
| 41–60 | "Nome" achou o encontro normal. | ★★★☆☆ |
| 21–40 | "Nome" não gostou do encontro... | ★★☆☆☆ |
| 0–20 | "Nome" odiou o encontro! | ★☆☆☆☆ |

## Dados, recursos e valores

| Valor | Início | Limites | Muda quando |
|---|---|---|---|
| Conversas | 3 (+ guardadas, máximo 4) | 0 a 4 | Buscar encontro (−1), conversa extra (+1) |
| Interesse | 20 por conversa | −100 a 100 | Respostas, flerte, Super Like |
| Coragem | 100 por dia | 0 a 100 | Flerte (−25), Energético, notificações |
| Carteira | R$ 70 por dia | mínimo R$ 0 | Compras, notificações |
| Satisfação | 70 por encontro | 0 a 100 | Roupa, presente, local, correria, perfume |
| Encontros marcados | 0 | 0 a 4 | Convite aceito |

**Personagens:** oito NPCs (Maria, Alana, Roberta e Kayla; Davi, Thiago, Rafael e Bruno), sorteados sem repetição durante o dia e filtrados pelo que o jogador busca.

## Regras principais

### Condição de aceite do convite

- Interesse abaixo de 60: a resposta é sempre NEGATIVA, sem sorteio.
- Interesse de 60 ou mais: o jogo sorteia um número de 1 a 100. Se for **menor ou igual** ao interesse, o NPC aceita. Com 85 de interesse, há 85% de chance de aceitar; com 100, o NPC sempre aceita.

### Flerte

- Cada flerte consome 25 de coragem.
- Sem coragem suficiente: *"Oh, parece que você não tem coragem o suficiente para flertar agora..."* e o jogador volta à conversa.

### Fim da conversa

- **Manual:** Bloquear.
- **Automático:** depois de "Chamar para encontro", com qualquer resposta.
- **Por limite:** se o interesse chegar a −60, o NPC bloqueia o jogador.

### Conversa extra

- Uma única conversa extra **por dia**. O jogo registra a compra, então não é possível comprar de novo depois de gastar a conversa.
- Não pode passar de 4 conversas: *"Parece que você atingiu o número máximo de conversas! Considere conversar um pouco antes de voltar aqui ;)"*
- Custa R$ 5 da carteira do jogo.
- NÃO: *"Compra não finalizada!"*. SIM: *"Compra finalizada com sucesso! Agora você tem mais uma conversa :D"*.
- Quando as conversas acabam e a compra ainda é possível, aparece *"Você pode ter mais uma conversa! Deseja comprá-la na loja?"*. SIM leva à compra; NÃO segue para os encontros.

### Descontos permanentes

Alguns NPCs têm uma loja. Conseguir um encontro com eles libera um desconto permanente (salvo entre os dias):

| NPC | Loja | Desconto |
|---|---|---|
| Roberta Cunha | Sebo da Roberta | 50% em Livro |
| Davi Rocha | Confeitaria Doce Rocha | 40% em Chocolates |
| Kayla Schrute | Fazenda Schrute — beterrabas e flores | 30% em Flores |

### Notificações do celular

Ao voltar ao menu depois de uma conversa, há 50% de chance de chegar uma notificação: dinheiro (ex.: Pix da tia), coragem (áudio do melhor amigo), cupom de desconto para a próxima compra, ou uma dica sobre alguém com encontro marcado.

### Encontro

- Disponível quando o jogador marcou pelo menos um encontro. Sem encontros, o jogo vai direto para os finais 1 ou 2.
- Roupa, presente e local: +10 de satisfação para cada preferência atendida, −10 para cada erro.
- Correria: −10 de satisfação em **todos** os encontros para cada encontro além do primeiro. Com 2 encontros, −10; com 3, −20; com 4, −30.
- Perfume: +10 no encontro em que for usado.

### Finais

O final aparece depois da fase de encontro.

| Final | Condição |
|---|---|
| 1 – Melhor nem tentar | Terminou o dia sem iniciar nenhuma conversa |
| 2 – Não foi dessa vez | Nenhum encontro, mas iniciou pelo menos uma conversa |
| 3 – Almas gêmeas | Exatamente 1 encontro |
| 4 – Calma amor | Exatamente 2 encontros |
| 5 – São muitas namoradas | Exatamente 3 encontros |
| 6 – Não deu conta do recado | 4 encontros |

Os textos estão em `src/JogoTextual/Dados/finais.json` (iguais aos do documento da M2). Os finais alcançados ficam registrados na galeria.

## Condições de encerramento

- **Por escolha do jogador:** Sair → Sair do aplicativo.
- **Por limite de conversas:** quando não há mais conversas nem compra possível, aparece *"Oh, parece que você atingiu o seu limite de conversas no nosso aplicativo! Mas não se preocupe, é hora do encontro!"* e o jogo segue para a fase de encontro e um dos seis finais.
- **Perfis esgotados:** se todos os NPCs compatíveis já apareceram, o jogo avisa e segue para os encontros.

## Escolhas inválidas

Em qualquer menu, uma opção inexistente (número fora da lista, letra ou campo vazio) mostra *"Oops! Falha no nosso sistema, esta não é uma opção correspondente! :("* e a mesma pergunta é repetida. Nenhuma tela avança com uma entrada inválida.

## Coleções de dados

Todo o conteúdo fica em arquivos JSON na pasta `src/JogoTextual/Dados`, conferidos pelo jogo ao abrir:

| Arquivo | Conteúdo |
|---|---|
| `npcs.json` | Personagens: biografia, gênero, preferências, tópicos de conversa, dicas e loja |
| `itens.json` | Presentes e itens especiais |
| `encontro.json` | Roupas e locais |
| `flertes.json` | Cantadas (podem se repetir) |
| `eventos.json` | Notificações do celular |
| `finais.json` | Textos dos seis finais |

Guia para adicionar conteúdo: [como-adicionar-conteudo.md](como-adicionar-conteudo.md).

## Persistência

O arquivo `save_letsdate.json` guarda:

- **Perfil do jogador** (para não precisar refazer o login a cada vez que abre o jogo)
- **Conversas guardadas**: as que sobraram ao usar "Sair do aplicativo"; somadas às 3 do próximo dia, até 4
- **Dados fictícios de compra**: pedidos só na primeira compra
- **Descontos conquistados**
- **Histórico de avaliações** e **finais desbloqueados**

O estado do dia (conversas em andamento, carteira, inventário) não é salvo: cada abertura do aplicativo começa um dia novo.

## Exemplos de decisões e consequências

**Curto prazo:** as respostas aparecem embaralhadas e sem valor visível. O jogador precisa ler a biografia e o assunto para escolher bem. Gastar coragem num flerte pode render +100 ou −90 de interesse.

**Médio prazo:** convidar com 60 de interesse tem 60% de chance; continuar conversando aumenta a chance, mas os assuntos acabam e o flerte é arriscado. Gastar dinheiro com a conversa extra ou com um Super Like deixa menos dinheiro para presentes.

**Longo prazo:** cada encontro a mais aumenta a correria (−10 em todos) e muda o final. Conseguir encontros com donos de loja barateia presentes nos próximos dias.

## Limitações assumidas

- Máximo de quatro conversas por dia, o que permite uma coleção pequena de personagens (quatro por gênero).
- Interface de aplicativo no console, sem mapas ou exploração.
- O estado do dia não é salvo no meio da partida.

---

## Alterações da M2 para a M3

### Correções pedidas na devolutiva da M2

| Item da devolutiva | O que foi feito |
|---|---|
| Idade não numérica passava pelo login | A idade é lida em laço: texto, vazio ou acima de 100 pedem só a idade de novo; abaixo de 18 fecha o app. |
| Roupa, presente ou local inválidos viravam texto vazio e entravam no cálculo | `LeitorEscolhasEncontro` só devolve opções válidas; a satisfação é calculada depois das três escolhas. |
| Compra permitia nova conversa extra depois de gastar uma | `EstadoPartida.ConversaExtraComprada` registra a compra; a regra é uma conversa extra por dia, sem passar de 4. |
| Convite: código sorteava a partir de 60, slide dizia que era garantido | Mantida a regra do documento (sorteio a partir de 60, com chance igual ao interesse). Testes conferem 59, 60 e 100. **O slide precisa ser atualizado.** |
| `Jogo.cs` concentrava tudo; telas como texto livre | Código dividido em pastas; `enum Tela`; uma classe por tela; `Jogo` só faz as transições. |
| Faltava diagrama de classes | [arquitetura.md](arquitetura.md) |
| README breve e sem comandos | README com comandos de execução, testes, estrutura e links. |
| Envio original incluía `bin` | `bin` e `obj` removidos do repositório e adicionados ao `.gitignore`. |

### Correções no documento

- Exemplo da correria: com 3 encontros, a penalidade é de 20 pontos (−10 para cada encontro além do primeiro), e não 30.
- A condição de aceite usa "menor ou igual": com 100 de interesse o NPC sempre aceita.

### Novidades

- Carteira (R$ 70 por dia), loja com presentes e itens especiais, inventário e presentes entregues no encontro.
- Descontos permanentes ao conseguir encontro com NPCs donos de loja.
- Notificações do celular entre as conversas.
- Dois assuntos por NPC, respostas embaralhadas e anotações das preferências.
- Quatro NPCs masculinos (o filtro "O que você busca?" passa a funcionar), roupa Casual e local Cinema.
- Galeria de finais e histórico de avaliações com estrelas.
- Conteúdo em JSON, validado ao abrir o jogo.
- Testes automatizados (xUnit) e modo de partida repetível (`LETSDATE_SEMENTE`).

## Declaração de uso de Inteligência Artificial generativa

Veja [registro-uso-ia.md](registro-uso-ia.md).

## Página no itch.io

https://rauweiss.itch.io/lets-date
