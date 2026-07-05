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
            if (string.IsNullOrWhiteSpace(srcPath) || !Directory.Exists(srcPath))
            {
                Console.WriteLine("Папка не найдена");
                return;

            }
            OrganizeFiles(srcPath);
        }
        private void OrganizeFiles(string srcPath)
        {
            var categories = new HashSet<string>();
            string[] files;
            try
            {
                files = Directory.GetFiles(
                    srcPath,
                    "*",
                    SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка доступа к папке: {ex.Message}");
                return;
            }
            var ignoredFolders = new HashSet<string>
            {
                Categories.Images,
                Categories.Videos,
                Categories.Documents,
                Categories.Audio,
                Categories.Programs,
                Categories.Archives,
                Categories.Other
            };
            files = files.Where(file =>
            {
                string? parentFolder =
                    Path.GetFileName(Path.GetDirectoryName(file));
                return parentFolder != null && !ignoredFolders.Contains(parentFolder);
            }).ToArray();
            if (files.Length == 0)
            {
                Console.WriteLine("Файлы не найдены");
                return;
            }
            var stats = new Dictionary<string, int>();
            var categorySizes = new Dictionary<string, long>();
            var fileMover = new FileMover();
            int movedFiles = 0;
            int skippedFiles = 0;
            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
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
            if (answer?.Trim().ToLower() != "y")
            {
                Console.WriteLine("Режим предпросмотра завершен");
                return;
            }

            foreach (var category in categories)
            {
                Directory.CreateDirectory(Path.Combine(srcPath, category));
                Console.WriteLine($"Создана папка: {category}");
            }
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];

                Console.WriteLine($"[{i + 1}/{files.Length}] {Path.GetFileName(filePath)}");

                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                string category = GetCategory(extension);

                string destinationPath = Path.Combine(
                    srcPath,
                    category,
                    Path.GetFileName(filePath));

                if (fileMover.Move(filePath, destinationPath))
                {
                    movedFiles++;
                    Console.WriteLine($"Перемещен: {Path.GetFileName(filePath)} -> {category}");
                }
                else
                {
                    skippedFiles++;
                    Console.WriteLine($"Пропущен: {Path.GetFileName(filePath)} уже существует");
                }
            }
            Console.WriteLine("Статистика:");
            foreach (var item in stats.OrderBy(x => x.Key))
            {
                Console.WriteLine(
                    $"{item.Key}: {item.Value} файлов ({(categorySizes[item.Key] / 1024.0 / 1024.0):F2} MB)");
            }
            Console.WriteLine($"\nПеремещено: {movedFiles}");
            Console.WriteLine($"Пропущено: {skippedFiles}");
            long totalSize = categorySizes.Values.Sum();

            Console.WriteLine($"\nВсего файлов: {files.Length}");
            Console.WriteLine($"Общий размер: {(totalSize / 1024.0 / 1024.0):F2} MB");
        }
        private string GetCategory(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return Categories.Other;

            return FileCategoryConfig.Extensions
                .TryGetValue(extension.ToLowerInvariant(), out var category)
                    ? category
                    : Categories.Other;
        }
    }
    class FileMover
    {
        public bool Move(string sourcePath, string destinationPath)
        {
            try
            {
                if (File.Exists(destinationPath))
                    return false;

                File.Move(sourcePath, destinationPath);
                return true;
            }
            catch {
                return false;
            }
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
        { ".ogg", Categories.Audio },{ ".exe", Categories.Programs },{ ".apk", Categories.Programs },
        { ".msi", Categories.Programs },{ ".cab", Categories.Programs },{ ".app", Categories.Programs },
        { ".rar", Categories.Archives },{ ".zip", Categories.Archives },{ ".7z", Categories.Archives },
        { ".iso", Categories.Archives },{ ".tar", Categories.Archives },{ ".gz", Categories.Archives }
    };
    }
    class Categories
    {
        public const string Images = "Images";
        public const string Videos = "Videos";
        public const string Documents = "Documents";
        public const string Audio = "Audio";
        public const string Other = "Other";
        public const string Programs = "Programs";
        public const string Archives = "Archives";
    }
}