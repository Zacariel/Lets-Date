using JogoTextual.Dominio;
using JogoTextual.Regras;

namespace JogoTextual.Telas
{
    /// Configurações: ver o perfil, alterar apelido, biografia e busca, e comprar conversas.
    public class TelaPerfil : ITelaJogo
    {
        public Tela Executar(ContextoJogo c)
        {
            var perfil = c.Perfil;
            c.Saida.Cabecalho("Configurações");
            c.Saida.Texto($"Nome:      {perfil.Nome} (não pode ser alterado)");
            c.Saida.Texto($"Idade:     {perfil.Idade} (não pode ser alterada)");
            c.Saida.Texto($"Apelido:   {perfil.Apelido}");
            c.Saida.Texto($"Busca:     {perfil.Busca}");
            c.Saida.Texto($"Biografia: {(string.IsNullOrWhiteSpace(perfil.Biografia) ? "(vazia)" : perfil.Biografia)}");
            if (c.Salvos.DadosCompra != null)
            {
                c.Saida.Texto($"Cartão:    {c.Salvos.DadosCompra.ApelidoCartao} (fictício)");
            }

            c.Saida.Texto();
            c.Saida.Menu(new[]
            {
                (1, "Alterar apelido"),
                (2, "Alterar biografia"),
                (3, "Alterar o que você busca"),
                (4, "Comprar mais conversas!"),
                (0, "Retornar")
            });

            switch (c.Entrada.LerOpcao(0, 4))
            {
                case 1:
                    perfil.Apelido = c.Entrada.LerTexto("Novo apelido: ", 20);
                    Confirmar(c);
                    return Tela.Perfil;
                case 2:
                    perfil.Biografia = c.Entrada.LerTexto(
                        $"Nova biografia (até {Constantes.TamanhoMaximoBiografia} caracteres): ", Constantes.TamanhoMaximoBiografia, obrigatorio: false);
                    Confirmar(c);
                    return Tela.Perfil;
                case 3:
                    c.Saida.Menu(new[] { (1, "Homem"), (2, "Mulher"), (3, "Ambos") });
                    perfil.Busca = c.Entrada.LerOpcao(1, 3) switch
                    {
                        1 => Preferencia.Homem,
                        2 => Preferencia.Mulher,
                        _ => Preferencia.Ambos
                    };
                    Confirmar(c);
                    return Tela.Perfil;
                case 4:
                    FluxoCompraConversa.Executar(c);
                    return Tela.Perfil;
                default:
                    return Tela.MenuInicial;
            }
        }

        private static void Confirmar(ContextoJogo c)
        {
            c.Salvar();
            c.Saida.Sucesso("Perfil atualizado!");
            c.Entrada.Pausar();
        }
    }
}
