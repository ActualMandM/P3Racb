using System.Text;

namespace P3Racb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine($"Usage: {System.Diagnostics.Process.GetCurrentProcess().ProcessName} <input>");
                return;
            }

            if (File.GetAttributes(args[0]).HasFlag(FileAttributes.Directory))
            {
                SearchDirectories(args[0]);
            }
            else
            {
                Console.WriteLine("Please provide a path to the CueSheet folder.");
                return;
            }
        }

        static void SearchDirectories(string path)
        {
            foreach (string folder in Directory.GetDirectories(path))
                SearchDirectories(folder);

            foreach (string file in Directory.GetFiles(path))
                TrimFile(file);
        }

        static void TrimFile(string filePath)
        {
            if (Path.GetExtension(filePath) != ".uasset")
                return;

            string FileName = Path.GetFileNameWithoutExtension(filePath);
            string AcbPath = Path.Combine(Path.GetDirectoryName(filePath), FileName + ".acb");

            if (File.Exists(AcbPath))
            {
                Console.WriteLine($"{FileName} has already been trimmed.");
                return;
            }

            byte[] AcbData = File.ReadAllBytes(filePath);

            int UtfIDX = PatternAt(AcbData, Encoding.ASCII.GetBytes("@UTF"));

            if (UtfIDX == -1)
            {
                Console.WriteLine($"{FileName} is not an acb.");
            }
            else
            {
                Console.WriteLine($"Trimmed {FileName}.");
                byte[] TrimData = new byte[AcbData.Length - UtfIDX];
                Array.Copy(AcbData, UtfIDX, TrimData, 0, AcbData.Length - UtfIDX);

                File.WriteAllBytes(AcbPath, TrimData);
            }
        }

        // love u stackoverflow https://stackoverflow.com/a/14712207
        static int PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                    return i;
            }

            return -1;
        }
    }
}
