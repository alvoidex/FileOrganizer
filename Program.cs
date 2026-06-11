namespace FileOrganizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Organizer = new FileOrganizer();
            Organizer.Start();
        }
    }
    class FileOrganizer
    {
        public void Start()
        {
            Console.Write("Введите путь:\n> ");
            string? srcPath = Console.ReadLine()?.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(srcPath) ||!Directory.Exists(srcPath))
            {
                Console.WriteLine("Папка не найдена");
                return;

            }
            OrganizeFiles(srcPath);
        }
        private void OrganizeFiles(string srcPath)
        {
            var categories = new HashSet<string>();
            var files = Directory.GetFiles(srcPath);
            var stats = new Dictionary<string, int>();
            var categorySizes = new Dictionary<string, long>();
            int movedFiles = 0;
            int skippedFiles = 0;
            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                string category = GetCategory(extension);
                long fileSize = new FileInfo(filePath).Length;

                if (!categorySizes.ContainsKey(category))
                    categorySizes[category] = 0;

                categorySizes[category] += fileSize;

                categories.Add(category);
                if (!stats.ContainsKey(category)) stats[category] = 0;
                stats[category]++;
                Console.WriteLine($"{filePath} -> {category}");
            }

            Console.Write("Переместить файлы? (y/n): ");
            var answer = Console.ReadLine();
            if (answer?.ToLower() != "y")
            {
                Console.WriteLine("Режим предпросмотра завершен");
                return;
            }

            foreach (var category in categories)
            {
                Directory.CreateDirectory(Path.Combine(srcPath, category));
                Console.WriteLine($"Создана папка: {category}");
            }
            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                string category = GetCategory(extension);

                string destinationPath = Path.Combine(
                    srcPath,
                    category,
                    Path.GetFileName(filePath));

                if (File.Exists(destinationPath))
                {
                    Console.WriteLine(
                        $"Пропущен: {Path.GetFileName(filePath)} уже существует");
                    skippedFiles++;
                    continue;
                }

                    File.Move(filePath, destinationPath);
                    movedFiles++;
                    Console.WriteLine(
                    $"Перемещен: {Path.GetFileName(filePath)} -> {category}");

            }
            Console.WriteLine("Статистика:");
            foreach (var item in stats)
            {
                Console.WriteLine(
                        $"{item.Key}: {item.Value} файлов ({categorySizes[item.Key] / 1024 / 1024} MB)");
            }
            Console.WriteLine($"\nПеремещено: {movedFiles}");
            Console.WriteLine($"Пропущено: {skippedFiles}");
        }
        private string GetCategory(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return Categories.Other;

            return FileCategoryConfig.Extensions
                .TryGetValue(extension.ToLower(), out var category)
                    ? category
                    : Categories.Other;
        }
    }
    class FileCategoryConfig
    {
        public static readonly Dictionary<string, string> Extensions = new()
    {
        { ".png", Categories.Images },{ ".jpg", Categories.Images },{ ".jpeg", Categories.Images },
        { ".gif", Categories.Images },{ ".raw", Categories.Images },{ ".bmp", Categories.Images },
        { ".mp4", Categories.Videos },{ ".mov", Categories.Videos },{ ".mkv", Categories.Videos },
        { ".mpeg", Categories.Videos },{ ".avi", Categories.Videos },{ ".wmv", Categories.Videos },
        { ".flv", Categories.Videos },{ ".doc", Categories.Documents },{ ".docx", Categories.Documents },
        { ".txt", Categories.Documents },{ ".fb2", Categories.Documents },{ ".csv", Categories.Documents },
        { ".json", Categories.Documents },{ ".pdf", Categories.Documents },{ ".xlsx", Categories.Documents },
        { ".xls", Categories.Documents },{ ".rtf", Categories.Documents },{ ".epub", Categories.Documents },
        { ".ppt", Categories.Documents },{ ".mp3", Categories.Audio },{ ".flac", Categories.Audio },
        { ".wav", Categories.Audio },{ ".aiff", Categories.Audio },{ ".aac", Categories.Audio },
        { ".ogg", Categories.Audio }
    };
    }
    class Categories
    {
        public const string Images = "Images";
        public const string Videos = "Videos";
        public const string Documents = "Documents";
        public const string Audio = "Audio";
        public const string Other = "Other";
    }
}
