using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XML_validator.Models;

namespace XML_validator.Services;

public sealed class XmlValidationService
{
    private const string TargetNamespace = "http://www.algonquincollege.com/cst8259/labs";

    public XmlValidationResultViewModel Validate(Stream schemaStream, Stream xmlStream)
    {
        var result = new XmlValidationResultViewModel();

        try
        {
            var schemas = new XmlSchemaSet
            {
                XmlResolver = null
            };

            using (var schemaReader = XmlReader.Create(schemaStream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreWhitespace = true
            }))
            {
                schemas.Add(TargetNamespace, schemaReader);
            }

            schemas.Compile();

            XDocument document;
            using (var xmlReader = XmlReader.Create(xmlStream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreWhitespace = false,
                IgnoreComments = false,
                ConformanceLevel = ConformanceLevel.Document
            }))
            {
                document = XDocument.Load(xmlReader, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
            }

            if (document.Root is null)
            {
                result.Messages.Add(new XmlValidationErrorViewModel
                {
                    ElementName = "xml",
                    Severity = "Error",
                    LineNumber = 0,
                    ColumnNumber = 0,
                    Message = "The XML document does not have a root element."
                });

                return result;
            }

            if (document.Root.Name.NamespaceName != TargetNamespace)
            {
                var lineInfo = (IXmlLineInfo)document.Root;

                result.Messages.Add(new XmlValidationErrorViewModel
                {
                    ElementName = document.Root.Name.LocalName,
                    Severity = "Error",
                    LineNumber = lineInfo.LineNumber,
                    ColumnNumber = lineInfo.LinePosition,
                    Message = $"The root element must be in the namespace '{TargetNamespace}'."
                });
            }

            document.Validate(schemas, (sender, e) =>
            {
                var elementName = "unknown";
                var lineNumber = 0;
                var columnNumber = 0;

                if (sender is XElement element)
                {
                    elementName = element.Name.LocalName;
                    if (element is IXmlLineInfo lineInfo)
                    {
                        lineNumber = lineInfo.LineNumber;
                        columnNumber = lineInfo.LinePosition;
                    }
                }
                else if (sender is XDocument xDocument && xDocument.Root is not null)
                {
                    elementName = xDocument.Root.Name.LocalName;
                    if (xDocument.Root is IXmlLineInfo lineInfo)
                    {
                        lineNumber = lineInfo.LineNumber;
                        columnNumber = lineInfo.LinePosition;
                    }
                }

                result.Messages.Add(new XmlValidationErrorViewModel
                {
                    ElementName = elementName,
                    Severity = e.Severity.ToString(),
                    LineNumber = lineNumber,
                    ColumnNumber = columnNumber,
                    Message = e.Message
                });
            }, true);
        }
        catch (XmlSchemaException ex)
        {
            result.Messages.Add(new XmlValidationErrorViewModel
            {
                ElementName = "schema",
                Severity = "Error",
                LineNumber = ex.LineNumber,
                ColumnNumber = ex.LinePosition,
                Message = ex.Message
            });
        }
        catch (XmlException ex)
        {
            result.Messages.Add(new XmlValidationErrorViewModel
            {
                ElementName = "xml",
                Severity = "Error",
                LineNumber = ex.LineNumber,
                ColumnNumber = ex.LinePosition,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            result.Messages.Add(new XmlValidationErrorViewModel
            {
                ElementName = "validator",
                Severity = "Error",
                Message = ex.Message
            });
        }

        return result;
    }
}