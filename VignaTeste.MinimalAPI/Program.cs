using System.Linq.Expressions;
using VignaTeste.MinimalAPI.Configurations;
using VignaTeste.MinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ExtratorTextoPDFService>();
builder.Services.AddHttpClient<GroqService>();

var configuration = builder.Configuration;
builder.Services.Configure<MistralSettings>(configuration.GetSection("MistralSettings"));

var app = builder.Build();

app.MapPost("/Processos/Upload/AI", async(IFormFile processoArquivo, ExtratorTextoPDFService pdfService, GroqService mistralService) =>
{
    try
    {

        if (processoArquivo == null || processoArquivo.Length == 0)
        {
            return Results.BadRequest("Nenhum arquvio foi enviado");
        }

        if (Path.GetExtension(processoArquivo.FileName).ToLower() != ".pdf")
        {
            return Results.BadRequest("O arquivo enviado não é um pdf");
        }
        string textoPdf;

        using (var memoryStream = new MemoryStream())
        {
            await processoArquivo.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            textoPdf = pdfService.ExtrairTextoDoPdf(memoryStream);
        }

        var processo = await mistralService.ProcessarTextoAsync(textoPdf);

        return Results.Ok(processo);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
    .DisableAntiforgery(); 



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
