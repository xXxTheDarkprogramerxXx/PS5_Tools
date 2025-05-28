using PS5_Tools.Structures;
using System.Text;

namespace PS5_Tools
{
    public class UcpTocParser
    {
        private const int TocEntrySize = 0x40;//Lenght of an entry

        public List<UcpTocEntry> Parse(string filePath)
        {
            var entries = new List<UcpTocEntry>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                // Read header info
                fs.Seek(0x10, SeekOrigin.Begin);
                int tocCount = ReadBigEndianInt32(br);
                int tocOffset = ReadBigEndianInt32(br);

                for (int i = 0; i < tocCount; i++)
                {
                    long entryStart = tocOffset + i * TocEntrySize;
                    fs.Seek(entryStart, SeekOrigin.Begin);
                    byte[] entry = br.ReadBytes(TocEntrySize);

                    string fileName = Encoding.ASCII.GetString(entry, 0x20, 0x20).TrimEnd('\0');
                    long fileOffset = ReadBigEndianInt64(entry, 0x40);
                    long fileSize = ReadBigEndianInt64(entry, 0x48);

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        string signature = GetFileSignature(fs, fileOffset, fileSize);
                        entries.Add(new UcpTocEntry
                        {
                            FileName = fileName,
                            Offset = fileOffset,
                            Size = fileSize,
                            Signature = signature
                        });
                    }
                }
            }

            return entries;
        }

        public void ExtractFile(string ucpFilePath, string outputDirectory, UcpTocEntry entry)
        {
            Directory.CreateDirectory(outputDirectory);

            using (var fs = new FileStream(ucpFilePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(entry.Offset, SeekOrigin.Begin);
                byte[] content = new byte[entry.Size];
                fs.Read(content, 0, (int)entry.Size);

                string safeName = entry.FileName.Replace("/", "_");
                File.WriteAllBytes(Path.Combine(outputDirectory, safeName), content);
            }
        }

        private string GetFileSignature(FileStream fs, long offset, long size)
        {
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = new byte[Math.Min(8, size)];
            fs.Read(buffer, 0, buffer.Length);
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        private int ReadBigEndianInt32(BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private long ReadBigEndianInt64(byte[] buffer, int startIndex)
        {
            byte[] slice = new byte[8];
            Array.Copy(buffer, startIndex, slice, 0, 8);
            Array.Reverse(slice);
            return BitConverter.ToInt64(slice, 0);
        }
    }
}