using System.ComponentModel.DataAnnotations;

namespace MadMicro.Web.Utility;

public class MaxFileSizeAttribute : ValidationAttribute
{

    private readonly int _maxFileSize;  
    public MaxFileSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        
        if (value is not FormFile formFile)
            return new ValidationResult("Invalid File");

        if (formFile.Length > (_maxFileSize * 1024 * 1024))
            return new ValidationResult($"Max allow file size is {_maxFileSize} MB.");

        return ValidationResult.Success;
    }
}

