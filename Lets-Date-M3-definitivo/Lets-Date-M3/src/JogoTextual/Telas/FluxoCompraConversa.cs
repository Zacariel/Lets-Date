//  1. "Confirme seu nome" + dados do cartão só na PRIMEIRA compra (o save guarda os dados).
//     Da segunda em diante é só um SIM/NÃO.
//  2. Quando o jogador já disse SIM no aviso "Você pode ter mais uma conversa!",
//     não pergunta de novo.
using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Compra da conversa extra ("Comprar mais conversas!"). É usada pela Loja,
    /// pelas Configurações e pelo aviso que aparece quando as conversas acabam.
    public static class FluxoCompraConversa
    {
        /// Devolve verdadeiro se a conversa foi comprada.
        /// <param name="jaConfirmou">verdadeiro quando o jogador já respondeu SIM antes de chegar aqui.</param>
        public static bool Executar(ContextoJogo c, bool jaConfirmou = false)
        {
            c.Saida.Cabecalho("Comprar mais conversas!");

            var verificacao = RegrasLoja.PodeComprarConversaExtra(c.Partida);
            if (verificacao != ResultadoCompraConversa.Sucesso)
            {
                MostrarResultado(c, verificacao);
                c.Entrada.Pausar();
                return false;
            }

            string nome;
            DadosCompra? dadosNovos = null;

            if (c.Salvos.DadosCompra == null)
            {
                // Primeira compra: confirma o nome e pede os dados fictícios (fica tudo salvo).
                c.Saida.Narracao("Compra simulada: use apenas dados fictícios.");
                c.Saida.Texto();

                while (true)
                {
                    nome = c.Entrada.LerTexto("Confirme seu nome (ou 0 para cancelar): ");
                    if (nome == "0")
                    {
                        MostrarResultado(c, ResultadoCompraConversa.Cancelada);
                        c.Entrada.Pausar();
                        return false;
                    }
                    if (RegrasLoja.NomeConfere(c.Perfil, nome)) break;
                    MostrarResultado(c, ResultadoCompraConversa.NomeIncorreto);
                }

                dadosNovos = new DadosCompra
                {
                    Titular = c.Entrada.LerTexto("Nome do titular (fictício): ", 40),
                    ApelidoCartao = c.Entrada.LerTexto("Apelido do cartão (fictício, ex.: \"Cartão do amor\"): ", 30)
                };
            }
            else
            {
                // compras seguintes usam o cartão salvo, sem pedir o nome de novo.
                nome = c.Perfil.Nome;
                c.Saida.Texto($"Usando o cartão salvo: {c.Salvos.DadosCompra.ApelidoCartao}.");
            }

            bool confirmou = jaConfirmou || c.Entrada.LerSimNao($"Você deseja comprar mais uma conversa por R$ {Constantes.PrecoConversaExtra},00?");
            var resultado = RegrasLoja.ComprarConversaExtra(c.Partida, c.Salvos, nome, confirmou, dadosNovos);
            MostrarResultado(c, resultado);
            c.Salvar();
            c.Entrada.Pausar();
            return resultado == ResultadoCompraConversa.Sucesso;
        }

        public static void MostrarResultado(ContextoJogo c, ResultadoCompraConversa resultado)
        {
            switch (resultado)
            {
                case ResultadoCompraConversa.Sucesso:
                    c.Saida.Sucesso("Compra finalizada com sucesso! Agora você tem mais uma conversa :D");
                    break;
                case ResultadoCompraConversa.NomeIncorreto:
                    c.Saida.Erro("Informações incorretas! Preencha seu nome novamente.");
                    break;
                case ResultadoCompraConversa.JaComprouHoje:
                    c.Saida.Aviso("Você já usou sua conversa extra de hoje! Ela só pode ser comprada uma vez por dia.");
                    break;
                case ResultadoCompraConversa.LimiteAtingido:
                    c.Saida.Aviso("Parece que você atingiu o número máximo de conversas! Considere conversar um pouco antes de voltar aqui ;)");
                    break;
                case ResultadoCompraConversa.SemDinheiro:
                    c.Saida.Erro($"Saldo insuficiente! A conversa extra custa R$ {Constantes.PrecoConversaExtra}.");
                    break;
                default:
                    c.Saida.Aviso("Compra não finalizada!");
                    break;
            }
        }
    }

    /// O que acontece quando as conversas acabam: se a conversa extra ainda puder ser comprada,
    /// aparece o aviso de compra; senão, o dia segue direto para os encontros.
    public static class FluxoFimDasConversas
    {
        public static Tela Executar(ContextoJogo c)
        {
            if (RegrasLoja.PodeComprarConversaExtra(c.Partida) == ResultadoCompraConversa.Sucesso)
            {
                c.Saida.Cabecalho("Let's Date!");
                c.Saida.Destaque("Você pode ter mais uma conversa! Deseja comprá-la na loja?");
                if (c.Entrada.LerSimNao("") && FluxoCompraConversa.Executar(c, jaConfirmou: true)) // NOVO
                {
                    return Tela.MenuInicial;
                }
            }

            c.Saida.Cabecalho("Let's Date!");
            c.Saida.Aviso("Oh, parece que você atingiu o seu limite de conversas no nosso aplicativo! Mas não se preocupe, é hora do encontro!");
            c.Entrada.Pausar();
            return Tela.Encontros;
        }
    }
}
