using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

// Entry point for EasyLogAPI
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// POST endpoint to receive logs from EasySave
app.MapPost("/api/logs", async (HttpContext context) =>
{
    // Read the raw content from the request body
    using var reader = new StreamReader(context.Request.Body);
    var rawContent = await reader.ReadToEndAsync();
    var logsPath = "./logs";
    Directory.CreateDirectory(logsPath);

    // Log the received Content-Type for debugging
    Console.WriteLine($"[EasyLogAPI] Content-Type received: {context.Request.ContentType}");

    // Determine file extension based on Content-Type
    var contentType = context.Request.ContentType?.ToLower() ?? "";
    var extension = contentType.Contains("application/json") ? "json"
        : contentType.Contains("application/xml") ? "xml"
        : "txt";
    // Use date for log file name, append new logs to the same file
    var filePath = Path.Combine(logsPath, DateTime.Now.ToString("yyyy-MM-dd") + "_log." + extension);

    // Append the log content to the file
    await File.AppendAllTextAsync(filePath, rawContent + Environment.NewLine);
    Console.WriteLine($"[EasyLogAPI] Log file saved: {filePath}");
    return Results.Ok();
});

// Start the web application
app.Run();
