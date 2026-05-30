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
            var files = Directory.GetFiles(srcPath);
            var stats = new Dictionary<string, int>();
            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                string category = GetCategory(extension);
                if (!stats.ContainsKey(category)) stats[category] = 0;
                stats[category]++;
                Console.WriteLine($"{filePath} -> {category}");
            }
            Console.WriteLine("Статистика:");
            foreach (var item in stats)
            {
                Console.WriteLine($"Найдено {item.Key}: {item.Value}");
            }
        }
        private string GetCategory(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return "Other";
            return extension.ToLower() switch
            {
                ".png" or ".jpg" or ".jpeg" or ".gif" or ".raw" or ".bmp" => "Images",
                ".mp4" or ".mov" or ".mkv" or ".mpeg" or ".avi" or ".wmv" or ".flv" => "Videos",
                ".doc" or ".docx" or ".txt" or ".fb2" or ".csv" or ".json" or ".pdf" or ".xlsx" or ".xls" or ".rtf" or ".epub" or ".ppt" => "Documents",
                ".mp3" or ".flac" or ".wav" or ".aiff" or ".aac" or ".ogg" => "Audio",
                _ => "Other"
            };
        }
    }
}
