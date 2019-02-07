using System;

namespace WkHtmlToPdf
{
    public class PdfConvertException : Exception
    {
        public PdfConvertException(string message)
            : base(message)
        {
        }
    }
}