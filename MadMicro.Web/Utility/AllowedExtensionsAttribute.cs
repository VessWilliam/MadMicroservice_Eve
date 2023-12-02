using System.ComponentModel.DataAnnotations;

namespace MadMicro.Web.Utility;

public class AllowedExtensionsAttribute : ValidationAttribute
{

    private readonly string[] _extensions;
    public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;


    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        if (value is not FormFile formFile)
            return new ValidationResult("Invalid File");

        var extension = Path.GetExtension(formFile.FileName).ToLower();

        if (!_extensions.Contains(extension))
            return new ValidationResult("This Photo extension is not allowed");

        return ValidationResult.Success;
    }
}
