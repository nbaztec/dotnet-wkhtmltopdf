using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf
{
    public class WkHtmlToPdfBinary : IWkHtmlToPdfBinary
    {
        public async Task<ExecutionResult> Execute(byte[] stdin, TimeSpan timeout)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "wkhtmltopdf",
                    Arguments = "- -",
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

                if (!process.Start())
                {
                    return new ExecutionResult { Success = false };
                }

                await process.StandardInput.BaseStream.WriteAsync(stdin, 0, stdin.Length);
                process.StandardInput.Close();

                using (var stdout = new MemoryStream())
                using (var stderr = new MemoryStream())
                {
                    await process.StandardOutput.BaseStream.CopyToAsync(stdout);
                    await process.StandardError.BaseStream.CopyToAsync(stderr);

                    var success = process.WaitForExit((int)timeout.TotalMilliseconds);

                    return new ExecutionResult
                    {
                        ExitCode = process.ExitCode,
                        Stderr = stderr.GetBuffer(),
                        Stdout = stdout.GetBuffer(),
                        Success = success,
                    };
                }
            }
        }
    }
}
