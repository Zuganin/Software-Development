namespace HSE_BANK.Import;

public abstract class DataImporter<T>
{
    public List<T> ImportData(string filePath)
    {
        string fileContent = ReadFile(filePath);
        
        List<T> data = ParseData(fileContent);
        
        SaveData(data);
        
        return data;
    }

    protected virtual string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }


    protected abstract List<T> ParseData(string fileContent);


    protected virtual void SaveData(List<T> data)
    {
        Console.WriteLine($"Импортировано {data.Count} элементов.");
    }
}