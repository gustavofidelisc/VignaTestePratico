using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using VignaTeste.MinimalAPI.Configurations;
using VignaTeste.MinimalAPI.Models;
namespace VignaTeste.MinimalAPI.Services
{

    public class MistralService
    {
        private readonly HttpClient _httpClient;
        private readonly MistralSettings _settings;

        public MistralService(HttpClient httpClient, IOptions<MistralSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        public async Task<ProcessoResponse?> ProcessarTextoAsync(string textoPdf)
        {
            var requestBody = new
            {
                model = "mistral-small",
                messages = new[]
                {
                new
                {
                    role = "user",
                    content = $"Extraia rigorosamente como JSON válido, SEM COMENTÁRIOS ADICIONAIS, as seguintes informações:\n" +
                              "1. Número do processo (string)\n" +
                              "2. Partes envolvidas (array de strings no formato 'Nome (Papel)')\n\n" +
                              $"Texto do processo:\n{textoPdf}\n\n" +
                              "Exemplo de resposta válida:\n" +
                              "{\n" +
                              "  \"numero_processo\": \"1001385-64.2023\",\n" +
                              "  \"partes\": [\"João Silva (Autor)\", \"Empresa XYZ (Réu)\"]\n" +
                              "}"
                }
            }
            };

            var response = await _httpClient.PostAsJsonAsync(_settings.ApiUrl, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<MistralResponse>();

            if (result?.Choices == null || result.Choices.Length == 0)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<ProcessoResponse>(result.Choices[0].Message.Content);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

}
