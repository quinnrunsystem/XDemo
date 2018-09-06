using XDemo.UI.Models.Validations.Base;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    public class IsNotNullOrEmptyCollectionRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            var str = value as string;
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
