namespace WkHtmlToPdf
{
    using System;
    using System.Threading.Tasks;

    public interface IWkHtmlToPdfBinary
    {
        Task<ExecutionResult> Execute(byte[] stdin, TimeSpan timeout);
    }
}