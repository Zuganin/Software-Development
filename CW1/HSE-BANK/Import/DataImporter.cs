namespace HSE_BANK.Import;

public abstract class DataImporter<T>
{
    public List<T>? ImportData(string filePath)
    {
        string fileContent = ReadFile(filePath);
        
        if(fileContent == "")
            return null;
        List<T> data = ParseData(fileContent);
        
        return data;
    }

    public virtual string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }


    public abstract List<T> ParseData(string fileContent);
    
}