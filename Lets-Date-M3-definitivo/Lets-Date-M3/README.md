# Let's Date!

> *"É dia dos namorados! Mas, infelizmente, você está sozinho... mas por pouco tempo. Com nosso novo app de encontros, o "Let's Date!", você terá um par até o fim do dia!"*

Jogo textual em C#/.NET para console, feito na disciplina de **Lógica de Programação** (Design de Games, 2026-02).

**Equipe:** Kimberly Bernardo da Silva, Lucas Gonçalves e Rau Guerreiro Weiss
**Professor:** Marcelo
**Página no itch.io:** https://rauweiss.itch.io/lets-date

## O jogo

Você usa um aplicativo de encontros durante um dia. Com **3 conversas** (4 com a compra da conversa extra), precisa escolher boas respostas, flertar com a sorte do D20 e decidir a hora certa de **convidar** alguém para sair. À noite, escolhe **roupa, presente e local** de cada encontro. Seis finais diferentes dependem de quantos encontros você conseguiu.

Novidades da M3: carteira e loja com presentes e itens especiais, inventário, anotações sobre as preferências de cada pessoa, notificações do celular, descontos permanentes nas lojas dos NPCs, oito personagens (quatro mulheres e quatro homens), galeria de finais e histórico de avaliações.

Regras completas: [docs/documento-m3.md](docs/documento-m3.md).

## Como executar

Requisitos: [.NET SDK 9](https://dotnet.microsoft.com/download) ou mais recente.

```powershell
git clone https://github.com/Zacariel/Lets-Date.git
cd Lets-Date
dotnet build
dotnet run --project src/JogoTextual
```

Se os acentos aparecerem errados no CMD, use o **Windows Terminal** ou rode `chcp 65001` antes.

### Testes automatizados

```powershell
dotnet test
```

São mais de 80 casos, incluindo os três cenários pedidos na devolutiva da M2 (idade e opções inválidas, compra repetida da conversa extra e convite com interesse 59, 60 e 100).

### Partida repetível (para a apresentação)

Defina a variável `LETSDATE_SEMENTE` para que os sorteios (NPCs, D20, convite e notificações) se repitam sempre iguais:

```powershell
$env:LETSDATE_SEMENTE = "7"
dotnet run --project src/JogoTextual
```

## Estrutura do projeto

```text
LetsDate.sln
src/JogoTextual/
├── Program.cs                  ponto de entrada: carrega dados e save e inicia o Jogo
├── Jogo.cs                     ciclo principal: só faz as transições entre telas
├── Dominio/                    classes de dados (estado, perfil, NPC, item, enums, resultados)
├── Dados/                      conteúdo em JSON + leitura e validação (CatalogoJogo)
│   ├── npcs.json               personagens, diálogos, preferências e lojas
│   ├── itens.json              presentes e itens especiais da loja
│   ├── encontro.json           roupas e locais
│   ├── flertes.json            cantadas sorteadas no flerte
│   ├── eventos.json            notificações do celular
│   └── finais.json             textos dos seis finais
├── Regras/                     regras puras e testáveis (conversa, loja, itens, encontro, finais, dia, eventos)
├── Telas/                      uma classe por tela (Login, Menu, Conversa, Loja, Encontros, Final...)
├── Interface/                  entrada validada e saída formatada do console
└── Persistencia/               save em JSON
tests/JogoTextual.Tests/        testes xUnit das regras, da entrada e dos dados
docs/                           documento do jogo, arquitetura, guia de conteúdo e roteiro de testes
```

## Adicionar NPCs e itens

Não é preciso mexer no código: basta editar os arquivos JSON da pasta `src/JogoTextual/Dados`. O jogo confere tudo ao abrir e avisa se alguma preferência, item ou id estiver errado. Passo a passo: [docs/como-adicionar-conteudo.md](docs/como-adicionar-conteudo.md).

## Documentação

- [Documento do jogo (M3)](docs/documento-m3.md): regras, finais e alterações desde a M2
- [Arquitetura e diagrama de classes](docs/arquitetura.md)
- [Como adicionar conteúdo](docs/como-adicionar-conteudo.md)
- [Roteiro de testes manuais](docs/roteiro-de-testes.md)
- [Registro de uso de IA](docs/registro-uso-ia.md)
- [Documento da M2 (histórico)](docs/documento-m2.md)
