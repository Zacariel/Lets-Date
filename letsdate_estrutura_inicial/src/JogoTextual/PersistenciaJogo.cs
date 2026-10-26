using System.IO;
using System.Text.Json;
using JogoTextual.Dominio;

namespace JogoTextual
{
    public class PersistenciaJogo
    {
        private readonly string _caminhoArquivo = "save_letsdate.json";

        public void Salvar(EstadoJogo estado)
        {
            var opcoes = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(estado, opcoes);
            File.WriteAllText(_caminhoArquivo, json);
        }

        public EstadoJogo Carregar()
        {
            if (!File.Exists(_caminhoArquivo))
            {
                return new EstadoJogo();
            }

            string json = File.ReadAllText(_caminhoArquivo);
            return JsonSerializer.Deserialize<EstadoJogo>(json) ?? new EstadoJogo();
        }
    }
}