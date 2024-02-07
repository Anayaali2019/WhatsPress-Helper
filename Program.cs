using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MudSlide_Downloader
{
    class Program
    {
      static  String comman = "\"";
        static async Task Main(string[] args)
        {
            string url = "https://github.com/robvanderleek/mudslide/releases/latest/download/mudslide.exe";
            string downloadPath = "mudslide.exe";
            string mudslidePath = "mudslide.exe";
            KillProcess(mudslidePath);
            File.Delete(mudslidePath);

            if (File.Exists(mudslidePath))
            {
              

            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        long? contentLength = response.Content.Headers.ContentLength;

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            await DownloadFileWithProgress(contentStream, contentLength, downloadPath);
                        }
                    }
                }

            }

               mudslide_logout("mudslide logout -c "+ comman + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data" + comman);

            Process process = new Process();

            // Set the file name (in this case, the command to run)
            process.StartInfo.FileName = "mudslide";

            // Set the arguments for the command
            process.StartInfo.Arguments = "login -c Data";

            // Optionally, you can set the working directory
           
            // Start the process
            process.Start();

            // Wait for the process to exit
            process.WaitForExit();


            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data"))
            {
                string[] files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data");

                if (files.Length > 0)
                {
                    File.Delete( Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data.zip");
                    ZipFile.CreateFromDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data.zip");

                    Environment.Exit(0);

                }
                else
                {
                    Console.WriteLine("The 'Data' folder is empty. Please Retry or contact Admin 00923356105927 on Whatsapp");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("The 'Data' folder is NOT EXISTS . Please Retry or contact Admin 00923356105927 on Whatsapp");
                Console.ReadLine();
            }

        }



        static void mudslide_logout(string processName) {
            string command = processName;

            // Create process start info
            ProcessStartInfo psi = new ProcessStartInfo("cmd", $"/c {command}");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            // Create process
            Process process = new Process();
            process.StartInfo = psi;

            // Event handler for asynchronous output
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.Start();
            process.WaitForExit();
        }
        static void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                Console.WriteLine($"{processName} is already running. Killing the process...");
                foreach (Process process in processes)
                {
                    process.Kill();
                }
            }
        }
        static async Task DownloadFileWithProgress(Stream contentStream, long? totalSize, string filePath)
        {
            const int bufferSize = 8192;
            byte[] buffer = new byte[bufferSize];
            long downloaded = 0;
            int read;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
            {
                while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, read);
                    downloaded += read;

                    if (totalSize.HasValue)
                    {
                        double percentage = ((double)downloaded / totalSize.Value) * 100;
                        Console.WriteLine($"Downloaded {downloaded}/{totalSize} bytes ({percentage:F2}%).");
                    }
                    else
                    {
                        Console.WriteLine($"Downloaded {downloaded} bytes.");
                    }
                }
            }
        }
    }
}
