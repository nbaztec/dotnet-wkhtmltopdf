using System.Threading.Tasks;

namespace WkHtmlToPdf
{
    using System;
    using System.Text;

    public class PdfGenerator
    {
        private readonly IWkHtmlToPdfBinary _pdfBinary;

        public static PdfGenerator MakeDefault()
        {
            return new PdfGenerator(new WkHtmlToPdfBinary());            
        }
        
        public PdfGenerator(IWkHtmlToPdfBinary pdfBinary)
        {
            _pdfBinary = pdfBinary;
        }
        
        public Task<byte[]> Generate(string data)
        {
            return Generate(data, TimeSpan.FromSeconds(30));
        }
        
        public async Task<byte[]> Generate(string data, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new PdfConvertException("input string empty");
            }
            
            var result = await _pdfBinary.Execute(data, timeout);

            if (!result.Success)
            {
                throw new PdfConvertException("call to executable failed");
            }

            if (result.Stderr.Length != 0 && result.ExitCode != 0)
            {
                throw new PdfConvertException($"{Encoding.UTF8.GetString(result.Stderr)} ; exit code {result.ExitCode}");                
            }
            
            if (result.Stdout.Length == 0)
            {
                throw new PdfConvertException("output empty");                
            }
            
            return result.Stdout;
        }
    }
}