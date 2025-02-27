# VignaTeste.MinimalAPI

## Descrição
Esta é uma API Minimal em .NET que permite o upload de arquivos PDF, extrai o texto do documento e envia para um modelo de IA (Mistral) para processamento. A resposta da IA retorna em formato JSON com as informações extraídas do documento.

---

## Tecnologias utilizadas
- .NET 8 (Minimal API)
- HttpClient para comunicação com a API de IA
- UglyToad.PdfPig para extração de texto de PDFs
- Swagger para documentação e testes da API

---

## Como rodar o projeto


### 1. Configurar variáveis de ambiente
Edite o arquivo `appsettings.json` e adicione suas credenciais da API Mistral:
```json
"MistralSettings": {
  "ApiUrl": "URL_DA_API",
  "ApiKey": "SUA_CHAVE_AQUI"
}
```

### 2. Rodar a API
Abra um terminal na pasta do projeto e execute:
```bash
  dotnet run
```
Ou, se estiver usando o Visual Studio, apenas inicie o projeto (`F5`).


---

## Endpoints
### **Upload e Processamento do PDF**
- **URL:** `POST /Processos/Upload/AI`
- **Descrição:** Faz o upload de um arquivo PDF, extrai o texto e envia para a IA para processamento.
- **Parâmetros:**
  - `processoArquivo` (form-data): arquivo PDF.
- **Resposta esperada:**
```json
{
  "numero_processo": "1001385-64.2023",
  "partes": ["João Silva (Autor)", "Empresa XYZ (Réu)"]
}
```

# Chamada à API da Mistral

## Configuração do HttpClient
O `MistralService` injeta `HttpClient` e configura o cabeçalho de autorização:
```csharp
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
```

## Requisição
Envia um `POST` para a URL configurada em `_settings.ApiUrl`, com um corpo JSON contendo o modelo e a mensagem:
```json
{
  "model": "mistral-small",
  "messages": [
    {
      "role": "user",
      "content": "Extraia rigorosamente como JSON válido..."
    }
  ]
}
```


## Tratamento da Resposta
A resposta da API é processada para verificar se é válida. Caso seja, os dados são extraídos e convertidos para um formato esperado. Se houver erro ou a resposta for inválida, o serviço retorna `null` para indicar falha no processamento.



