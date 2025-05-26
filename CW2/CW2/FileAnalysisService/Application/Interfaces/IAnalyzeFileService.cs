using FileAnalysisService.Application.DTOs;

namespace FileAnalysisService.Application.Interfaces;

public interface IAnalyzeFileService
{
    Task<AnalyzeFileResponse> AnalyzeAsync(AnalyzeFileRequest request, CancellationToken cancellationToken = default);
}