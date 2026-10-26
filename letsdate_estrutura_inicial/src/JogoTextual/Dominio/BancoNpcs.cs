using System;
using System.Collections.Generic;
using JogoTextual.Dominio;

namespace JogoTextual
{
    public class BancoNpcs
    {
        public static Npc SortearNpc(List<string> jaVistos)
        {
            List<Npc> todos = new List<Npc>
            {
                new Npc {
                    Nome = "Maria Antônia", Idade = 27,
                    Biografia = "Estudante de veterinária 6/10. AMO meus cachorros e ADORO aproveitar a natureza!! Procurando um relacionamento sério! ^^",
                    VestuarioPreferido = "Esportivo", PresentePreferido = "Flores", LocalPreferido = "Praça",
                    PerguntaBom = "Eu também amo cachorros! Na verdade, adoro todos os animais",
                    RespostaBom = "SIMM eles também são bonitinhos demais! Compartilhamos essa paixão em comum ^^",
                    PerguntaRegular = "Admito que prefiro gatos, mas os cachorros tem seu charme...",
                    RespostaRegular = "Tudo bem, eles também são adoráveis ^^ aprendi a gostar de todos os animais",
                    PerguntaRuim = "Eu realmente não gosto de cachorros, como você lida com os pelos e a baba?",
                    RespostaRuim = "Eu realmente amo eles, isso são detalhes irrelevantes! Temos interesses bem diferentes",
                    PerguntaPessimo = "Não era melhor ter cursado medicina de verdade?",
                    RespostaPessimo = "Eu curso medicina de verdade! Com essa mentalidade, é melhor você não ter animais..."
                },
                new Npc {
                    Nome = "Alana Martins", Idade = 19,
                    Biografia = "Procuro alguém pra me divertir, sair pra beber e passar altas noites fora! Conheço a cidade toda e posso te levar para várias baladas diferentes!!",
                    VestuarioPreferido = "Sexy", PresentePreferido = "Vinhos", LocalPreferido = "Restaurante",
                    PerguntaBom = "Woww você conhece todos as baladas mesmo?? Preciso de uma boa companhia pra me mostrar as melhores",
                    RespostaBom = "SIMM HAHAHA é um talento não tão secreto meu >:D dependendo do seu estilo te levo nos lugares mais INSANOS",
                    PerguntaRegular = "Admito que eu sou do mais do tipo caseiro, não frequento muitas baladas",
                    RespostaRegular = "Acontece, tem lugar pra todo tipo de gente! A vezes eu NECESSITO de um ambiente com gente e música alta",
                    PerguntaRuim = "Não curto muito essas pessoas festeiras, você não consegue fazer outras coisas?",
                    RespostaRuim = "Você fala SEM me conhecer! Tenho minhas preferências, você tem as suas",
                    PerguntaPessimo = "19 anos já devia agir como adulta, quando você vai deixar de ser adolescente?",
                    RespostaPessimo = "ISSO NÃO TEM NADA A VER! Você nem me conhece para assumir algo assim, se toca!"
                },
                new Npc {
                    Nome = "Roberta Cunha", Idade = 30,
                    Biografia = "Uma apreciadora da boa literatura, amante de filmes dramáticos e dona de um pequeno negócio. Nada melhor do que aproveitar o tempo livre com muita cultura ou longas caminhadas pela beira do mar.",
                    VestuarioPreferido = "Elegante", PresentePreferido = "Livros", LocalPreferido = "Praia",
                    PerguntaBom = "Adoro o poder de um bom livro em nos transportar para realidades alternativas.",
                    RespostaBom = "Ah, finalmente alguém para compartilhar esta paixão! Realmente, não é fascinante o que algumas histórias conseguem fazer conosco?",
                    PerguntaRegular = "Eu não costumo ler muito, mas assisto muitos filmes é séries no tempo livre.",
                    RespostaRegular = "Todos temos nossa forma de relaxar, não há nada errado nisso, mas talvez só te falta encontrar uma boa história.",
                    PerguntaRuim = "Pra que ler tantos livros se podemos achar tudo na internet hoje em dia?",
                    RespostaRuim = "O acesso fácil a informação não significa que todas as formas de conhecimento são iguais! O habito da leitura nos trás outros benefícios além do acesso a informação.",
                    PerguntaPessimo = "Quem passa muito tempo lendo parece querer compensar algo que falta na vida real.",
                    RespostaPessimo = "Sua conclusão é muito superficial. A leitura amplia nosso conhecimento sobre todas as áreas, não substitui nenhuma experencia real."
                },
                new Npc {
                    Nome = "Kayla Schrute", Idade = 35,
                    Biografia = "A mais de 15 anos no mesmo ramo! Busco uma pessoa descente para me relacionar e seguir a linhagem da família Schrute. Fazendas, tratores, beterrabas, e diversos outros bens para te impressionar.",
                    VestuarioPreferido = "Clássico", PresentePreferido = "Chocolates", LocalPreferido = "Restaurante",
                    PerguntaBom = "Poucas pessoas tem tanta disciplina para seguir no mesmo ramo por tanto tempo.",
                    RespostaBom = "Aparentemente você tem capacidade para observar os fatos. Outros desperdiçam seu tempo procurando um propósito para vida.",
                    PerguntaRegular = "Beterrabas são interessantes, acho.",
                    RespostaRegular = "Interessantes? São o sustento de uma linhagem inteira.",
                    PerguntaRuim = "Não gosto muito de fazendas, suja muito a roupa.",
                    RespostaRuim = "Você não sobreviveria um dia no inverno rigoroso.",
                    PerguntaPessimo = "Você parece meio maluca com essas coisas de beterraba.",
                    RespostaPessimo = "Falso. Sou perfeitamente sã. Você que é ignorante."
                }
            };

            // Filtra os que não foram vistos ainda[cite: 65]
            List<Npc> disponiveis = new List<Npc>();
            foreach (var npc in todos)
            {
                if (!jaVistos.Contains(npc.Nome))
                {
                    disponiveis.Add(npc);
                }
            }

            if (disponiveis.Count == 0) return null; // Acabaram as opções

            int index = new Random().Next(disponiveis.Count);
            return disponiveis[index];
        }
    }
}