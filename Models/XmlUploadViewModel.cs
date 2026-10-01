using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace XML_validator.Models;

public sealed class XmlUploadViewModel
{
    [Display(Name = "Schema file")]
    [Required(ErrorMessage = "Please choose an XSD file.")]
    public IFormFile? SchemaFile { get; set; }

    [Display(Name = "XML file")]
    [Required(ErrorMessage = "Please choose an XML file.")]
    public IFormFile? XmlFile { get; set; }
}