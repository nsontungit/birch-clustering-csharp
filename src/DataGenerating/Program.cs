// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");
GenerateData(30_000, @"D:\small-projects\birch-clustering-csharp\data\gt1MB\set21.csv");
//GenerateData(30, @"C:\Users\nsont\Downloads\portables\datn\src\data\set15.csv");
//GenerateData(30, @"C:\Users\nsont\Downloads\portables\datn\src\data\set16.csv");
//GenerateData(30, @"C:\Users\nsont\Downloads\portables\datn\src\data\set17.csv");

void GenerateData(int count, string path)
{
    Random rd = new Random();
    using var stream = File.CreateText(path);
    for (int i = 0; i < count; i++)
    {
        var value1 = rd.NextDouble();
        var value2 = rd.NextDouble();

        var line = string.Format("{0},{1}", value1, value2);
        stream.WriteLine(line);
    }
    stream.Close();
}