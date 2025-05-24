using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FileAnalysisService.Domain.Interfaces;

namespace FileAnalysisService.Infrastructure.WordCloud;

public class QuickChartWordCloudGenerator : IWordCloudGenerator
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public QuickChartWordCloudGenerator(HttpClient http)
    {
        _http = http;
        // базовый URL QuickChart; можно вынести в конфиг
        _baseUrl = "https://quickchart.io/wordcloud";
    }

    public async Task<string> GenerateWordCloudAsync(string text, string savePath)
    {
        // разбиваем текст на слова и считаем частоты
        var words = text
            .Split(new[] { ' ', '\r', '\n', '\t', ',', '.', ';', ':', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(w => w.ToLowerInvariant())
            .Select(g => new { text = g.Key, weight = g.Count() })
            .ToArray();

        // формируем payload QuickChart
        var payload = new
        {
            format = "png",
            width = 500,
            height = 500,
            fontFamily = "Arial",
            // слова передаются в args.words
            words = words
        };

        var json = JsonSerializer.Serialize(new { chart = new { type = "wordCloud", data = new { words }, options = payload } });

        // HTTP POST
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(_baseUrl, content);
        response.EnsureSuccessStatusCode();

        // сохраняем PNG
        await using var stream = await response.Content.ReadAsStreamAsync();
        var dir = Path.GetDirectoryName(savePath)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        await using var fileStream = File.Create(savePath);
        await stream.CopyToAsync(fileStream);

        return savePath;
    }
}