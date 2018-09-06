using XDemo.UI.Models.Validations.Base;

namespace XDemo.UI.Models.Validations.DefinedRules
{
    public class MaxValueRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            throw new System.NotImplementedException();
        }
    }

}
