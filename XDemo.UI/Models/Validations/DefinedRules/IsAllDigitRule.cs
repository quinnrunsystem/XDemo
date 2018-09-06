using XDemo.UI.Models.Validations.Base;
using System.Linq;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    public class IsAllDigitRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return str.All(c => char.IsDigit(c));
        }
    }

}
