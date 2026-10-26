using System;
using JogoTextual.Dominio;

namespace JogoTextual
{
    public class Jogo
    {
        private EstadoJogo _estado;
        private PersistenciaJogo _persistencia;

        public Jogo()
        {
            _persistencia = new PersistenciaJogo();
            _estado = _persistencia.Carregar();

            _estado.ConversasDisponiveis += 3;
            if (_estado.ConversasDisponiveis > 4) _estado.ConversasDisponiveis = 4;

            _estado.NpcsComEncontroMarcado.Clear();
            _estado.ConversasIniciadas = 0;
            _estado.Coragem = 100;
            if (_estado.TelaAtual == "Fim" || _estado.TelaAtual == "Encontro")
                _estado.TelaAtual = "MenuInicial";
        }

        public void Iniciar()
        {
            bool rodando = true;
            while (rodando)
            {
                switch (_estado.TelaAtual)
                {
                    case "Login":
                        ExecutarLogin();
                        break;
                    case "MenuInicial":
                        ExecutarMenuInicial();
                        break;
                    case "Conversa":
                        ExecutarConversa();
                        break;
                    case "Configuracoes":
                        ExecutarConfiguracoes();
                        break;
                    case "Historico":
                        ExecutarHistorico();
                        break;
                    case "Encontro":
                        ExecutarEncontros();
                        break;
                    case "Fim":
                        rodando = false;
                        _persistencia.Salvar(_estado);
                        Console.WriteLine("\nFechando o Let's Date... Até a próxima!");
                        break;
                }
            }
        }

        private void ExecutarLogin()
        {
            InterfaceConsole.MostrarCabecalho("Let's Date! - Login");
            Console.WriteLine("Com nosso novo app de encontros, o \"Let's Date!\", você terá um par até o fim do dia!\n");

            Console.Write("Digite seu Nome: ");
            _estado.Jogador.Nome = Console.ReadLine() ?? "";

            Console.Write("Digite seu Apelido: ");
            _estado.Jogador.Apelido = Console.ReadLine() ?? "";

            Console.Write("Digite sua Idade: ");
            if (int.TryParse(Console.ReadLine(), out int idade))
            {
                _estado.Jogador.Idade = idade;
                if (idade < 18)
                {
                    InterfaceConsole.MostrarErro("Oh, parece que você ainda é muito novo para usar nosso aplicativo! Volte em breve.");
                    _estado.TelaAtual = "Fim";
                    return;
                }
                if (idade > 100)
                {
                    InterfaceConsole.MostrarErro("Idade inválida.");
                    return;
                }
            }

            Console.Write("O que você busca? (Homem/Mulher/Ambos): ");
            _estado.Jogador.Buscando = Console.ReadLine() ?? "";

            Console.Write("Escreva sua Biografia (Max 100 caracteres): ");
            string bio = Console.ReadLine() ?? "";
            _estado.Jogador.Biografia = bio.Length > 100 ? bio.Substring(0, 100) : bio;

            _estado.TelaAtual = "MenuInicial";
        }

