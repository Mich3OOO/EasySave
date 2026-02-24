using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using EasyLog;

// Entry point for EasyLogAPI
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "API EasyLog OK!");

///<summary>
/// POST endpoint to receive logs from EasySave
///</summary>
app.MapPost("/api/logs", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var rawContent = await reader.ReadToEndAsync();

    var contentType = context.Request.ContentType?.ToLower() ?? "";
    var extension = contentType.Contains("application/json") ? "json"
        : contentType.Contains("application/xml") ? "xml"
        : "txt";

    Logger.GetInstance().Log(rawContent, extension);
    return Results.Ok();
});

app.Run();
