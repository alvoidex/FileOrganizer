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
            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                string category = GetCategory(extension);
                categories.Add(category);
                if (!stats.ContainsKey(category)) stats[category] = 0;
                stats[category]++;
                Console.WriteLine($"{filePath} -> {category}");
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
                    continue;
                }
                File.Move(filePath, destinationPath);

                Console.WriteLine(
                    $"Перемещен: {Path.GetFileName(filePath)} -> {category}");
            }
            Console.WriteLine("Статистика:");
            foreach (var item in stats)
            {
                Console.WriteLine($"Найдено {item.Key}: {item.Value}");
            }
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
