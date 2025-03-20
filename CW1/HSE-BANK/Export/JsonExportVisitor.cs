using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Export;

namespace HSE_BANK.Export;

public class JsonExportVisitor : IExportVisitor
{
    private readonly string _outputDirectory;
    private readonly List<BankAccount> _bankAccounts = new();
    private readonly List<Category> _categories = new();
    private readonly List<Operation> _operations = new();

    public JsonExportVisitor(string outputDirectory)
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
        if (_bankAccounts != null || _bankAccounts.Any())
        {
            File.WriteAllText(Path.Combine(_outputDirectory, "bankAccounts.json"),
                JsonConvert.SerializeObject(_bankAccounts, Formatting.Indented));
        }

        if (_categories != null || _categories.Any())
        {
            File.WriteAllText(Path.Combine(_outputDirectory, "categories.json"),
                JsonConvert.SerializeObject(_categories, Formatting.Indented));
        }

        if (_operations != null || _operations.Any())
        {
            File.WriteAllText(Path.Combine(_outputDirectory, "operations.json"),
                JsonConvert.SerializeObject(_operations, Formatting.Indented));
        }
    }
}