using PS5_Tools;

namespace Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new UcpTocParser();
            var entries = parser.Parse(@"C:\Users\eonvw\OneDrive\Documents\SCE\Trophy FIles\trophy01.ucp");

            foreach (var entry in entries)
            {
                Console.WriteLine($"[{entry.FileName}] Offset: {entry.Offset}, Size: {entry.Size}, Signature: {entry.Signature}, PNG: {entry.IsPng}, JSON: {entry.IsJson}");
            }

            // Optional: Extract all JSON files
            foreach (var entry in entries)
            {
                if (entry.IsJson || entry.IsPng)
                {
                    parser.ExtractFile(@"C:\Users\eonvw\OneDrive\Documents\SCE\Trophy FIles\trophy01.ucp", "output", entry);
                }
            }
        }
    }
}
