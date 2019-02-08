# WkHtmlToPdf wrapper for C#

```
var pdf = PdfGenerator.MakeDefault();

try
{
  var bytes = await pdf.Generate(File.ReadAllText("test.html"));
}
catch (Exception e)
{
  Console.WriteLine(e.Message);
}

File.WriteAllBytes("test.pdf", bytes);
```

Requires [wkhtmltopdf](https://wkhtmltopdf.org/downloads.html) binary in the system PATH.
