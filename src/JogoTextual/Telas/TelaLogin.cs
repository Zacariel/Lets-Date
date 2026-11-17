using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// <summary>Login: cria o perfil (ou continua o perfil salvo) e começa o dia.</summary>
    public class TelaLogin : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            c.Saida.Cabecalho("Let's Date!");
            c.Saida.Narracao("É dia dos namorados! Mas, infelizmente, você está sozinho... mas por pouco tempo.");
            c.Saida.Texto("Com nosso novo app de encontros, o \"Let's Date!\", você terá um par até o fim do dia!");
            c.Saida.Texto();

            if (c.Salvos.Perfil != null)
            {
                return ContinuarPerfil(c);
            }

            c.Saida.Menu(new[] { (1, "Começar!"), (0, "Sair") });
            if (c.Entrada.LerOpcao(new[] { 1, 0 }) == 0) return Tela.Encerrar;

            return CriarPerfil(c);
        }

        private Tela ContinuarPerfil(ContextoJogo c)
        {
            var perfil = c.Salvos.Perfil!;
            c.Saida.Texto($"Perfil salvo encontrado: {perfil.Apelido} ({perfil.Nome}).");
            if (c.Salvos.ConversasGuardadas > 0)
            {
                c.Saida.Destaque($"Você guardou {c.Salvos.ConversasGuardadas} conversa(s) da última vez.");
            }
            c.Saida.Texto();
            c.Saida.Menu(new[] { (1, $"Continuar como {perfil.Apelido}"), (2, "Criar um perfil novo (apaga o progresso)"), (0, "Sair") });

            switch (c.Entrada.LerOpcao(new[] { 1, 2, 0 }))
            {
                case 1:
                    return ComecarDia(c);
                case 2:
                    if (!c.Entrada.LerSimNao("Isso apaga avaliações, finais e descontos conquistados. Tem certeza?"))
                        return Tela.Login;
                    c.Salvos = new DadosSalvos();
                    return CriarPerfil(c);
                default:
                    return Tela.Encerrar;
            }
        }

        private Tela CriarPerfil(ContextoJogo c)
        {
            c.Saida.Secao("Informações pessoais");

            var perfil = new PerfilJogador
            {
                Nome = c.Entrada.LerTexto("Nome: ", 40),
                Apelido = c.Entrada.LerTexto("Apelido (é assim que vão te chamar): ", 20)
            };

            int? idade = LerIdade(c);
            if (idade == null)
            {
                c.Saida.Erro("Oh, parece que você ainda é muito novo para usar nosso aplicativo! Volte em breve.");
                c.Entrada.Pausar("Aperte ENTER para fechar...");
                return Tela.Encerrar;
            }
            perfil.Idade = idade.Value;

            c.Saida.Texto("O que você busca?");
            c.Saida.Menu(new[] { (1, "Homem"), (2, "Mulher"), (3, "Ambos") });
            perfil.Busca = c.Entrada.LerOpcao(1, 3) switch
            {
                1 => Preferencia.Homem,
                2 => Preferencia.Mulher,
                _ => Preferencia.Ambos
            };

            perfil.Biografia = c.Entrada.LerTexto(
                $"Biografia (até {Constantes.TamanhoMaximoBiografia} caracteres): ", Constantes.TamanhoMaximoBiografia, obrigatorio: false);

            c.Salvos.Perfil = perfil;
            return ComecarDia(c);
        }

        /// <summary>
        /// Pede a idade até receber um número inteiro de 18 a 100.
        /// Devolve null quando a idade é menor que 18 (o aplicativo fecha).
        /// </summary>
        private static int? LerIdade(ContextoJogo c)
        {
            while (true)
            {
                int idade = c.Entrada.LerInteiro("Idade: ", "Digite sua idade usando apenas números inteiros.");
                if (idade < Constantes.IdadeMinima) return null;
                if (idade > Constantes.IdadeMaxima)
                {
                    c.Saida.OpcaoInvalida();
                    continue;
                }
                return idade;
            }
        }

        private static Tela ComecarDia(ContextoJogo c)
        {
            c.Partida = RegrasDia.NovoDia(c.Salvos);
            c.Salvos.Perfil ??= new PerfilJogador();
            c.Salvar();

            c.Saida.Texto();
            c.Saida.Sucesso($"Tudo pronto, {c.Perfil.Apelido}! Hoje você tem {c.Partida.Conversas} conversas e R$ {c.Partida.Dinheiro} na carteira.");
            c.Entrada.Pausar();
            return Tela.MenuInicial;
        }
    }
}
