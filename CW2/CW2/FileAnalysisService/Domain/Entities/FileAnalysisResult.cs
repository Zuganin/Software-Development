namespace FileAnalysisService.Domain.Entities;

public class FileAnalysisResult
{
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public string Location { get; set; }
    public int PlagiarismPercent { get; set; }
    public DateTime AnalyzedAt { get; set; }
}