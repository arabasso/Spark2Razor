using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Spark2Razor.Rules;

namespace Spark2Razor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) return;

            var path = args[0];

            if (!Directory.Exists(path)) return;

            Console.WriteLine("Converting files on {0}", path);

            var converter = new Converter();

            converter.AddRulesFromNamespace("Spark2Razor.Rules");

            var files = Directory.GetFiles(path, "*.spark", SearchOption.AllDirectories);

            Parallel.ForEach(files, file =>
            {
                try
                {
                    var newFile = Path.Combine(path, "Conversion" + Path.ChangeExtension(file, ".cshtml").Replace(path, ""));

                    var directory = Path.GetDirectoryName(newFile);

                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    Console.WriteLine("\t{0} -> {1}", file.Replace(path, ""), newFile.Replace(path, ""));

                    var text = converter.Convert(File.ReadAllText(file));

                    using (var stream = new StreamWriter(newFile, false, new UTF8Encoding(true)))
                    {
                        stream.Write(text);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("\tException ({0}): {1}", file, e.Message);
                }
            });

            Console.WriteLine();
            Console.WriteLine("Press any key to continue. . .");
            Console.ReadKey();
        }
    }
}
