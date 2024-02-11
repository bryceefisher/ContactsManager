using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Helpers;

internal abstract class ValidationHelper
{
    /// <summary>
    ///Validates attributes of provided object.
    /// </summary>
    /// <param name="obj">Object to validate</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ModelValidation(object obj)
    {
        //Model Validations
        ValidationContext validationContext = new ValidationContext(obj);
        List<ValidationResult> results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(obj, validationContext, results, true);

        if (!isValid)
        {
            throw new ArgumentException(results.FirstOrDefault()?.ErrorMessage);
        }
    }
}