        private void ExecutarMenuInicial()
        {
            InterfaceConsole.MostrarCabecalho("Menu Inicial");
            Console.WriteLine($"Bem-vindo, {_estado.Jogador.Apelido}! Conversas disponíveis: {_estado.ConversasDisponiveis}\n");

            Console.WriteLine("1. Buscar encontro!");
            Console.WriteLine("2. Ir para Encontros");
            Console.WriteLine("3. Configurações da Conta");
            Console.WriteLine("4. Avaliações (Estrelas)");
            Console.WriteLine("5. Salvar Progresso");
            Console.WriteLine("6. Carregar Progresso");
            Console.WriteLine("7. Sair do App");
            Console.Write("\nEscolha: ");

            string? escolha = Console.ReadLine();

            if (escolha == "1")
            {
                if (_estado.ConversasDisponiveis > 0)
                {
                    _estado.NpcAtual = BancoNpcs.SortearNpc(_estado.NpcsJaVistos);

                    if (_estado.NpcAtual == null)
                    {
                        InterfaceConsole.MostrarErro("Acabaram as pessoas na sua região! Indo para os encontros...");
                        Console.ReadLine();
                        _estado.TelaAtual = "Encontro";
                        return;
                    }

                    _estado.ConversasDisponiveis--;
                    _estado.ConversasIniciadas++;
                    _estado.NpcsJaVistos.Add(_estado.NpcAtual.Nome);
                    _estado.InteresseAtualNpc = 20;
                    _estado.TelaAtual = "Conversa";
                }
                else
                {
                    Console.WriteLine("\nVocê pode ter mais uma conversa! Deseja compra-la na loja? (SIM/NÃO)");
                    if (Console.ReadLine()?.ToUpper().Trim() == "SIM")
                    {
                        ExecutarCompra();
                    }
                    else
                    {
                        Console.WriteLine("Oh, parece que você atingiu o seu limite de conversas no nosso aplicativo! Mas não se preocupe, é hora do encontro!");
                        Console.ReadLine();
                        _estado.TelaAtual = "Encontro";
                    }
                }
            }
            else if (escolha == "2") { _estado.TelaAtual = "Encontro"; }
            else if (escolha == "3") { _estado.TelaAtual = "Configuracoes"; }
            else if (escolha == "4") { _estado.TelaAtual = "Historico"; }
            else if (escolha == "5")
            {
                _persistencia.Salvar(_estado);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nProgresso salvo com sucesso!");
                Console.ResetColor();
                Console.ReadLine();
            }
            else if (escolha == "6")
            {
                _estado = _persistencia.Carregar();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nProgresso carregado com sucesso!");
                Console.ResetColor();
                Console.ReadLine();
            }
            else if (escolha == "7") { _estado.TelaAtual = "Fim"; }
            else
            {
                InterfaceConsole.MostrarErro("Oops! Falha no nosso sistema, esta não é uma opção correspondente! :(");
                Console.ReadLine();
            }
        }

        private void ExecutarHistorico()
        {
            InterfaceConsole.MostrarCabecalho("Avaliações dos seus Encontros");
            if (_estado.HistoricoAvaliacoes.Count == 0)
            {
                Console.WriteLine("Você ainda não foi a nenhum encontro para receber avaliações.");
            }
            else
            {
                foreach (var avaliacao in _estado.HistoricoAvaliacoes)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(avaliacao);
                    Console.ResetColor();
                }
            }
            Console.WriteLine("\nAperte ENTER para voltar...");
            Console.ReadLine();
            _estado.TelaAtual = "MenuInicial";
        }

