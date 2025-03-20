using System;
using System.Collections.Generic;
using System.IO;
using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Export;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlExportVisitor : IExportVisitor
{
    private readonly string _outputDirectory;
    private readonly List<BankAccount> _bankAccounts = new();
    private readonly List<Category> _categories = new();
    private readonly List<Operation> _operations = new();

    public YamlExportVisitor(string outputDirectory)
    {
        _outputDirectory = outputDirectory;
        if (!Directory.Exists(_outputDirectory))
            Directory.CreateDirectory(_outputDirectory);
    }

    public void Visit(BankAccount account) 
    { 
        _bankAccounts.Add(account); 
    }
    
    public void Visit(Category category) 
    { 
        _categories.Add(category); 
    }
    
    public void Visit(Operation operation) 
    { 
        _operations.Add(operation); 
    }
    
    public void Close()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        File.WriteAllText(Path.Combine(_outputDirectory, "bankAccounts.yaml"), serializer.Serialize(_bankAccounts));
        File.WriteAllText(Path.Combine(_outputDirectory, "categories.yaml"), serializer.Serialize(_categories));
        File.WriteAllText(Path.Combine(_outputDirectory, "operations.yaml"), serializer.Serialize(_operations));
    }
}