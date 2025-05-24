namespace FileAnalysisService.Application.DTOs;

public class AnalyzeFileResponse
{
    public Guid FileId { get; set; }
    public int PlagiarismPercent { get; set; }
    public string WordCloudPath { get; set; }
}