namespace WkHtmlToPdf
{
    public struct ExecutionResult
    {
        public bool Success;
        public int ExitCode;
        public byte[] Stdout;
        public byte[] Stderr;
    }
}