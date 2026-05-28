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
            string? srcPath = Console.ReadLine()?.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(srcPath) ||!Directory.Exists(srcPath))
            {
                Console.WriteLine("Папка не найдена");
                return;

            }
            var path = File.ReadLines(srcPath);
        }
        private void OrganizeFiles()
        {
        }

        private string GetCategory()
        {
        }
    }
}
