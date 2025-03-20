using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Export;

namespace HSE_BANK.Export;

public class CsvExportVisitor : IExportVisitor
{
    private readonly string _outputDirectory;
    private readonly List<BankAccount> _bankAccounts = new();
    private readonly List<Category> _categories = new();
    private readonly List<Operation> _operations = new();

    public CsvExportVisitor(string outputDirectory)
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
        // Экспорт BankAccounts
        using (var writer = new StreamWriter(Path.Combine(_outputDirectory, "bankAccounts.csv")))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(_bankAccounts);
        }
        // Экспорт Categories
        using (var writer = new StreamWriter(Path.Combine(_outputDirectory, "categories.csv")))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(_categories);
        }
        // Экспорт Operations
        using (var writer = new StreamWriter(Path.Combine(_outputDirectory, "operations.csv")))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(_operations);
        }
    }

}