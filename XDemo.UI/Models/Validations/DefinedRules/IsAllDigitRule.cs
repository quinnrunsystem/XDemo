using XDemo.UI.Models.Validations.Base;
using System.Linq;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    /// <summary>
    /// [String] Is all digit rule.
    /// </summary>
    public class IsAllDigitRule<T> : IValidationRule<T> 
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            //for string only
            var str = value as string;
            return !string.IsNullOrWhiteSpace(str) && str.All(c => char.IsDigit(c));
        }
    }
}
