using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SolveCaptcha
{
    class Program
    {

        public static async Task<Stream> Image() {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cookie", "***");

            var stream = await client.GetStreamAsync("***");

            return stream;
        }
        public static async Task<string> Solution(string solution)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cookie", "***");

            var str = await client.GetStringAsync($"***?x={solution}");

            return str;
        }

        static void Main(string[] args)
        {
            var path = @"C:\Users\john\Desktop\test1";
            var textfile = $@"{path}\text.txt";
            if (File.Exists(textfile))
            {
                File.Delete(textfile);
                Thread.Sleep(1);
            }

            var client = new HttpClient();

            Image()
                .ContinueWith(x => {
                var stream = x.Result;
                    try
                    {
                        using (var image = new MagickImage(stream))
                        {
                            image.Quantize(new QuantizeSettings() { Colors = 2, DitherMethod = DitherMethod.No });
                            image.Write($@"{path}\source.png");
                        }
                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Dispose();
                        }

                    }
            }).Wait();

            var command= $@"tesseract ""{path}\source.png"" ""{path}\text"" -l eng";
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo($@"cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();
            var flag = true;

           
                        var text = File.ReadAllText(textfile).Trim();
                        Solution(text).ContinueWith(str =>
                        {
                            File.WriteAllText($@"{path}\index.html", str.Result);
                            flag = false;

                        }).Wait();


            Console.WriteLine("Hello World!");
        }
    }
}
