using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        public static int CountForThirdMethod { get; set; } = 0;
        static object locker = new object();

        private static int ReadFile(string path)
        {
            int count = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string line = String.Empty;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        i++;
                        MatchCollection matches3 = Regex.Matches(line, "а");
                        count += matches3.Count;
                        Console.WriteLine($"in line num {i} count symbol \"a\" equal: {matches3.Count}");
                    }
                }
            }
            return count;
        }

        public static async Task<int> ReadFileAsync(string path)
        {
            int count = 0;
            using (StreamReader sr = File.OpenText(path))
            {
                string line = String.Empty;
                int i = 0;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        i++;
                        MatchCollection matches3 = Regex.Matches(line, "а");
                        count += matches3.Count;
                        Console.WriteLine($"in line num {i} count symbol a equal: {matches3.Count}");
                    }
                }
            }
            return count;
        }

        public static int CountOfThreads { get; set; } = 0;

        public static void ReadFileThread(string path)
        {
            StreamReader sr = File.OpenText(path);
            string line = String.Empty;
            var threads = new List<Thread>();
            while ((line = sr.ReadLine()) != null)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(WorkWithLines));
                    thread.Start(line);
                    threads.Add(thread);
                }
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        public static int Counter { get; set; } = 0;


        private static void WorkWithLines(object line)
        {
            Counter++;
            MatchCollection matches3 = Regex.Matches(line.ToString(), "а");
            Console.WriteLine($"in line num {Counter} count symbol a equal: {matches3.Count}");

            lock (locker)
            {
                CountForThirdMethod += matches3.Count;
            }

        }

        static void Extract(string whereFile, string nameOfFolder, string namOfFile)
        {
            string nameOfFolder1 = "Resources";
            string namOfFile1 = "text.txt";

            Assembly assembly = Assembly.GetCallingAssembly();
            using (Stream s = assembly.GetManifestResourceStream("ConsoleApp1" + "." + (nameOfFolder1 == "" ? "" : nameOfFolder1 + ".") + namOfFile1))
            {
                using (BinaryReader r = new BinaryReader(s))
                {
                    using (FileStream fs = new FileStream(whereFile + "\\" + namOfFile, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter w = new BinaryWriter(fs))
                        {
                            w.Write(r.ReadBytes((int)s.Length));
                        }
                    }
                }

            }
        }

        static void PutFileInOutput()
        {
            Extract(Directory.GetCurrentDirectory(), "Resources", "text.txt");
        }

        static async Task Main(string[] args)
        {
            PutFileInOutput();
            string path = Directory.GetCurrentDirectory() + "\\text.txt";
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();
            Stopwatch stopwatch3 = new Stopwatch();

            stopwatch.Start();
            ReadFile(path);
            int count1 = ReadFile(path);
            stopwatch.Stop();

            stopwatch2.Start();
            int count2 = await ReadFileAsync(path);
            stopwatch2.Stop();


            stopwatch3.Start();

            ReadFileThread(path);
            int count3 = CountForThirdMethod;
            stopwatch3.Stop();


            Console.WriteLine($"Wasted the time consistently method {stopwatch.Elapsed + Environment.NewLine}");

            Console.WriteLine($"Wasted the time async method {stopwatch2.Elapsed + Environment.NewLine}");

            Console.WriteLine($"Wasted the time multy Thread method {stopwatch3.Elapsed + Environment.NewLine}");

            Console.WriteLine($"Total count of \"a\" in consistently method: {count1 + Environment.NewLine}");

            Console.WriteLine($"Total count of \"a\" in async method: {count2 + Environment.NewLine}");

            Console.WriteLine($"Total count of \"a\" in multy Thread method: {count3 + Environment.NewLine}");

            Console.ReadKey();
        }
    }
}