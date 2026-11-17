using System;
using System.IO;
using System.Text.Json;
using JogoTextual.Dados;
using JogoTextual.Dominio;

namespace JogoTextual.Persistencia
{
    /// Salva e carrega o arquivo save_letsdate.json: perfil, conversas guardadas,
    /// dados fictícios de compra, descontos conquistados, avaliações e finais.
    public class PersistenciaJogo
    {
        public string Caminho { get; }

        public PersistenciaJogo(string? caminho = null)
        {
            Caminho = caminho ?? Path.Combine(AppContext.BaseDirectory, "save_letsdate.json");
        }

        public bool ExisteSave => File.Exists(Caminho);

        public void Salvar(DadosSalvos dados)
        {
            string json = JsonSerializer.Serialize(dados, CarregadorCatalogo.OpcoesJson);
            File.WriteAllText(Caminho, json);
        }

        /// Carrega o save. Se o arquivo não existir ou estiver corrompido, devolve dados novos
        /// (o arquivo corrompido é renomeado para .bak, para não ser perdido).
        public DadosSalvos Carregar(out bool estavaCorrompido)
        {
            estavaCorrompido = false;
            if (!File.Exists(Caminho)) return new DadosSalvos();

            try
            {
                return JsonSerializer.Deserialize<DadosSalvos>(File.ReadAllText(Caminho), CarregadorCatalogo.OpcoesJson)
                       ?? new DadosSalvos();
            }
            catch (JsonException)
            {
                estavaCorrompido = true;
                File.Copy(Caminho, Caminho + ".bak", overwrite: true);
                return new DadosSalvos();
            }
        }

        public void Apagar()
        {
            if (File.Exists(Caminho)) File.Delete(Caminho);
        }
    }
}