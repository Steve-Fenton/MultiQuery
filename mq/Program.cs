using Fenton.MultiQuery;
using mq;
using System.Text.Json;


string driveletter = Environment.GetCommandLineArgs()[1];

string path = $@"{driveletter}:\Temp\mq";
string inputPath = Path.Combine(path, "input.json");
string outputPath = Path.Combine(path, "output.csv");

Console.WriteLine($"Reading query from {inputPath}");
Input input = JsonSerializer.Deserialize<Input>(File.ReadAllText(inputPath));

if (input == null)
{
    Console.WriteLine("Could not read input file");
}
else
{
    Console.WriteLine($"Creating runner with supplied input");
    QueryRunner runner = new QueryRunner(input.ConnectionStrings, input.Query, input.Fields);

    Console.WriteLine($"Writing result to {outputPath}");
    runner.ToCsv(outputPath);
}


