using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using VignaTeste.MinimalAPI.Configurations;
using VignaTeste.MinimalAPI.Models;
namespace VignaTeste.MinimalAPI.Services
{

    public class GroqService
    {
        private readonly HttpClient _httpClient;
        private readonly GroqSettings _settings;

        public GroqService(HttpClient httpClient, IOptions<GroqSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        public async Task<ProcessoResponse?> ProcessarTextoAsync(string textoPdf)
        {
            var requestBody = new
            {
                model = _settings.ModelName,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $"Extraia rigorosamente como JSON válido, SEM COMENTÁRIOS ADICIONAIS, as seguintes informações:\n" +
                          "1. Número do processo (string, mantendo exatamente a formatação original, incluindo pontos, traços ou barras se existirem no texto).\n" +
                          "2. Partes envolvidas (array de strings no formato 'Nome (Papel)').\n\n" +
                          $"Texto do processo:\n{textoPdf}\n\n" +
                          "Exemplo de resposta válida:\n" +
                          "{\n" +
                          "  \"numero_processo\": \"número\",\n" +
                          "  \"partes\": [\"João Silva (Autor)\", \"Empresa XYZ (Réu)\"]\n" +
                          "}"

                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(_settings.ApiUrl, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Erro ao chamar a API da Groq: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<GroqResponse>();

            if (result?.Choices == null || result.Choices.Length == 0)
            {
                throw new Exception("Resposta inválida da API: Choices está vazio ou nulo.");
            }

            return JsonSerializer.Deserialize<ProcessoResponse>(result.Choices[0].Message.Content)
                ?? throw new JsonException("Erro ao desserializar a resposta da API.");
        }
    }

}
