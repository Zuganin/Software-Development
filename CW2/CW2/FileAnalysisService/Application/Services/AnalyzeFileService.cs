using System.Text;
using FileAnalysisService.Application.DTOs;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;
using FileStoringService.Application.Interfaces;

namespace FileAnalysisService.Application.Services;

public class AnalyzeFileService
{
    private readonly IFileAnalysisRepository _analysisRepo;
    private readonly IFileStoringService _fileStorage;
    private readonly IWordCloudGenerator _wordCloudGen;

    public AnalyzeFileService(
        IFileAnalysisRepository analysisRepo,
        IFileStoringService fileStorage,
        IWordCloudGenerator wordCloudGen)
    {
        _analysisRepo = analysisRepo;
        _fileStorage = fileStorage;
        _wordCloudGen = wordCloudGen;
    }

    public async Task<AnalyzeFileResponse> AnalyzeAsync(
        AnalyzeFileRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1) Проверяем, не проанализирован ли файл ранее
        var existing = await _analysisRepo.GetByFileIdAsync(request.FileId);
        if (existing is not null)
        {
            return new AnalyzeFileResponse
            {
                FileId = existing.FileId,
                PlagiarismPercent = existing.PlagiarismPercent,
                WordCloudPath = existing.Location
            };
        }

        // 2) Получаем содержимое и имя файла из FileStoringService
        (byte[] contentBytes, string fileName) fileData;
        try
        {
            fileData = await _fileStorage.GetFileAsync(request.FileId, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            throw new FileNotFoundException($"File with ID {request.FileId} was not found in storage.");
        }

        // 3) Декодируем текст (UTF-8)
        var text = Encoding.UTF8.GetString(fileData.contentBytes);

        // 4) Генерируем облако слов и сохраняем PNG
        var cloudFolder = Path.Combine("wwwroot", "wordclouds");
        Directory.CreateDirectory(cloudFolder);

        var cloudFileName = $"{request.FileId}.png";
        var cloudFilePath = Path.Combine(cloudFolder, cloudFileName);
        var savedCloudPath = await _wordCloudGen.GenerateWordCloudAsync(text, cloudFilePath);

        // 5) Считаем случайный процент антиплагиата (0–60%)
        var random = new Random();
        var percent = random.Next(0, 61);

        // 6) Сохраняем результат в базу
        var result = new FileAnalysisResult
        {
            FileId = request.FileId,
            FileName = fileData.fileName,
            Location = savedCloudPath,
            PlagiarismPercent = percent,
            AnalyzedAt = DateTime.UtcNow
        };
        await _analysisRepo.SaveAsync(result);

        // 7) Формируем ответ
        return new AnalyzeFileResponse
        {
            FileId = result.FileId,
            PlagiarismPercent = result.PlagiarismPercent,
            WordCloudPath = result.Location
        };
    }
}