        private void ExecutarConfiguracoes()
        {
            InterfaceConsole.MostrarCabecalho("Configurações do Perfil");
            Console.WriteLine($"Nome: {_estado.Jogador.Nome}");
            Console.WriteLine($"Apelido: {_estado.Jogador.Apelido}");
            Console.WriteLine($"Idade: {_estado.Jogador.Idade}");
            Console.WriteLine($"Buscando: {_estado.Jogador.Buscando}");
            Console.WriteLine($"Biografia: {_estado.Jogador.Biografia}\n");

            Console.WriteLine("1. Editar Perfil");
            Console.WriteLine("2. Comprar mais conversas");
            Console.WriteLine("3. Voltar ao Menu");
            Console.Write("\nEscolha: ");
            string escolha = Console.ReadLine() ?? "";

            if (escolha == "1")
            {
                Console.Write("\nNovo Apelido: ");
                _estado.Jogador.Apelido = Console.ReadLine() ?? _estado.Jogador.Apelido;
                Console.Write("Nova Biografia: ");
                string bio = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(bio))
                    _estado.Jogador.Biografia = bio.Length > 100 ? bio.Substring(0, 100) : bio;
                Console.WriteLine("Perfil atualizado!");
                Console.ReadLine();
            }
            else if (escolha == "2") { ExecutarCompra(); }
            else if (escolha == "3") { _estado.TelaAtual = "MenuInicial"; }
        }

        private void ExecutarCompra()
        {
            InterfaceConsole.MostrarCabecalho("Loja");
            Console.Write("Confirme seu nome: ");
            string nomeConfirmacao = Console.ReadLine() ?? "";

            if (!nomeConfirmacao.Trim().Equals(_estado.Jogador.Nome.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                InterfaceConsole.MostrarErro("Informações incorretas! Preencha seu nome novamente.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Você deseja comprar mais uma conversa por R$5,00? (SIM/NÃO)");
            if (Console.ReadLine()?.ToUpper().Trim() == "SIM")
            {
                if (_estado.ConversasDisponiveis >= 4)
                {
                    InterfaceConsole.MostrarErro("Parece que você atingiu o número máximo de conversas! Considere conversar um pouco antes de voltar aqui ;)");
                }
                else
                {
                    _estado.ConversasDisponiveis++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Compra finalizada com sucesso! Agora você tem mais uma conversa :D");
                    Console.ResetColor();
                }
            }
            else
            {
                InterfaceConsole.MostrarErro("Compra não finalizada!");
            }

            Console.WriteLine("\nAperte ENTER para voltar...");
            Console.ReadLine();
        }

        private void ExecutarConversa()
        {
            Npc npc = _estado.NpcAtual!;
            string apelido = _estado.Jogador.Apelido; // Pegamos o apelido pra usar na fala

            if (_estado.InteresseAtualNpc <= -60)
            {
                InterfaceConsole.MostrarErro($"Você foi bloqueado por {npc.Nome} devido ao baixo interesse (-60).");
                Console.ReadLine();
                _estado.NpcAtual = null;
                _estado.TelaAtual = "MenuInicial";
                return;
            }

            InterfaceConsole.MostrarCabecalho($"Conversando com {npc.Nome}");
            Console.WriteLine($"Idade: {npc.Idade}");
            Console.WriteLine($"Bio: {npc.Biografia}\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Interesse: {_estado.InteresseAtualNpc} | Coragem: {_estado.Coragem}");
            Console.ResetColor();

            Console.WriteLine("\n1. CONVERSAR");
            Console.WriteLine("2. FLERTE");
            Console.WriteLine("3. CHAMAR PARA ENCONTRO");
            Console.WriteLine("4. BLOQUEAR");

            Console.Write("\nEscolha: ");
            string escolha = Console.ReadLine() ?? "";

            if (escolha == "1")
            {
                Console.WriteLine("\nOpções de Diálogo:");
                Console.WriteLine($"1. {npc.PerguntaBom}");
                Console.WriteLine($"2. {npc.PerguntaRegular}");
                Console.WriteLine($"3. {npc.PerguntaRuim}");
                Console.WriteLine($"4. {npc.PerguntaPessimo}");
                Console.Write("Escolha uma fala: ");
                string fala = Console.ReadLine() ?? "";

                // Injetamos o nome do jogador no final da resposta pra ficar natural
                if (fala == "1") { Console.WriteLine($"\n{npc.Nome}: \"{npc.RespostaBom} né, {apelido}?\" (+40 Interesse)"); RegrasJogo.AlterarInteresse(_estado, 40); }
                else if (fala == "2") { Console.WriteLine($"\n{npc.Nome}: \"{npc.RespostaRegular} {apelido}.\" (+20 Interesse)"); RegrasJogo.AlterarInteresse(_estado, 20); }
                else if (fala == "3") { Console.WriteLine($"\n{npc.Nome}: \"{npc.RespostaRuim} Não acha, {apelido}?\" (-10 Interesse)"); RegrasJogo.AlterarInteresse(_estado, -10); }
                else if (fala == "4") { Console.WriteLine($"\n{npc.Nome}: \"{npc.RespostaPessimo} Se liga, {apelido}!\" (-30 Interesse)"); RegrasJogo.AlterarInteresse(_estado, -30); }
            }
            else if (escolha == "2")
            {
                int resultado = RegrasJogo.Flertar(_estado);
                if (resultado == -999)
                {
                    InterfaceConsole.MostrarErro("Oh, parece que você não tem coragem o suficiente para flertar agora...");
                }
                else
                {
                    Console.WriteLine($"\nVocê rolou os dados e o interesse mudou em {resultado}!");
                }
            }
            else if (escolha == "3")
            {
                bool aceitou = RegrasJogo.ChamarParaEncontro(_estado);
                if (aceitou)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n{npc.Nome}: \"Claro {apelido}, eu adoraria!\"");
                    Console.ResetColor();
                }
                else
                {
                    InterfaceConsole.MostrarErro($"\n{npc.Nome}: \"Acho melhor não, {apelido}...\"");
                }

                Console.ReadLine();
                _estado.NpcAtual = null;
                _estado.TelaAtual = "MenuInicial";
                return;
            }
            else if (escolha == "4")
            {
                Console.Write($"\nTem certeza que deseja bloquear {npc.Nome}? (SIM/NÃO): ");
                if (Console.ReadLine()?.ToUpper().Trim() == "SIM")
                {
                    _estado.NpcAtual = null;
                    _estado.TelaAtual = "MenuInicial";
                    return;
                }
            }

            Console.WriteLine("\nAperte ENTER para continuar...");
            Console.ReadLine();
        }

        private void ExecutarEncontros()
        {
            InterfaceConsole.MostrarCabecalho("Fase de Encontros");

            if (_estado.NpcsComEncontroMarcado.Count == 0)
            {
                ExecutarFinais();
                return;
            }

            Console.WriteLine("Após algumas horas no aplicativo buscando por encontros, finalmente chegou o momento de começar a noite, será o seu melhor dia dos namorados!\n");

            foreach (var npc in _estado.NpcsComEncontroMarcado)
            {
                Console.WriteLine($"\n--- Preparando encontro com {npc.Nome} ---");

                Console.WriteLine("\nO que vai vestir?");
                Console.WriteLine("1. Clássico");
                Console.WriteLine("2. Elegante");
                Console.WriteLine("3. Esportivo");
                Console.WriteLine("4. Romântico");
                Console.WriteLine("5. Sexy");
                Console.Write("Escolha (1-5): ");
                string escolhaRoupa = Console.ReadLine() ?? "";
                string roupa = escolhaRoupa == "1" ? "Clássico" : escolhaRoupa == "2" ? "Elegante" : escolhaRoupa == "3" ? "Esportivo" : escolhaRoupa == "4" ? "Romântico" : escolhaRoupa == "5" ? "Sexy" : "";

                Console.WriteLine("\nVamos comprar presentes!");
                Console.WriteLine("1. Chocolates");
                Console.WriteLine("2. Flores");
                Console.WriteLine("3. Vinhos");
                Console.WriteLine("4. Livros");
                Console.Write("Escolha (1-4): ");
                string escolhaPresente = Console.ReadLine() ?? "";
                string presente = escolhaPresente == "1" ? "Chocolates" : escolhaPresente == "2" ? "Flores" : escolhaPresente == "3" ? "Vinhos" : escolhaPresente == "4" ? "Livros" : "";

                Console.WriteLine("\nMelhor escolher um bom lugar.");
                Console.WriteLine("1. Restaurante");
                Console.WriteLine("2. Praça");
                Console.WriteLine("3. Praia");
                Console.WriteLine("4. Parque");
                Console.Write("Escolha (1-4): ");
                string escolhaLocal = Console.ReadLine() ?? "";
                string local = escolhaLocal == "1" ? "Restaurante" : escolhaLocal == "2" ? "Praça" : escolhaLocal == "3" ? "Praia" : escolhaLocal == "4" ? "Parque" : "";

                int satisfacao = RegrasJogo.CalcularSatisfacaoEncontro(npc, roupa, presente, local, _estado.NpcsComEncontroMarcado.Count);

                string estrelas = "";
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (satisfacao >= 81) { Console.WriteLine($"\n{npc.Nome} adorou o encontro! ({satisfacao} pts)"); estrelas = "★★★★★"; }
                else if (satisfacao >= 61) { Console.WriteLine($"\n{npc.Nome} gostou do encontro! ({satisfacao} pts)"); estrelas = "★★★★☆"; }
                else if (satisfacao >= 41) { Console.WriteLine($"\n{npc.Nome} achou o encontro normal. ({satisfacao} pts)"); estrelas = "★★★☆☆"; }
                else if (satisfacao >= 21) { Console.WriteLine($"\n{npc.Nome} não gostou do encontro... ({satisfacao} pts)"); estrelas = "★★☆☆☆"; }
                else { Console.WriteLine($"\n{npc.Nome} odiou o encontro! ({satisfacao} pts)"); estrelas = "★☆☆☆☆"; }

                // Salva a avaliação pro menu principal
                _estado.HistoricoAvaliacoes.Add($"{npc.Nome}: {estrelas} ({satisfacao} pts)");
                Console.ResetColor();

                Console.WriteLine("\nAperte ENTER para continuar...");
                Console.ReadLine();
            }

            ExecutarFinais();
        }

        private void ExecutarFinais()
        {
            InterfaceConsole.MostrarCabecalho("Fim da Noite");
            int encontros = _estado.NpcsComEncontroMarcado.Count;

            if (encontros == 0)
            {
                if (_estado.ConversasIniciadas == 0)
                {
                    Console.WriteLine("Final 1 - Melhor nem tentar: Você rolou pelas opções do aplicativo sem sequer iniciar uma conversa. Entediado, fechou as abas abertas no celular e voltou a atenção para o mundo real ao seu redor, uma breve olhada no ambiente te fez perceber que tem muita coisa em casa pra ser feita ao invés de procurar por gente. Suspirando, você se encosta no sofá e aumenta o volume da TV.");
                }
                else
                {
                    Console.WriteLine("Final 2 - Não foi dessa vez: Depois de diálogos desconfortáveis e rejeições dolorosas durante toda a vida, não deveria ser tão humilhante dessa vez! O aplicativo ainda está aberto no celular deixado de lado, você desliza as costas pelo sofá até chegar no chão gelado. Olhando para o teto meio sujo, você finaliza a noite em reflexão profunda.");
                }
            }
            else if (encontros == 1)
            {
                Console.WriteLine("Final 3 - Almas gêmeas: Você podia sentir o suor grudar em todas as partes do seu corpo, a conversa foi agradável e não havia motivos para nervosismo, você até se sentia confiante, mas as flores começando a murchar na sua mão parecem no mínimo um sinal visual de que as coisas não vão sair como o esperado. Com atenção você caminha até o local marcado com antecedência. Uma voz suave chama sua atenção, e na primeira vez que você observou aquele belo olhar brilhante, soube que estava perdido.");
            }
            else if (encontros == 2)
            {
                Console.WriteLine("Final 4 - Calma amor: OK, não era de se esperar, mas agora você estava mais do que confiante! Você veste a melhor roupa, passa o perfume mais cheiroso, escova bem os dentes e usa o resto do seu tempo disponível para treinar algumas falas e... outras coisas em frente ao espelho. Com animação, você sai pela porta da frente pronto para arrasar corações. É bom dar conta do recado.");
            }
            else if (encontros == 3)
            {
                Console.WriteLine("Final 5 - São muitas namoradas: Talvez tenha sido exagero da sua parte procurar por tantos encontros, quer dizer, você estava desesperado, mas nem tanto! Agora não havia como voltar atrás, você se arruma e sai rapidamente confiante das suas habilidades sociais, mas infelizmente isso não ia te salvar do fator tempo. Quem diria que marcar 3 diferentes encontros de 40 minutos cada não daria tão certo? Talvez alguém ainda fale com você depois de uma saída medíocre");
            }
            else
            {
                Console.WriteLine("Final 6 - Não deu conta do recado: Você mentiria se dissesse que não sabe o que levou você até esse momento, sentado no meio fio em frente a sua casa, sozinho. Recapitulando as últimas horas, você não sabe dizer quando realmente as coisas começaram a dar errado. Será que foi quando confundiu os nomes das pessoas com quem ia sair? Os presentes entregues errado? O restaurante repetido? Ou todos terem descoberto seu planejamento (quase) inexistente? Infelizmente, não há nada que você possa fazer no momento.");
            }

            Console.WriteLine("\n------------------------------------------------");
            Console.WriteLine("Deseja tentar outro Dia dos Namorados no Let's Date?");
            Console.WriteLine("1. Sim, recomeçar o dia! (Volta ao Menu Principal)");
            Console.WriteLine("2. Não, fechar o aplicativo.");
            Console.Write("Escolha: ");

            if (Console.ReadLine() == "1")
            {
                // Reseta a corrida diária
                _estado.Coragem = 100;
                _estado.ConversasDisponiveis = 4;
                _estado.ConversasIniciadas = 0;
                _estado.NpcsComEncontroMarcado.Clear();
                _estado.NpcAtual = null;
                // Mantém NpcsJaVistos para não repetir as garotas e HistoricoAvaliacoes pra ver as estrelas depois

                _estado.TelaAtual = "MenuInicial";
            }
            else
            {
                _estado.TelaAtual = "Fim";
            }
        }
    }
}