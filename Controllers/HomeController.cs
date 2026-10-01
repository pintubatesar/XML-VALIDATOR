using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XML_validator.Models;
using XML_validator.Services;

namespace XML_validator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly XmlValidationService _xmlValidationService;

        public HomeController(ILogger<HomeController> logger, XmlValidationService xmlValidationService)
        {
            _logger = logger;
            _xmlValidationService = xmlValidationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new XmlUploadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(XmlUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using var schemaStream = model.SchemaFile!.OpenReadStream();
                using var xmlStream = model.XmlFile!.OpenReadStream();

                var result = _xmlValidationService.Validate(schemaStream, xmlStream);
                result.SchemaFileName = model.SchemaFile.FileName;
                result.XmlFileName = model.XmlFile.FileName;

                return View("ValidationResult", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation failed.");
                ModelState.AddModelError(string.Empty, "Unable to validate the uploaded files.");
                return View(model);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
