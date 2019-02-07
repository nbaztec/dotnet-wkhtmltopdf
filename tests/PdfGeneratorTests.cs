using System.Threading.Tasks;

namespace WkHtmlToPdfTests
{
    using System;
    using System.Text;
    using NSubstitute;
    using Xunit;
    using WkHtmlToPdf;

    public class PdfGeneratorTests
    {
        [Fact]
        public async Task ShouldReturnPdfBytesWhenSuccessful()
        {
            var expectedBytes =  Encoding.ASCII.GetBytes("test-pdf");
            
            var wkHtmlToPdfBinary = Substitute.For<IWkHtmlToPdfBinary>();
            wkHtmlToPdfBinary.Execute(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(Task.FromResult(new ExecutionResult
            {
                Stderr = new byte[]{},
                Stdout = expectedBytes,
                Success = true,
            }));
            
            
            var pdf = new PdfGenerator(wkHtmlToPdfBinary);
            var result = await pdf.Generate("test");

            Assert.Equal(result, expectedBytes);
        }
        
        [Fact]
        public async Task ShouldReturnPdfBytesWhenStderrExistsButExitCodeZeroAndProcessSuccessful()
        {
            var expectedBytes =  Encoding.ASCII.GetBytes("test-pdf");

            var wkHtmlToPdfBinary = Substitute.For<IWkHtmlToPdfBinary>();
            wkHtmlToPdfBinary.Execute(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(Task.FromResult(new ExecutionResult
            {
                Stderr = new byte[]{ 70 },
                Stdout = expectedBytes,
                Success = true,
                ExitCode = 0
            }));
            
            var pdf = new PdfGenerator(wkHtmlToPdfBinary);
            var result = await pdf.Generate("test");
            
            Assert.Equal(result, expectedBytes);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ShouldReturnExceptionWhenInputEmpty(string input)
        {
            var wkHtmlToPdfBinary = Substitute.For<IWkHtmlToPdfBinary>();
            wkHtmlToPdfBinary.Execute(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(Task.FromResult(new ExecutionResult()));
            
            var pdf = new PdfGenerator(wkHtmlToPdfBinary);

            await Assert.ThrowsAsync<PdfConvertException>(() => pdf.Generate(input));
        }
        
        [Theory]
        [InlineData(new byte[]{})]
        [InlineData(null)]
        public async Task ShouldReturnExceptionWhenOutputEmpty(byte[] output)
        {
            var wkHtmlToPdfBinary = Substitute.For<IWkHtmlToPdfBinary>();
            wkHtmlToPdfBinary.Execute(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(Task.FromResult(new ExecutionResult
            {
                Stdout = output,
            }));
            
            var pdf = new PdfGenerator(wkHtmlToPdfBinary);

            await Assert.ThrowsAsync<PdfConvertException>(() => pdf.Generate("test"));
        }
        
        [Fact]
        public async Task ShouldReturnExceptionWhenStderrExistsAndExitCodeNonZero()
        {
            var wkHtmlToPdfBinary = Substitute.For<IWkHtmlToPdfBinary>();
            wkHtmlToPdfBinary.Execute(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(Task.FromResult(new ExecutionResult
            {
                Stderr = Encoding.UTF8.GetBytes("an error occurred"),
                ExitCode = 1
            }));
            
            var pdf = new PdfGenerator(wkHtmlToPdfBinary);

            await Assert.ThrowsAsync<PdfConvertException>(() => pdf.Generate("test"));
        }
        
        [Fact]
        public async Task ShouldReturnExceptionWhenProcessFailsOrTimesout()
        {
            var wkHtmlToPdfBinary = Substitute.For<IWkHtmlToPdfBinary>();
            wkHtmlToPdfBinary.Execute(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(Task.FromResult(new ExecutionResult
            {
                Success = false,
            }));
            
            var pdf = new PdfGenerator(wkHtmlToPdfBinary);

            await Assert.ThrowsAsync<PdfConvertException>(async () => await pdf.Generate("test"));
        }
    }
}