namespace XML_validator.Models;

public sealed class XmlValidationErrorViewModel
{
    public string ElementName { get; set; } = "unknown";
    public string Severity { get; set; } = "Error";
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
    public string Message { get; set; } = string.Empty;
}