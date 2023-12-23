using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class Figure
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Figure() { }

    public Figure(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileManager
{
    public static string[] LoadFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();
            switch (fileExtension)
            {
                case ".txt":
                    return File.ReadAllLines(filePath);
                case ".json":
                    string jsonData = File.ReadAllText(filePath);
                    Figure figure = JsonConvert.DeserializeObject<Figure>(jsonData);
                    return new[] { $"Name: {figure.Name}", $"Width: {figure.Width}", $"Height: {figure.Height}" };
                case ".xml":
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                    XmlNode root = xmlDoc.DocumentElement;
                    string[] lines = new string[root.ChildNodes.Count];
                    for (int i = 0; i < root.ChildNodes.Count; i++)
                    {
                        XmlNode childNode = root.ChildNodes[i];
                        lines[i] = $"{childNode.Name}: {childNode.InnerText}";
                    }
                    return lines;
                default:
                    Console.WriteLine("Неподдерживаемый формат файла.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Файл не найден.");
        }

        return new string[] { };
    }

    public static void SaveFile(string filePath, string[] content)
    {
        string fileExtension = Path.GetExtension(filePath).ToLower();
        switch (fileExtension)
        {
            case ".txt":
                File.WriteAllLines(filePath, content);
                break;
            case ".json":
                Figure figure = new Figure
                {
                    Name = content[0].Split(':')[1].Trim(),
                    Width = int.Parse(content[1].Split(':')[1].Trim()),
                    Height = int.Parse(content[2].Split(':')[1].Trim())
                };
                string jsonData = JsonConvert.SerializeObject(figure, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                break;
            case ".xml":
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Figure));
                using (TextWriter writer = new StreamWriter(filePath))
                {
                    xmlSerializer.Serialize(writer, new Figure
                    {
                        Name = content[0].Split(':')[1].Trim(),
                        Width = int.Parse(content[1].Split(':')[1].Trim()),
                        Height = int.Parse(content[2].Split(':')[1].Trim())
                    });
                }
                break;
            default:
                Console.WriteLine("Неподдерживаемый формат файла.");
                break;
        }
    }
}

public class TextEditor
{
    private Figure currentFigure;
    private string filePath;

    public void Run()
    {
        Console.WriteLine("Введите путь к файлу: ");
        filePath = Console.ReadLine();

        while (true)
        {
            Console.Clear();
            DisplayFigureInfo();

            Console.WriteLine("Опции:");
            Console.WriteLine("1. Редактировать рисунок");
            Console.WriteLine("2. Сохранить (F1)");
            Console.WriteLine("3. Выход (Escape)");

            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    EditFigure();
                    break;
                case ConsoleKey.F1:
                    FileManager.SaveFile(filePath, new[] { $"Name: {currentFigure.Name}", $"Width: {currentFigure.Width}", $"Height: {currentFigure.Height}" });
                    Console.WriteLine("Файл успешно сохранен.");
                    Console.ReadKey();
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private void DisplayFigureInfo()
    {
        string[] content = FileManager.LoadFile(filePath);
        if (content.Length >= 3) // Удостоверьтесь, что есть достаточно строк для представления данных Figure
        {
            currentFigure = new Figure
            {
                Name = content[0].Split(':')[1].Trim(),
                Width = int.Parse(content[1].Split(':')[1].Trim()),
                Height = int.Parse(content[2].Split(':')[1].Trim())
            };

            Console.WriteLine($"Name: {currentFigure.Name}");
            Console.WriteLine($"Width: {currentFigure.Width}");
            Console.WriteLine($"Height: {currentFigure.Height}");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Ошибка: Данные рисунка неполны или неверны.");
            Console.ReadKey();
        }
    }

    private void EditFigure()
    {
        DisplayFigureInfo();
        if (currentFigure == null)
        {
            Console.WriteLine("Ошибка: рисунок не загружен");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine("Редактировать рисунок:");
        Console.WriteLine("1. Имя");
        Console.WriteLine("2. Ширина");
        Console.WriteLine("3. Height");
        Console.WriteLine("4. Back");

        ConsoleKeyInfo editKey = Console.ReadKey();
        switch (editKey.Key)
        {
            case ConsoleKey.D1:
                Console.Write("Введите новое имя: ");
                currentFigure.Name = Console.ReadLine();
                break;
            case ConsoleKey.D2:
                Console.Write("Введите новую ширину: ");
                int newWidth;
                if (int.TryParse(Console.ReadLine(), out newWidth))
                {
                    currentFigure.Width = newWidth;
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Ширина должна быть целым числом.");
                    Console.ReadKey();
                }
                break;
            case ConsoleKey.D3:
                Console.Write("Введите новую высоту: ");
                int newHeight;
                if (int.TryParse(Console.ReadLine(), out newHeight))
                {
                    currentFigure.Height = newHeight;
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Высота должна быть целым числом.");
                    Console.ReadKey();
                }
                break;
            case ConsoleKey.D4:
                break;
        }
    }

    public static void Main()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.Run();
    }
}