using System;
using System.Collections.Generic;
using System.Linq;

namespace XML_validator.Models;

public sealed class XmlValidationResultViewModel
{
    public string SchemaFileName { get; set; } = string.Empty;
    public string XmlFileName { get; set; } = string.Empty;

    public List<XmlValidationErrorViewModel> Messages { get; } = [];

    public bool IsValid => Messages.All(message =>
        !string.Equals(message.Severity, "Error", StringComparison.OrdinalIgnoreCase));
}